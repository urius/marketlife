using System.Collections.Generic;

public class UIBottomPanelShelfsTabMediator : UIBottomPanelInteriorTabMediatorBase<ShelfConfigDto>
{
    private readonly IShelfsConfig _shelfsConfig;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly LocalizationManager _localizationManager;

    public UIBottomPanelShelfsTabMediator(BottomPanelView view)
        : base(view)
    {
        _shelfsConfig = GameConfigManager.Instance.MainConfig;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _localizationManager = LocalizationManager.Instance;
    }

    protected override IEnumerable<(int id, ShelfConfigDto config)> GetConfigsForLevel(int level)
    {
        return _shelfsConfig.GetShelfConfigsForLevel(level);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, (int id, ShelfConfigDto config) shelfConfigData)
    {
        var config = shelfConfigData.config;
        itemView.SetImage(_spritesProvider.GetShelfIcon(shelfConfigData.id));
        itemView.SetPrice(Price.FromString(config.price));

        var shelfName = _localizationManager.GetLocalization($"{LocalizationKeys.NameShopObjectPrefix}s_{shelfConfigData.id}");
        var hintDescriptionRaw = _localizationManager.GetLocalization(LocalizationKeys.HintBottomPanelShelfDescription);
        var hintDescription = string.Format(hintDescriptionRaw, shelfName, config.part_volume * config.parts_num, config.parts_num);
        itemView.SetupHint(hintDescription);
    }

    protected override void HandleClick(int id)
    {
        _dispatcher.UIBottomPanelPlaceShelfClicked(id);
    }
}
