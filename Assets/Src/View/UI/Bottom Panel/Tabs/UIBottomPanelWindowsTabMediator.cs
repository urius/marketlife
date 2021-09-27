using System.Collections.Generic;

public class UIBottomPanelWindowsTabMediator : UIBottomPanelInteriorModeScrollItemsTabMediatorBase<ItemConfig<ShopDecorationConfigDto>>
{
    private readonly IWindowsConfig _windowsConfig;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly UserModel _playerModel;

    public UIBottomPanelWindowsTabMediator(BottomPanelView view)
        : base(view)
    {
        _windowsConfig = GameConfigManager.Instance.MainConfig;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _playerModel = PlayerModelHolder.Instance.UserModel;
    }

    public override void Mediate()
    {
        base.Mediate();
        View.SetButtonSelected(View.InteriorWindowsButton);
    }

    public override void Unmediate()
    {
        View.SetButtonUnselected(View.InteriorWindowsButton);
        base.Unmediate();
    }

    protected override IEnumerable<ItemConfig<ShopDecorationConfigDto>> GetViewModelsToShow()
    {
        return _windowsConfig.GetWindowsConfigsForLevel(_playerModel.ProgressModel.Level);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, ItemConfig<ShopDecorationConfigDto> configData)
    {
        var configDto = configData.ConfigDto;
        itemView.SetupIconSize(80);
        itemView.SetImage(_spritesProvider.GetWindowIcon(configData.NumericId));
        itemView.SetPrice(Price.FromString(configDto.price));
    }

    protected override void HandleClick(ItemConfig<ShopDecorationConfigDto> viewModel)
    {
        _dispatcher.UIBottomPanelPlaceWindowClicked(viewModel.NumericId);
    }
}
