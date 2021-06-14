using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBottomPanelWarehouseTabMediator : UIBottomPanelScrollItemsTabMediatorBase<ProductSlotModel>
{
    private readonly ShopWarehouseModel _warehouseModel;
    private readonly GameConfigManager _configManager;
    private readonly UpdatesProvider _updatesProvider;
    private readonly LocalizationManager _loc;
    private readonly Dispatcher _dispatcher;
    private readonly SpritesProvider _spritesProvider;
    private readonly GameStateModel _gameStateModel;
    private readonly Dictionary<int, UIOrderProductFromPopupAnimator> _orderProductAnimatorsBySlotIndex = new Dictionary<int, UIOrderProductFromPopupAnimator>();

    public UIBottomPanelWarehouseTabMediator(BottomPanelView view)
        : base(view)
    {
        _dispatcher = Dispatcher.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _gameStateModel = GameStateModel.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _warehouseModel = PlayerModel.Instance.ShopModel.WarehouseModel;
        _configManager = GameConfigManager.Instance;
        _loc = LocalizationManager.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();
        _dispatcher.UIRequestOrderProductAnimation += OnUIRequestOrderProductAnimation;
        _updatesProvider.RealtimeSecondUpdate += OnRealtimeSecondUpdate;
    }

    public override void Unmediate()
    {
        _dispatcher.UIRequestOrderProductAnimation -= OnUIRequestOrderProductAnimation;
        _updatesProvider.RealtimeSecondUpdate -= OnRealtimeSecondUpdate;
        base.Unmediate();
    }

    private async void OnUIRequestOrderProductAnimation(RectTransform rectTransform, Vector2 screenPosition, int slotIndex, ProductModel productModel)
    {
        var slotModel = _warehouseModel.Slots[slotIndex];
        var animator = new UIOrderProductFromPopupAnimator(rectTransform, screenPosition, GetViewByViewModel(slotModel), productModel);
        _orderProductAnimatorsBySlotIndex[slotIndex] = animator;
        await animator.AnimateAsync();
        _orderProductAnimatorsBySlotIndex.Remove(slotIndex);
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
        DeactivateItem(viewModel);
        if (_orderProductAnimatorsBySlotIndex.ContainsKey(viewModel.Index))
        {
            _orderProductAnimatorsBySlotIndex[viewModel.Index].CancelAnimation();
        }

        base.BeforeHideItem(itemView, viewModel);
    }

    protected override void ActivateItem(UIBottomPanelScrollItemView itemView)
    {
        itemView.BottomButtonClicked += OnBottomButtonClicked;
        base.ActivateItem(itemView);
    }

    protected override void DeactivateItem(UIBottomPanelScrollItemView itemView)
    {
        itemView.BottomButtonClicked -= OnBottomButtonClicked;
        base.DeactivateItem(itemView);
    }

    private void OnBottomButtonClicked(UIBottomPanelScrollItemView itemView)
    {
        var viewModel = DisplayedItems.First(t => t.View == itemView).ViewModel;
        _dispatcher.UIBottomPanelWarehouseQuickDeliverClicked(viewModel.Index);
    }

    private void ActivateItem(ProductSlotModel slotModel)
    {
        slotModel.ProductIsSet += OnProductSet;
        slotModel.ProductAmountChanged += OnProductAmountChanged;
        slotModel.ProductRemoved += OnProductRemoved;
        slotModel.ProductDeliveryTimeChanged += OnDeliveryTimeChanged;
    }

    private void DeactivateItem(ProductSlotModel slotModel)
    {
        slotModel.ProductIsSet -= OnProductSet;
        slotModel.ProductAmountChanged -= OnProductAmountChanged;
        slotModel.ProductRemoved -= OnProductRemoved;
        slotModel.ProductDeliveryTimeChanged -= OnDeliveryTimeChanged;
    }

    private async void OnProductSet(int slotIndex)
    {
        if (_orderProductAnimatorsBySlotIndex.TryGetValue(slotIndex, out var animator))
        {
            await animator.AnimationTask;
        }

        var slotmodel = _warehouseModel.Slots[slotIndex];
        var view = GetViewByViewModel(slotmodel);
        if (view != null)
        {
            UpdateSlotView(view, slotmodel);
            await view.AnimateJumpAsync();
        }
    }

    private void OnProductRemoved(int slotIndex, ProductModel removedProduct)
    {
        UpdateSlotViewByIndex(slotIndex);
    }

    private void OnProductAmountChanged(int slotIndex, int deltaAmount)
    {
        UpdateSlotViewByIndex(slotIndex);
    }

    private void OnDeliveryTimeChanged(int slotIndex, int deltaTime)
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
        if (view != null)
        {
            UpdateSlotView(view, slotmodel);
        }
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
            itemView.DisableHint();

            var deltaDeliver = product.DeliverTime - _gameStateModel.ServerTime;
            if (deltaDeliver > 0)
            {
                itemView.SetImageAlpha(0.3f);
                itemView.SetTopText(FormattingHelper.ToSeparatedTimeFormat(deltaDeliver));
                var quickDeliverPrice = CalculationHelper.GetPriceForDeliver(_configManager.MainConfig.QuickDeliverPriceGoldPerMinute, deltaDeliver);
                itemView.SetBottomButtonPrice(quickDeliverPrice);
                itemView.SetupBottomButtonHint(_loc.GetLocalization(LocalizationKeys.BottomPanelWarehouseQuickDeliveryHint));
            }
            else
            {
                itemView.DisableBottomButton();
                itemView.SetImageAlpha(1);
                UpdateProductAmount(itemView, product);
            }
        }
        else
        {
            itemView.Reset();
            itemView.SetupIconSize(110);
            itemView.SetImage(_spritesProvider.GetOrderIcon());
            itemView.SetTopText(_loc.GetLocalization(LocalizationKeys.BottomPanelWarehouseEmptySlot));
            itemView.SetupMainHint(_loc.GetLocalization(LocalizationKeys.BottomPanelWarehouseEmptySlotHint));
        }
    }

    private void OnRealtimeSecondUpdate()
    {
        foreach (var displayedItem in DisplayedItems)
        {
            if (_orderProductAnimatorsBySlotIndex.ContainsKey(displayedItem.ViewModel.Index)) continue;
            if (displayedItem.ViewModel.HasProduct
                && (displayedItem.ViewModel.Product.DeliverTime - _gameStateModel.ServerTime) >= 0)
            {
                UpdateSlotView(displayedItem.View, displayedItem.ViewModel);
            }
        }
    }
}