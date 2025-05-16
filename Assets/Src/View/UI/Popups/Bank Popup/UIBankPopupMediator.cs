using System.Collections.Generic;
using Src.Common;
using Src.Managers;
using Src.Model;
using Src.Model.Popups;
using UnityEngine;

namespace Src.View.UI.Popups.Bank_Popup
{
    public class UIBankPopupMediator : UIContentPopupMediator
    {
        private readonly RectTransform _parentTransform;
        private readonly PrefabsHolder _prefabsHolder;
        private readonly LocalizationManager _loc;
        private readonly SpritesProvider _spritesProvider;
        private readonly Dispatcher _dispatcher;
        private readonly GameStateModel _gameStateModel;
        private readonly UpdatesProvider _updatesProvider;
        private readonly Dictionary<UIBankPopupItemViewBase, IBankItemViewModel> _modelByViewDict = new Dictionary<UIBankPopupItemViewBase, IBankItemViewModel>();

        private UITabbedContentPopupView _popupView;
        private int _currentTabIndex;
        private IEnumerable<IBankItemViewModel> _viewModelsToDisplay;
        private BankPopupViewModel _viewModel;

        public UIBankPopupMediator(RectTransform parentTransform)
        {
            _parentTransform = parentTransform;

            _gameStateModel = GameStateModel.Instance;
            _prefabsHolder = PrefabsHolder.Instance;
            _loc = LocalizationManager.Instance;
            _spritesProvider = SpritesProvider.Instance;
            _dispatcher = Dispatcher.Instance;
            _updatesProvider = UpdatesProvider.Instance;
        }

        protected override UIContentPopupView PopupView => _popupView;

        public override async void Mediate()
        {
            _viewModel = _gameStateModel.ShowingPopupModel as BankPopupViewModel;
            TopPadding = 5;
            var popupGo = GameObject.Instantiate(_prefabsHolder.UITabbedContentPopupPrefab, _parentTransform);
            _popupView = popupGo.GetComponent<UITabbedContentPopupView>();
            _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupBankTitle));
            _popupView.SetSize(1050, 835);
            _popupView.SetupTabButtons(new string[] { _loc.GetLocalization(LocalizationKeys.PopupBankGoldTab), _loc.GetLocalization(LocalizationKeys.PopupBankCashTab) });

            ShowTab(_viewModel.InitialTabIndex);
            Activate();

            await _popupView.Appear2Async();
        }

        public override async void Unmediate()
        {
            base.Unmediate();
            Deactivate();

            await _popupView.Disppear2Async();

            ClearDisplayedItems();
            GameObject.Destroy(_popupView.gameObject);
        }

        protected override void ClearDisplayedItems()
        {
            foreach (var kvp in _modelByViewDict)
            {
                DeactivateItemView(kvp.Key);
            }
            _modelByViewDict.Clear();

            base.ClearDisplayedItems();
        }

        private void Activate()
        {
            _popupView.TabButtonClicked += OnTabButtonClicked;
            _popupView.ButtonCloseClicked += OnCloseClicked;
            _viewModel.ItemsUpdated += OnItemsUpdated;
            _viewModel.RestWatchesCountChanged += OnRestWatchesCountChanged;
            _viewModel.AdvertWatchTimeChanged += OnAdvertWatchTimeChanged;
            _updatesProvider.RealtimeSecondUpdate += OnRealtimeSecondUpdate;
        }

        private void Deactivate()
        {
            _popupView.TabButtonClicked -= OnTabButtonClicked;
            _popupView.ButtonCloseClicked -= OnCloseClicked;
            _viewModel.ItemsUpdated -= OnItemsUpdated;
            _viewModel.RestWatchesCountChanged -= OnRestWatchesCountChanged;
            _viewModel.AdvertWatchTimeChanged -= OnAdvertWatchTimeChanged;
            _updatesProvider.RealtimeSecondUpdate -= OnRealtimeSecondUpdate;
        }

        private void OnRealtimeSecondUpdate()
        {
            UpdateAdvertItems();
        }

        private void OnAdvertWatchTimeChanged()
        {
            UpdateAdvertItems();
        }

        private void OnRestWatchesCountChanged()
        {
            UpdateAdvertItems();
        }

        private void UpdateAdvertItems()
        {
            foreach (var kvp in _modelByViewDict)
            {
                if (kvp.Value.RetrieveMethodType == BankItemRetrieveMethodType.Advert)
                {
                    UpdateItemView(kvp.Key, kvp.Value);
                }
            }
        }

        private void OnItemsUpdated()
        {
            ShowTab(_currentTabIndex);
        }

        private void OnCloseClicked()
        {
            _dispatcher.UIRequestRemoveCurrentPopup();
        }

        private void OnTabButtonClicked(int tabIndex)
        {
            ShowTab(tabIndex);
        }

        private void ShowTab(int tabIndex)
        {
            _popupView.SetTabButtonSelected(tabIndex);
            ClearDisplayedItems();

            _currentTabIndex = tabIndex;
            _viewModelsToDisplay = _currentTabIndex == 0 ? _viewModel.GoldItems : _viewModel.CashItems;
            foreach (var viewModel in _viewModelsToDisplay)
            {
                PutItem(viewModel);
            }
        }

        private void PutItem(IBankItemViewModel viewModel)
        {
            var isAdvertItem = viewModel.RetrieveMethodType == BankItemRetrieveMethodType.Advert;
            var rectTransform = GetOrCreateItemToDisplay(isAdvertItem ? _prefabsHolder.UIBankPopupAdsItemPrefab : _prefabsHolder.UIBankPopupItemPrefab);
            var itemView = rectTransform.GetComponent<UIBankPopupItemViewBase>();
            UpdateItemView(itemView, viewModel);
            ActivateItemView(itemView);
            _modelByViewDict[itemView] = viewModel;
        }

        private void UpdateItemView(UIBankPopupItemViewBase itemView, IBankItemViewModel viewModel)
        {
            if (viewModel.RetrieveMethodType == BankItemRetrieveMethodType.Advert)
            {
                UpdateAdvertItem(itemView as UIBankPopupAdvertItemView, viewModel as BankAdvertItemViewModel);
            }
            else if (viewModel.RetrieveMethodType == BankItemRetrieveMethodType.RealBuy)
            {
                UpdateBuyableItem(itemView as UIBankPopupBuyableItemView, viewModel as BankBuyableItemViewModel);
            }
        }

        private void UpdateAdvertItem(UIBankPopupAdvertItemView itemView, BankAdvertItemViewModel bankAdvertItemViewModel)
        {
            UpdateBaseItem(itemView, bankAdvertItemViewModel);
            var restSingleWatchCooldown =
                Mathf.Max(0, bankAdvertItemViewModel.EndSingleWatchCooldownTime - _gameStateModel.ServerTime);
            
            var canWatch = bankAdvertItemViewModel.RestWatchesCount > 0 && restSingleWatchCooldown <= 0;
            
            itemView.SetAvailable(canWatch);
            if (canWatch)
            {
                itemView.SetPriceText(_loc.GetLocalization(LocalizationKeys.PopupBankAdsItemText));
                itemView.NotificationCounter.SetCounterText(bankAdvertItemViewModel.RestWatchesCount.ToString());
            }
            else
            {
                var cooldownValue = 0;
                if (bankAdvertItemViewModel.RestWatchesCount <= 0)
                {
                    cooldownValue = bankAdvertItemViewModel.ResetWatchesCountTime - _gameStateModel.ServerTime;
                }
                else
                {
                    cooldownValue = restSingleWatchCooldown;
                }

                var timeRestStr = FormattingHelper.ToSeparatedTimeFormat(Mathf.Max(0, cooldownValue));
                var priceText = string.Format(_loc.GetLocalization(LocalizationKeys.PopupBankAdvertAvailableAfterFormat), timeRestStr);
                itemView.SetPriceText(priceText);
            }
        }

        private void UpdateBuyableItem(UIBankPopupBuyableItemView itemView, BankBuyableItemViewModel viewModel)
        {
            UpdateBaseItem(itemView, viewModel);
            itemView.SetAvailable(viewModel.IsAvailable);
            var currencyName = GetCurrencyName(viewModel);
            itemView.SetPriceText($"{FormattingHelper.ToCommaSeparatedNumber(viewModel.Price)} {currencyName}");
            itemView.SetExtraPercentText(string.Empty);

            if (viewModel.ExtraPercent > 0)
            {
                itemView.SetExtraPercentText($"(+{viewModel.ExtraPercent}%)");
            }
        }

        private string GetCurrencyName(BankBuyableItemViewModel viewModel)
        {
            return viewModel.HaveLocalizedCurrencyName
                ? viewModel.LocalizedCurrencyName
                : $"{_loc.GetLocalization($"{LocalizationKeys.CommonPayCurrencyNamePlural}{PluralsHelper.GetPlural(viewModel.Price)}")}";
        }

        private void UpdateBaseItem(UIBankPopupItemViewBase itemView, IBankItemViewModel viewModel)
        {
            ColorUtility.TryParseHtmlString(
                viewModel.IsGold ? Constants.GoldAmountTextRedColor : Constants.CashAmountTextGreenColor, out var color);
        
            itemView.SetIconSprite(viewModel.IsGold ? _spritesProvider.GetGoldIcon() : _spritesProvider.GetCashIcon());
            itemView.SetAmountTextColor(color);
            itemView.SetAmountText(FormattingHelper.ToCommaSeparatedNumber(viewModel.Value));
        }

        private void ActivateItemView(UIBankPopupItemViewBase view)
        {
            view.Clicked += OnViewClicked;
        }

        private void DeactivateItemView(UIBankPopupItemViewBase view)
        {
            view.Clicked -= OnViewClicked;
        }

        private void OnViewClicked(UIBankPopupItemViewBase view)
        {
            var viewModel = _modelByViewDict[view];
            if (viewModel.RetrieveMethodType == BankItemRetrieveMethodType.Advert)
            {
                _dispatcher.UIBankAdsItemClicked(viewModel.IsGold);
            }
            else
            {
                _dispatcher.UIBankItemClicked((viewModel as BankBuyableItemViewModel).GetBankConfigItem());
            }
        }
    }
}
