using System.Collections.Generic;
using UnityEngine;

public class UIBottomPanelWarehouseTabMediator : UIBottomPanelScrollItemsTabMediatorBase<WarehouseSlotModel>
{
    private readonly ShopWarehouseModel _warehouseModel;
    private readonly Dispatcher _dispatcher;
    private readonly SpritesProvider _spritesProvider;
    private readonly GameStateModel _gameStateModel;

    public UIBottomPanelWarehouseTabMediator(BottomPanelView view)
        : base(view)
    {
        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _warehouseModel = PlayerModel.Instance.ShopModel.WarehouseModel;
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
            var product = viewModel.Product;
            var config = viewModel.Product.Config;
            var icon = _spritesProvider.GetProductIcon(config.Key);

            itemView.SetupIconSize(110);
            itemView.SetImage(icon);

            if (product.DeliverTime > _gameStateModel.ServerTime)
            {
                //TODO: show deliver mode
                //TODO: subscribe OnSecondPassed
            }
            else
            {
                itemView.SetTopText(product.Amount.ToString());

                var fullness = (float)(product.Amount * config.Volume) / _warehouseModel.Volume;
                itemView.SetPercentLineXScaleMultiplier(fullness);

                var color = Color.Lerp(Color.red, Color.yellow, fullness);
                itemView.SetPercentLineColor(color);
            }
        }
    }
}
