using System.Collections.Generic;

public class UIBottomPanelShelfsTabMediator : UIBottomPanelSubMediatorBase
{
    private readonly IShelfsConfig _shelfsConfig;
    private readonly PlayerModel _playerModel;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly LocalizationManager _localizationManager;
    private readonly Dictionary<UIBottomPanelScrollItemView, int> _shelfIdsByItem;

    public UIBottomPanelShelfsTabMediator(BottomPanelView view)
        : base(view)
    {
        _shelfsConfig = GameConfigManager.Instance.MainConfig;
        _playerModel = PlayerModel.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _localizationManager = LocalizationManager.Instance;

        _shelfIdsByItem = new Dictionary<UIBottomPanelScrollItemView, int>();
    }

    public override void Mediate()
    {
        base.Mediate();

        ShowScrollBox();

        var configsToShow = _shelfsConfig.GetShelfConfigsForLevel(_playerModel.Level);
        foreach (var shelfConfig in configsToShow)
        {
            var itemView = GetOrCreateScrollBoxItem();
            _shelfIdsByItem.Add(itemView, shelfConfig.id);
            SetupItem(itemView, shelfConfig);
            ActivateItem(itemView);
        }
    }

    public override void Unmediate()
    {
        var itemViews = _shelfIdsByItem.Keys;
        foreach (var itemView in itemViews)
        {
            DeactivateItem(itemView);
            ReturnOrDestroyScrollBoxItem(itemView);
        }

        HideScrollBox();

        base.Unmediate();
    }

    private void SetupItem(UIBottomPanelScrollItemView itemView, (int id, ShelfConfigDto config) shelfConfigData)
    {
        var config = shelfConfigData.config;
        itemView.SetImage(_spritesProvider.GetShelfIcon(shelfConfigData.id));
        itemView.SetPrice(Price.FromString(config.price));

        var shelfName = _localizationManager.GetLocalization($"{LocalizationKeys.NameShopObjectPrefix}s_{shelfConfigData.id}");
        var hintDescriptionRaw = _localizationManager.GetLocalization(LocalizationKeys.HintBottomPanelShelfDescription);
        var hintDescription = string.Format(hintDescriptionRaw, shelfName, config.part_volume * config.parts_num, config.parts_num);
        itemView.SetupHint(hintDescription);
    }

    private void ActivateItem(UIBottomPanelScrollItemView itemView)
    {
        itemView.Clicked += OnItemClicked;
    }

    private void DeactivateItem(UIBottomPanelScrollItemView itemView)
    {
        itemView.Clicked -= OnItemClicked;
    }

    private void OnItemClicked(UIBottomPanelScrollItemView itemView)
    {
        var shelfId = _shelfIdsByItem[itemView];
        _dispatcher.UIBottomPanelPlaceShelfClicked(shelfId);
    }
}
