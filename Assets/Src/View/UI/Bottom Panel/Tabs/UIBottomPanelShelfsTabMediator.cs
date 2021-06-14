using System.Collections.Generic;

public class UIBottomPanelShelfsTabMediator : UIBottomPanelScrollItemsTabMediatorBase<ItemConfig<ShelfConfigDto>>
{
    private readonly IShelfsConfig _shelfsConfig;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly LocalizationManager _localizationManager;
    private readonly ShopModel _playerShopModel;

    public UIBottomPanelShelfsTabMediator(BottomPanelView view)
        : base(view)
    {
        _shelfsConfig = GameConfigManager.Instance.MainConfig;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _localizationManager = LocalizationManager.Instance;
        _playerShopModel = GameStateModel.Instance.PlayerShopModel;
    }

    public override void Mediate()
    {
        base.Mediate();
        View.SetDecorationButtonSelected(View.InteriorObjectsButton);
    }

    public override void Unmediate()
    {
        View.SetDecorationButtonUnselected(View.InteriorObjectsButton);
        base.Unmediate();
    }

    protected override IEnumerable<ItemConfig<ShelfConfigDto>> GetViewModelsToShow()
    {
        return _shelfsConfig.GetShelfConfigsForLevel(_playerShopModel.ProgressModel.Level);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, ItemConfig<ShelfConfigDto> viewModel)
    {
        var configDto = viewModel.ConfigDto;
        itemView.SetImage(_spritesProvider.GetShelfIcon(viewModel.NumericId));
        itemView.SetPrice(Price.FromString(configDto.price));

        var shelfName = _localizationManager.GetLocalization($"{LocalizationKeys.NameShopObjectPrefix}s_{viewModel.NumericId}");
        var hintDescriptionRaw = _localizationManager.GetLocalization(LocalizationKeys.HintBottomPanelShelfDescription);
        var hintDescription = string.Format(hintDescriptionRaw, shelfName, configDto.part_volume * configDto.parts_num, configDto.parts_num);
        itemView.SetupMainHint(hintDescription);
    }

    protected override void HandleClick(ItemConfig<ShelfConfigDto> viewModel)
    {
        _dispatcher.UIBottomPanelPlaceShelfClicked(viewModel.NumericId);
    }
}
