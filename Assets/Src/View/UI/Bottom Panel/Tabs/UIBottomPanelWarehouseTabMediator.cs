using System.Collections.Generic;
using UnityEngine;

public class UIBottomPanelWarehouseTabMediator : UIBottomPanelScrollItemsTabMediatorBase<ProductSlotModel>
{
    private readonly ShopWarehouseModel _warehouseModel;
    private readonly LocalizationManager _loc;
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
        _loc = LocalizationManager.Instance;
    }

    protected override IEnumerable<ProductSlotModel> GetViewModelsToShow()
    {
        return _warehouseModel.Slots;
    }

    protected override void HandleClick(ProductSlotModel slotModel)
    {
        _dispatcher.UIBottomPanelWarehouseSlotClicked(slotModel.Index);
    }

    protected override void SetupItem(UIBottomPanelScrollItemView itemView, ProductSlotModel slotModel)
    {
        ActivateItem(slotModel);
        UpdateSlotView(itemView, slotModel);
    }

    protected override void BeforeHideItem(UIBottomPanelScrollItemView itemView, ProductSlotModel viewModel)
    {
        base.BeforeHideItem(itemView, viewModel);
        DeactivateItem(viewModel);
    }

    private void ActivateItem(ProductSlotModel slotModel)
    {
        slotModel.ProductAmountChanged += OnProductAmountChanged;
        slotModel.ProductRemoved += OnProductRemoved;
    }

    private void DeactivateItem(ProductSlotModel slotModel)
    {
        slotModel.ProductAmountChanged -= OnProductAmountChanged;
        slotModel.ProductRemoved -= OnProductRemoved;
    }

    private void OnProductRemoved(int slotIndex, ProductModel removedProduct)
    {
        UpdateSlotViewByIndex(slotIndex);
    }

    private void OnProductAmountChanged(int slotIndex, int deltaAmount)
    {
        UpdateSlotViewByIndex(slotIndex);
    }

    private void UpdateProductAmount(UIBottomPanelScrollItemView itemView, ProductModel product)
    {
        itemView.SetTopText(product.Amount.ToString());
        var fullness = (float)(product.Amount * product.Config.Volume) / _warehouseModel.Volume;
        itemView.SetPercentLineXScaleMultiplier(fullness);
        var color = Color.Lerp(Color.red, Color.yellow, fullness);
        itemView.SetPercentLineColor(color);
    }

    private void UpdateSlotViewByIndex(int slotIndex)
    {
        var slotmodel = _warehouseModel.Slots[slotIndex];
        var view = GetViewByViewModel(slotmodel);
        UpdateSlotView(view, slotmodel);
    }

    private void UpdateSlotView(UIBottomPanelScrollItemView itemView, ProductSlotModel slotModel)
    {
        if (slotModel.HasProduct)
        {
            var product = slotModel.Product;
            var config = slotModel.Product.Config;
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
        else
        {
            itemView.Reset();
            itemView.SetupIconSize(110);
            itemView.SetImage(_spritesProvider.GetOrderIcon());
            itemView.SetTopText(_loc.GetLocalization(LocalizationKeys.BottomPanelWarehouseEmptySlot));
            itemView.SetupHint(_loc.GetLocalization(LocalizationKeys.BottomPanelWarehouseEmptySlotHint));
        }
    }
}
