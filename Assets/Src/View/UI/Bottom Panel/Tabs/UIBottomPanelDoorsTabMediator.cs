using System.Collections.Generic;

public class UIBottomPanelDoorsTabMediator : UIBottomPanelInteriorTabMediatorBase<ItemConfig<ShopDecorationConfigDto>>
{
    private readonly IDoorsConfig doorsConfig;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly ShopModel _playerShopModel;

    public UIBottomPanelDoorsTabMediator(BottomPanelView view)
        : base(view)
    {
        doorsConfig = GameConfigManager.Instance.MainConfig;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _playerShopModel = GameStateModel.Instance.PlayerShopModel;
    }

    public override void Mediate()
    {
        base.Mediate();
        View.SetDecorationButtonSelected(View.InteriorDoorsButton);
    }

    public override void Unmediate()
    {
        View.SetDecorationButtonUnselected(View.InteriorDoorsButton);
        base.Unmediate();
    }

    protected override IEnumerable<ItemConfig<ShopDecorationConfigDto>> GetViewModelsToShow()
    {
        return doorsConfig.GetDoorsConfigsForLevel(_playerShopModel.ProgressModel.Level);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, ItemConfig<ShopDecorationConfigDto> viewModel)
    {
        itemView.SetupIconSize(110);
        itemView.SetImage(_spritesProvider.GetDoorIcon(viewModel.NumericId));
        itemView.SetPrice(viewModel.Price);
    }

    protected override void HandleClick(ItemConfig<ShopDecorationConfigDto> viewModel)
    {
        _dispatcher.UIBottomPanelPlaceDoorClicked(viewModel.NumericId);
    }
}
