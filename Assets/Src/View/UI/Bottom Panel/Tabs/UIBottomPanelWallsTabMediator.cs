using System.Collections.Generic;

public class UIBottomPanelWallsTabMediator : UIBottomPanelScrollItemsTabMediatorBase<ItemConfig<ShopDecorationConfigDto>>
{
    private readonly IWallsConfig _wallsConfig;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly ShopModel _playerShopModel;

    public UIBottomPanelWallsTabMediator(BottomPanelView view)
        : base(view)
    {
        _wallsConfig = GameConfigManager.Instance.MainConfig;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _playerShopModel = GameStateModel.Instance.PlayerShopModel;
    }

    public override void Mediate()
    {
        base.Mediate();
        View.SetDecorationButtonSelected(View.InteriorWallsButton);
    }

    public override void Unmediate()
    {
        View.SetDecorationButtonUnselected(View.InteriorWallsButton);
        base.Unmediate();
    }

    protected override IEnumerable<ItemConfig<ShopDecorationConfigDto>> GetViewModelsToShow()
    {
        return _wallsConfig.GetWallsConfigsForLevel(_playerShopModel.ProgressModel.Level);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, ItemConfig<ShopDecorationConfigDto> viewModel)
    {
        itemView.SetupIconSize(110);
        itemView.SetImage(_spritesProvider.GetWallIcon(viewModel.NumericId));
        itemView.SetPrice(viewModel.Price);
    }

    protected override void HandleClick(ItemConfig<ShopDecorationConfigDto> viewModel)
    {
        _dispatcher.UIBottomPanelPlaceWallClicked(viewModel.NumericId);
    }
}
