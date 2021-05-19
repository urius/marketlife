using System.Collections.Generic;

public class UIBottomPanelFloorsTabMediator : UIBottomPanelInteriorTabMediatorBase<ShopDecorationConfigDto>
{
    private readonly IFloorsConfig _floorsConfig;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;

    public UIBottomPanelFloorsTabMediator(BottomPanelView view)
        : base(view)
    {
        _floorsConfig = GameConfigManager.Instance.MainConfig;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();
        View.SetDecorationButtonSelected(View.InteriorFloorsButton);
    }

    public override void Unmediate()
    {
        View.SetDecorationButtonUnselected(View.InteriorFloorsButton);
        base.Unmediate();
    }

    protected override IEnumerable<(int id, ShopDecorationConfigDto config)> GetConfigsForLevel(int level)
    {
        return _floorsConfig.GetFloorsConfigsForLevel(level);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, (int id, ShopDecorationConfigDto config) configData)
    {
        var config = configData.config;
        itemView.SetupIconSize(80);
        itemView.SetImage(_spritesProvider.GetFloorIcon(configData.id));
        itemView.SetPrice(Price.FromString(config.price));
    }

    protected override void HandleClick(int id)
    {
        _dispatcher.UIBottomPanelPlaceFloorClicked(id);
    }
}
