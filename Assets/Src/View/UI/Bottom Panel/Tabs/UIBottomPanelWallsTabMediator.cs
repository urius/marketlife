using System.Collections.Generic;
using Src.Common;

public class UIBottomPanelWallsTabMediator : UIBottomPanelInteriorModeScrollItemsTabMediatorBase<ItemConfig<ShopDecorationConfigDto>>
{
    private readonly IWallsConfig _wallsConfig;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly UserModel _playerModel;

    public UIBottomPanelWallsTabMediator(BottomPanelView view)
        : base(view)
    {
        _wallsConfig = GameConfigManager.Instance.MainConfig;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _playerModel = PlayerModelHolder.Instance.UserModel;
    }

    public override void Mediate()
    {
        base.Mediate();
        View.SetButtonSelected(View.InteriorWallsButton);
    }

    public override void Unmediate()
    {
        View.SetButtonUnselected(View.InteriorWallsButton);
        base.Unmediate();
    }

    protected override IEnumerable<ItemConfig<ShopDecorationConfigDto>> GetViewModelsToShow()
    {
        return _wallsConfig.GetWallsConfigsForLevel(_playerModel.ProgressModel.Level);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, ItemConfig<ShopDecorationConfigDto> viewModel)
    {
        itemView.SetupIconSize(110);
        itemView.SetImage(_spritesProvider.GetWallSprite(viewModel.NumericId));
        itemView.SetPrice(viewModel.Price);
    }

    protected override void HandleClick(ItemConfig<ShopDecorationConfigDto> viewModel)
    {
        _dispatcher.UIBottomPanelPlaceWallClicked(viewModel.NumericId);
    }
}
