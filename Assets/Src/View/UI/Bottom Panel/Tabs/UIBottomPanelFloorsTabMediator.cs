using System.Collections.Generic;

public class UIBottomPanelFloorsTabMediator : UIBottomPanelInteriorModeScrollItemsTabMediatorBase<ItemConfig<ShopDecorationConfigDto>>
{
    private readonly IFloorsConfig _floorsConfig;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly UserModel _playerModel;

    public UIBottomPanelFloorsTabMediator(BottomPanelView view)
        : base(view)
    {
        _floorsConfig = GameConfigManager.Instance.MainConfig;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _playerModel = PlayerModelHolder.Instance.UserModel;
    }

    public override void Mediate()
    {
        base.Mediate();
        View.SetButtonSelected(View.InteriorFloorsButton);
    }

    public override void Unmediate()
    {
        View.SetButtonUnselected(View.InteriorFloorsButton);
        base.Unmediate();
    }

    protected override IEnumerable<ItemConfig<ShopDecorationConfigDto>> GetViewModelsToShow()
    {
        return _floorsConfig.GetFloorsConfigsForLevel(_playerModel.ProgressModel.Level);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, ItemConfig<ShopDecorationConfigDto> viewModel)
    {
        itemView.SetupIconSize(80);
        itemView.SetImage(_spritesProvider.GetFloorIcon(viewModel.NumericId));
        itemView.SetPrice(viewModel.Price);
    }

    protected override void HandleClick(ItemConfig<ShopDecorationConfigDto> viewModel)
    {
        _dispatcher.UIBottomPanelPlaceFloorClicked(viewModel.NumericId);
    }
}
