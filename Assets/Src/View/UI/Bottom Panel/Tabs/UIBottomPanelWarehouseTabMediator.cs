using System.Collections.Generic;
using System.Linq;
using Src.Common;
using Src.Managers;
using Src.Model;
using Src.View.UI.Tutorial;
using UnityEngine;

namespace Src.View.UI.Bottom_Panel.Tabs
{
    public class UIBottomPanelWarehouseTabMediator : UIBottomPanelCommonScrollItemsTabMediatorBase<BottomPanelWarehouseItemViewModel>
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
            _tutorialUIElementsProvider.ClearElement(TutorialUIElement.BottomPanelWarehouseTabLastDeliveringSlot);
            _tutorialUIElementsProvider.ClearElement(TutorialUIElement.BottomPanelWarehouseTabLastDeliveringSlotTime);
            _tutorialUIElementsProvider.ClearElement(TutorialUIElement.BottomPanelWarehouseTabFirstFreeSlot);

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
                UpdateItemView(displayedItem.View, displayedItem.ViewModel);
            }
        }

        private void OnSlotsAdded(int addedSlotsCount)
        {
            RefreshScrollContent();
        }

        private async void OnUIRequestOrderProductAnimation(RectTransform rectTransform, Vector2 screenPosition, int slotIndex, ProductModel productModel)
        {
            var (view, _) = GetDisplayedItemBySlotIndex(slotIndex);
            var animator = new UIOrderProductFromPopupAnimator(rectTransform, screenPosition, view, productModel);
            _orderProductAnimatorsBySlotIndex[slotIndex] = animator;
            await animator.AnimateAsync();
            _orderProductAnimatorsBySlotIndex.Remove(slotIndex);
        }

        protected override IEnumerable<BottomPanelWarehouseItemViewModel> GetViewModelsToShow()
        {
            return _warehouseModel.Slots
                .Select(s => new BottomPanelWarehouseItemViewModel(s))
                .Concat(new BottomPanelWarehouseItemViewModel[] { new BottomPanelWarehouseItemViewModel() });
        }

        protected override void HandleClick(BottomPanelWarehouseItemViewModel viewModel)
        {
            if (viewModel.HasSlot)
            {
                _dispatcher.UIBottomPanelWarehouseSlotClicked(viewModel.SlotModel.Index);
            }
            else
            {
                _dispatcher.UIBottomPanelExpandWarehouseClicked();
            }
        }

        protected override void SetupItem(UIBottomPanelScrollItemView itemView, BottomPanelWarehouseItemViewModel viewModel)
        {
            UpdateItemView(itemView, viewModel);
        }

        protected override void BeforeHideItem(UIBottomPanelScrollItemView itemView, BottomPanelWarehouseItemViewModel viewModel)
        {
            if (viewModel.HasSlot)
            {
                if (_orderProductAnimatorsBySlotIndex.ContainsKey(viewModel.SlotModel.Index))
                {
                    _orderProductAnimatorsBySlotIndex[viewModel.SlotModel.Index].CancelAnimation();
                }
            }

            base.BeforeHideItem(itemView, viewModel);
        }

        protected override void ActivateItem(UIBottomPanelScrollItemView itemView, BottomPanelWarehouseItemViewModel viewModel)
        {
            base.ActivateItem(itemView, viewModel);

            itemView.BottomButtonClicked += OnBottomButtonClicked;
            itemView.RemoveButtonClicked += OnRemoveButtonClicked;
            if (viewModel.HasSlot)
            {
                var slotModel = viewModel.SlotModel;
                slotModel.ProductIsSet += OnProductSet;
                slotModel.ProductAmountChanged += OnProductAmountChanged;
                slotModel.ProductRemoved += OnProductRemoved;
                slotModel.ProductDeliveryTimeChanged += OnDeliveryTimeChanged;
            }
        }

        protected override void DeactivateItem(UIBottomPanelScrollItemView itemView, BottomPanelWarehouseItemViewModel viewModel)
        {
            itemView.BottomButtonClicked -= OnBottomButtonClicked;
            itemView.RemoveButtonClicked -= OnRemoveButtonClicked;
            if (viewModel.HasSlot)
            {
                var slotModel = viewModel.SlotModel;
                slotModel.ProductIsSet -= OnProductSet;
                slotModel.ProductAmountChanged -= OnProductAmountChanged;
                slotModel.ProductRemoved -= OnProductRemoved;
                slotModel.ProductDeliveryTimeChanged -= OnDeliveryTimeChanged;
            }

            base.DeactivateItem(itemView, viewModel);
        }

        private void OnRemoveButtonClicked(UIBottomPanelScrollItemView itemView)
        {
            var viewModel = DisplayedItems.First(t => t.View == itemView).ViewModel;
            if (viewModel.HasSlot)
            {
                _dispatcher.UIBottomPanelWarehouseRemoveProductClicked(viewModel.SlotModel.Index);
            }
        }

        private void OnBottomButtonClicked(UIBottomPanelScrollItemView itemView)
        {
            var viewModel = DisplayedItems.First(t => t.View == itemView).ViewModel;
            if (viewModel.HasSlot)
            {
                _dispatcher.UIBottomPanelWarehouseQuickDeliverClicked(viewModel.SlotModel.Index);
            }
        }

        private async void OnProductSet(int slotIndex)
        {
            if (_orderProductAnimatorsBySlotIndex.TryGetValue(slotIndex, out var animator))
            {
                await animator.AnimationTask;
            }

            var (view, viewModel) = GetDisplayedItemBySlotIndex(slotIndex);
            if (view != null)
            {
                UpdateItemView(view, viewModel);
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
            var (view, viewModel) = GetDisplayedItemBySlotIndex(slotIndex);
            if (view != null)
            {
                UpdateItemView(view, viewModel);
            }
        }

        private (UIBottomPanelScrollItemView View, BottomPanelWarehouseItemViewModel ViewModel) GetDisplayedItemBySlotIndex(int slotIndex)
        {
            return DisplayedItems.FirstOrDefault(i => i.ViewModel.HasSlot && i.ViewModel.SlotModel.Index == slotIndex);
        }

        private void UpdateItemView(UIBottomPanelScrollItemView itemView, BottomPanelWarehouseItemViewModel itemViewModel)
        {
            if (itemViewModel.HasSlot)
            {
                var slotModel = itemViewModel.SlotModel;
                if (slotModel.HasProduct)
                {
                    var product = slotModel.Product;
                    var config = slotModel.Product.Config;
                    var icon = _spritesProvider.GetProductIcon(config.Key);

                    itemView.SetupIconSize(110);
                    itemView.SetImage(icon);
                    itemView.DisableHint();
                    itemView.SetTextGray();

                    var deltaDeliver = product.DeliverTime - _gameStateModel.ServerTime;
                    if (deltaDeliver > 0)
                    {
                        itemView.SetImageAlpha(0.3f);
                        itemView.SetTopText(FormattingHelper.ToSeparatedTimeFormat(deltaDeliver));
                        var quickDeliverPrice = CalculationHelper.GetPriceForDeliver(_configManager.MainConfig.QuickDeliverPriceGoldPerHour, deltaDeliver);
                        itemView.SetBottomButtonPrice(quickDeliverPrice);
                        itemView.SetupBottomButtonHint(_loc.GetLocalization(LocalizationKeys.BottomPanelWarehouseQuickDeliveryHint));

                        _tutorialUIElementsProvider.SetElement(TutorialUIElement.BottomPanelWarehouseTabLastDeliveringSlot, itemView.transform as RectTransform);
                        _tutorialUIElementsProvider.SetElement(TutorialUIElement.BottomPanelWarehouseTabLastDeliveringSlotTime, itemView.TopTextRectTransform);
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
            else
            {
                itemView.Reset();
                itemView.SetupIconSize(110);
                itemView.SetImage(_spritesProvider.GetBigPlusSignIcon());
                itemView.SetSkinGreen();
                //itemView.SetTopText(_loc.GetLocalization(LocalizationKeys.BottomPanelWarehouseEmptySlot));
                itemView.SetupMainHint(_loc.GetLocalization(LocalizationKeys.BottomPanelWarehouseExpandHint));
            }
        }

        private void OnRealtimeSecondUpdate()
        {
            foreach (var displayedItem in DisplayedItems)
            {
                var slotModel = displayedItem.ViewModel.SlotModel;
                if (slotModel == null || _orderProductAnimatorsBySlotIndex.ContainsKey(slotModel.Index)) continue;
                if (slotModel.HasProduct)
                {
                    var restDeliverTime = slotModel.Product.DeliverTime - _gameStateModel.ServerTime;
                    if (restDeliverTime >= -1)
                    {
                        UpdateItemView(displayedItem.View, displayedItem.ViewModel);
                        if (restDeliverTime == 0)
                        {
                            _audioManager.PlaySound(SoundNames.Delivered);
                        }
                    }
                }
            }
        }
    }

    public class BottomPanelWarehouseItemViewModel
    {
        public readonly ProductSlotModel SlotModel;

        public BottomPanelWarehouseItemViewModel(ProductSlotModel slotModel = null)
        {
            SlotModel = slotModel;
        }

        public bool HasSlot => SlotModel != null;
    }
}