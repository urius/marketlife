using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBottomPanelWarehouseTabMediator : UIBottomPanelScrollItemsTabMediatorBase<ProductSlotModel>
{
    private readonly ShopWarehouseModel _warehouseModel;
    private readonly GameConfigManager _configManager;
    private readonly UpdatesProvider _updatesProvider;
    private readonly LocalizationManager _loc;
    private readonly AudioManager _audioManager;
    private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;
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
        _warehouseModel = PlayerModelHolder.Instance.ShopModel.WarehouseModel;
        _configManager = GameConfigManager.Instance;
        _loc = LocalizationManager.Instance;
        _audioManager = AudioManager.Instance;
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();
        Activate();
        View.SetButtonSelected(View.WarehouseButton);
    }

    public override void Unmediate()
    {
        View.SetButtonUnselected(View.WarehouseButton);
        Deactivate();
        base.Unmediate();
    }

    private void Activate()
    {
        _dispatcher.UIRequestOrderProductAnimation += OnUIRequestOrderProductAnimation;
        _updatesProvider.RealtimeSecondUpdate += OnRealtimeSecondUpdate;
        _warehouseModel.SlotsAdded += OnSlotsAdded;
        _warehouseModel.VolumeAdded += OnVolumeAdded;
    }

    private void Deactivate()
    {
        _dispatcher.UIRequestOrderProductAnimation -= OnUIRequestOrderProductAnimation;
        _updatesProvider.RealtimeSecondUpdate -= OnRealtimeSecondUpdate;
        _warehouseModel.SlotsAdded -= OnSlotsAdded;
        _warehouseModel.VolumeAdded -= OnVolumeAdded;
    }

    private void OnVolumeAdded(int addedVolume)
    {
        foreach (var displayedItem in DisplayedItems)
        {
            UpdateSlotView(displayedItem.View, displayedItem.ViewModel);
        }
    }

    private void OnSlotsAdded(int addedSlotsCount)
    {
        RefreshScrollContent();
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
        UpdateSlotView(itemView, slotModel);
    }

    protected override void BeforeHideItem(UIBottomPanelScrollItemView itemView, ProductSlotModel viewModel)
    {
        if (_orderProductAnimatorsBySlotIndex.ContainsKey(viewModel.Index))
        {
            _orderProductAnimatorsBySlotIndex[viewModel.Index].CancelAnimation();
        }

        base.BeforeHideItem(itemView, viewModel);
    }

    protected override void ActivateItem(UIBottomPanelScrollItemView itemView, ProductSlotModel slotModel)
    {
        base.ActivateItem(itemView, slotModel);

        itemView.BottomButtonClicked += OnBottomButtonClicked;
        itemView.RemoveButtonClicked += OnRemoveButtonClicked;
        slotModel.ProductIsSet += OnProductSet;
        slotModel.ProductAmountChanged += OnProductAmountChanged;
        slotModel.ProductRemoved += OnProductRemoved;
        slotModel.ProductDeliveryTimeChanged += OnDeliveryTimeChanged;
    }

    protected override void DeactivateItem(UIBottomPanelScrollItemView itemView, ProductSlotModel slotModel)
    {
        itemView.BottomButtonClicked -= OnBottomButtonClicked;
        itemView.RemoveButtonClicked -= OnRemoveButtonClicked;
        slotModel.ProductIsSet -= OnProductSet;
        slotModel.ProductAmountChanged -= OnProductAmountChanged;
        slotModel.ProductRemoved -= OnProductRemoved;
        slotModel.ProductDeliveryTimeChanged -= OnDeliveryTimeChanged;

        base.DeactivateItem(itemView, slotModel);
    }

    private void OnRemoveButtonClicked(UIBottomPanelScrollItemView itemView)
    {
        var viewModel = DisplayedItems.First(t => t.View == itemView).ViewModel;
        _dispatcher.UIBottomPanelWarehouseRemoveProductClicked(viewModel.Index);
    }

    private void OnBottomButtonClicked(UIBottomPanelScrollItemView itemView)
    {
        var viewModel = DisplayedItems.First(t => t.View == itemView).ViewModel;
        _dispatcher.UIBottomPanelWarehouseQuickDeliverClicked(viewModel.Index);
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
            _audioManager.PlaySound(SoundNames.ProductDrop1);
            LeanTween.delayedCall(0.5f, PlayProductDropSound);
            //LeanTween.delayedCall(0.7f, PlayProductDropSound);
            await view.AnimateJumpAsync();
        }
    }

    private void PlayProductDropSound()
    {
        _audioManager.PlaySound(SoundNames.ProductDrop2);
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
        var color = Color.Lerp(Color.red, Color.green, fullness);
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
                itemView.ShowRemoveButton();
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

            if (_tutorialUIElementsProvider.HasElement(TutorialUIElement.BottomPanelWarehouseTabFirstFreeSlot) == false
                && slotModel.HasProduct == false)
            {
                _tutorialUIElementsProvider.SetElement(TutorialUIElement.BottomPanelWarehouseTabFirstFreeSlot, itemView.transform as RectTransform);
            }
        }
    }

    private void OnRealtimeSecondUpdate()
    {
        foreach (var displayedItem in DisplayedItems)
        {
            if (_orderProductAnimatorsBySlotIndex.ContainsKey(displayedItem.ViewModel.Index)) continue;
            if (displayedItem.ViewModel.HasProduct)
            {
                var restDeliverTime = displayedItem.ViewModel.Product.DeliverTime - _gameStateModel.ServerTime;
                if (restDeliverTime >= -1)
                {
                    UpdateSlotView(displayedItem.View, displayedItem.ViewModel);
                    if (restDeliverTime == 0)
                    {
                        _audioManager.PlaySound(SoundNames.Delivered);
                    }
                }
            }
        }
    }
}