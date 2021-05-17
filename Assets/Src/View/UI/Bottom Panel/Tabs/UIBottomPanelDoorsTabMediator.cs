using System.Collections.Generic;

public class UIBottomPanelDoorsTabMediator : UIBottomPanelInteriorTabMediatorBase<ShopDecorationConfigDto>
{
    private readonly IDoorsConfig doorsConfig;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;

    public UIBottomPanelDoorsTabMediator(BottomPanelView view)
        : base(view)
    {
        doorsConfig = GameConfigManager.Instance.MainConfig;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    protected override IEnumerable<(int id, ShopDecorationConfigDto config)> GetConfigsForLevel(int level)
    {
        return doorsConfig.GetDoorsConfigsForLevel(level);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, (int id, ShopDecorationConfigDto config) configData)
    {
        var config = configData.config;
        itemView.SetupIconSize(110);
        itemView.SetImage(_spritesProvider.GetDoorIcon(configData.id));
        itemView.SetPrice(Price.FromString(config.price));
    }

    protected override void HandleClick(int id)
    {
        _dispatcher.UIBottomPanelPlaceDoorClicked(id);
    }
}
