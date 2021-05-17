using System.Collections.Generic;

public class UIBottomPanelWallsTabMediator : UIBottomPanelInteriorTabMediatorBase<ShopDecorationConfigDto>
{
    private readonly IWallsConfig _wallsConfig;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;

    public UIBottomPanelWallsTabMediator(BottomPanelView view)
        : base(view)
    {
        _wallsConfig = GameConfigManager.Instance.MainConfig;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    protected override IEnumerable<(int id, ShopDecorationConfigDto config)> GetConfigsForLevel(int level)
    {
        return _wallsConfig.GetWallsConfigsForLevel(level);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, (int id, ShopDecorationConfigDto config) configData)
    {
        var config = configData.config;
        itemView.SetupIconSize(110);
        itemView.SetImage(_spritesProvider.GetWallIcon(configData.id));
        itemView.SetPrice(Price.FromString(config.price));
    }

    protected override void HandleClick(int id)
    {
        _dispatcher.UIBottomPanelPlaceWallClicked(id);
    }
}
