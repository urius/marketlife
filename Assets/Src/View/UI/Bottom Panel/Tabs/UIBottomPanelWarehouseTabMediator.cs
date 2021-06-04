using System.Collections.Generic;
using UnityEngine;

public class UIBottomPanelWarehouseTabMediator : UIBottomPanelScrollItemsTabMediatorBase<WarehouseSlotModel>
{
    private readonly ShopWarehouseModel _warehouseModel;
    private readonly Dispatcher _dispatcher;
    private readonly SpritesProvider _spritesProvider;

    public UIBottomPanelWarehouseTabMediator(BottomPanelView view)
        : base(view)
    {
        _warehouseModel = PlayerModel.Instance.ShopModel.WarehouseModel;
        _dispatcher = Dispatcher.Instance;
        _spritesProvider = SpritesProvider.Instance;
    }

    protected override IEnumerable<WarehouseSlotModel> GetViewModelsToShow()
    {
        return _warehouseModel.Slots;
    }

    protected override void HandleClick(WarehouseSlotModel slotModel)
    {
        _dispatcher.UIBottomPanelWarehouseSlotClicked(slotModel.Index);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, WarehouseSlotModel viewModel)
    {
        if (viewModel.Product != null)
        {
            var config = viewModel.Product.Config;
            var icon = _spritesProvider.GetProductIcon(config.Key);

            itemView.SetupIconSize(110);
            itemView.SetImage(icon);
        }
    }
}
