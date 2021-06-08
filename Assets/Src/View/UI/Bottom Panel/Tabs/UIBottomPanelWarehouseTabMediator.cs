using System.Collections.Generic;
using UnityEngine;

public class UIBottomPanelWarehouseTabMediator : UIBottomPanelScrollItemsTabMediatorBase<ProductSlotModel>
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

    protected override IEnumerable<ProductSlotModel> GetViewModelsToShow()
    {
        return _warehouseModel.Slots;
    }

    protected override void HandleClick(ProductSlotModel slotModel)
    {
        _dispatcher.UIBottomPanelWarehouseSlotClicked(slotModel.Index);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, ProductSlotModel viewModel)
    {
        ActivateItem(viewModel);
        if (viewModel.Product != null)
        {
            var product = viewModel.Product;
            var config = viewModel.Product.Config;
            var icon = _spritesProvider.GetProductIcon(config.Key);

            itemView.SetupIconSize(110);
            itemView.SetImage(icon);

            var deltaDeliver = product.DeliverTime - _gameStateModel.ServerTime;
            if (deltaDeliver > 0)
            {
                itemView.SetTopText(FormattingHelper.ToSeparatedTimeFormat(deltaDeliver));
                //TODO: show deliver mode
                //TODO: subscribe OnSecondPassed
            }
            else
            {
                UpdateProductAmount(itemView, product);
            }
        }
    }

    protected override void BeforeHideItem(UIBottomPanelScrollItemView itemView, ProductSlotModel viewModel)
    {
        base.BeforeHideItem(itemView, viewModel);
        DeactivateItem(viewModel);
    }

    private void ActivateItem(ProductSlotModel viewModel)
    {
        //viewModel
    }

    private void DeactivateItem(ProductSlotModel viewModel)
    {
        //throw new System.NotImplementedException();
    }

    private void UpdateProductAmount(UIBottomPanelScrollItemView itemView, ProductModel product)
    {
        itemView.SetTopText(product.Amount.ToString());
        var fullness = (float)(product.Amount * product.Config.Volume) / _warehouseModel.Volume;
        itemView.SetPercentLineXScaleMultiplier(fullness);
        var color = Color.Lerp(Color.red, Color.yellow, fullness);
        itemView.SetPercentLineColor(color);
    }
}
