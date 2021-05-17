using System.Collections.Generic;

public class UIBottomPanelWindowsTabMediator : UIBottomPanelInteriorTabMediatorBase<ShopDecorationConfigDto>
{
    private readonly IWindowsConfig _windowsConfig;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;

    public UIBottomPanelWindowsTabMediator(BottomPanelView view)
        : base(view)
    {
        _windowsConfig = GameConfigManager.Instance.MainConfig;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    protected override IEnumerable<(int id, ShopDecorationConfigDto config)> GetConfigsForLevel(int level)
    {
        return _windowsConfig.GetWindowsConfigsForLevel(level);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, (int id, ShopDecorationConfigDto config) configData)
    {
        var config = configData.config;
        itemView.SetupIconSize(80);
        itemView.SetImage(_spritesProvider.GetWindowIcon(configData.id));
        itemView.SetPrice(Price.FromString(config.price));
    }

    protected override void HandleClick(int id)
    {
        _dispatcher.UIBottomPanelPlaceWindowClicked(id);
    }
}
