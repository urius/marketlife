using System.Linq;
using Src.Common;
using Src.Managers;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Popups;
using TMPro;
using UnityEngine;

namespace Src.View.UI.Popups.OfflineReport_Popup
{
    public class UIOfflineReportPopupMediator : UIContentPopupMediator
    {
        private readonly GameStateModel _gameStateModel;
        private readonly RectTransform _parentTransform;
        private readonly PrefabsHolder _prefabsHolder;
        private readonly LocalizationManager _loc;
        private readonly SpritesProvider _spritesProvider;
        private readonly Dispatcher _dispatcher;
        private readonly MainConfig _config;
        private readonly AdvertViewStateModel _advertViewStateModel;
        private readonly FriendsDataHolder _friendsHolder;
        private readonly ColorsHolder _colorsHolder;

        //
        private UIOfflineReportPopupView _popupView;
        private OfflineReportPopupViewModel _viewModel;
        private int _currentTabIndex;
        private bool _shareSuccessReceived = false;

        protected override UIContentPopupView PopupView => _popupView;

        public UIOfflineReportPopupMediator(RectTransform parentTransform)
        {
            _parentTransform = parentTransform;

            _gameStateModel = GameStateModel.Instance;
            _prefabsHolder = PrefabsHolder.Instance;
            _loc = LocalizationManager.Instance;
            _spritesProvider = SpritesProvider.Instance;
            _dispatcher = Dispatcher.Instance;
            _config = GameConfigManager.Instance.MainConfig;
            _advertViewStateModel = AdvertViewStateModel.Instance;
            _friendsHolder = FriendsDataHolder.Instance;
            _colorsHolder = ColorsHolder.Instance;
        }

        public override async void Mediate()
        {
            _viewModel = _gameStateModel.ShowingPopupModel as OfflineReportPopupViewModel;
            var popupGo = GameObject.Instantiate(_prefabsHolder.UIOfflineReportPopupPrefab, _parentTransform);
            _popupView = popupGo.GetComponent<UIOfflineReportPopupView>();

            var titleTimePassedStr = _viewModel.ReportModel.CalculationHours >= 1
                ? string.Format(_loc.GetLocalization(LocalizationKeys.CommonHoursShortFormat), (int)_viewModel.ReportModel.CalculationHours)
                : string.Format(_loc.GetLocalization(LocalizationKeys.CommonMinutesShortFormat), _viewModel.ReportModel.CalculationMinutes);
            _popupView.SetTitleText(string.Format(_loc.GetLocalization(LocalizationKeys.PopupOfflineReportTitleFormat), titleTimePassedStr));
            _popupView.SetupTabButtons(_viewModel.Tabs.Select(ToTabName).ToArray());
            _popupView.SetShareButtonText(_loc.GetLocalization(LocalizationKeys.CommonShare));
            _popupView.SetShareRevenueButtonText($"+{_config.ShareOfflineReportRewardGold}");
            _popupView.SetAdsButtonVisibility(_viewModel.ProfitFromSell > 0);
            ShowTab(0);

            await _popupView.Appear2Async();

            Activate();
        }

        public override async void Unmediate()
        {
            base.Unmediate();

            Deactivate();

            await _popupView.Disppear2Async();

            GameObject.Destroy(_popupView.gameObject);
        }

        private void Activate()
        {
            _dispatcher.UIShareSuccessCallback += OnUIShareSuccessCallback;
            _popupView.ButtonCloseClicked += OnCloseClicked;
            _popupView.TabButtonClicked += OnTabButtonClicked;
            _popupView.ShareClicked += OnShareClicked;
            _popupView.AdsClicked += OnAdsClicked;
            _advertViewStateModel.WatchStateChanged += OnWatchStateChanged;
        }

        private void Deactivate()
        {
            _dispatcher.UIShareSuccessCallback -= OnUIShareSuccessCallback;
            _popupView.ButtonCloseClicked -= OnCloseClicked;
            _popupView.TabButtonClicked -= OnTabButtonClicked;
            _popupView.ShareClicked -= OnShareClicked;
            _popupView.AdsClicked -= OnAdsClicked;
            _advertViewStateModel.WatchStateChanged -= OnWatchStateChanged;
        }

        private void OnWatchStateChanged(AdvertTargetType advertTarget)
        {
            RefreshTab();
        }

        private void OnUIShareSuccessCallback()
        {
            _shareSuccessReceived = true;
            _popupView.SetShareButtonInteractable(false);
        }

        private void OnShareClicked()
        {
            _dispatcher.UIOfflineReportShareClicked(_popupView.ShareButtonTransform.position);
        }

        private void OnAdsClicked()
        {
            _dispatcher.UIOfflineReportPopupViewAdsClicked();
        }

        private void OnTabButtonClicked(int tabIndex)
        {
            ShowTab(tabIndex);
        }

        private void OnCloseClicked()
        {
            _dispatcher.UIRequestRemoveCurrentPopup();
        }

        private void ShowTab(int tabIndex)
        {
            _currentTabIndex = tabIndex;
            ClearDisplayedItems();

            _popupView.SetTabButtonSelected(tabIndex);
            switch (_viewModel.Tabs[tabIndex])
            {
                case OfflineReportTabType.SellProfit:
                    ShowProfitTab();
                    break;
                case OfflineReportTabType.Personal:
                    ShowPersonalTab();
                    break;
                case OfflineReportTabType.Guests:
                    ShowActivityTab();
                    break;
            }

            UpdateAdsButton();
        }

        private void UpdateAdsButton()
        {
            _popupView.SetAdsButtonInteractable(
                _advertViewStateModel.IsWatched(AdvertTargetType.OfflineProfitX2) == false
                || _advertViewStateModel.IsWatched(AdvertTargetType.OfflineExpX2) == false);

            if (_advertViewStateModel.IsWatched(AdvertTargetType.OfflineProfitX2) == false)
            {
                _popupView.SetAdsButtonText(_loc.GetLocalization(LocalizationKeys.PopupOfflineReportX2Profit));
            }
            else if (_advertViewStateModel.IsWatched(AdvertTargetType.OfflineExpX2) == false)
            {
                _popupView.SetAdsButtonText(_loc.GetLocalization(LocalizationKeys.PopupOfflineReportX2Exp));
                _popupView.SetAdsButtonSkinSprite(_spritesProvider.GetOrangeButtonSprite());
            }
            else
            {
                _popupView.SetAdsButtonVisibility(false);
                _popupView.SetShareButtonVisiblity(true);
                _popupView.SetShareButtonInteractable(_shareSuccessReceived == false);
            }
        }

        private void RefreshTab()
        {
            ShowTab(_currentTabIndex);
        }

        private void ShowProfitTab()
        {
            foreach (var itemViewMdoel in _viewModel.SoldProducts)
            {
                PutNextSellItem(_spritesProvider.GetProductIcon(itemViewMdoel.ProductKey), $"x{FormattingHelper.ToCommaSeparatedNumber(itemViewMdoel.Amount)}", $"+{FormattingHelper.ToCommaSeparatedNumber(itemViewMdoel.Profit)}$", $"+{FormattingHelper.ToCommaSeparatedNumber(itemViewMdoel.ExpToAdd)}");
            }
            PutNextSpace();

            var bonusCash = 0;
            var bonusExp = 0;
            var isWatchedX2ProfitAds = _advertViewStateModel.IsWatched(AdvertTargetType.OfflineProfitX2);
            var isWatchedX2ExpAds = _advertViewStateModel.IsWatched(AdvertTargetType.OfflineExpX2);
            if (isWatchedX2ProfitAds || isWatchedX2ExpAds)
            {
                bonusCash = isWatchedX2ProfitAds ? _viewModel.ProfitFromSell : 0;
                bonusExp = isWatchedX2ExpAds ? _viewModel.ReportModel.ExpFromSell : 0;
                PutNextOverallItem(_loc.GetLocalization(LocalizationKeys.PopupOfflineReportProfitTabBonus), $"+{FormattingHelper.ToCommaSeparatedNumber(bonusCash)}$", $"+{FormattingHelper.ToCommaSeparatedNumber(bonusExp)}");
            }

            PutNextOverallItem(_loc.GetLocalization(LocalizationKeys.PopupOfflineReportProfitTabOverallProfit), $"{FormattingHelper.ToCommaSeparatedNumber(bonusCash + _viewModel.ProfitFromSell)}$", $"{FormattingHelper.ToCommaSeparatedNumber(_viewModel.ReportModel.ExpFromSell + bonusExp)}");
        }

        private void ShowPersonalTab()
        {
            if (_viewModel.MerchandiserResult.Length > 0)
            {
                PutNextCaption(_loc.GetLocalization($"{LocalizationKeys.CommonPersonalNamePrefix}{Constants.PersonalMerchandiserStr}"));
                foreach (var item in _viewModel.MerchandiserResult)
                {
                    PutNextReportItem(_spritesProvider.GetProductIcon(item.ProductKey), $"x{FormattingHelper.ToCommaSeparatedNumber(item.Amount)}");
                }
            }

            if (_viewModel.UnwashesCleanedAmount > 0)
            {
                PutNextCaption(_loc.GetLocalization($"{LocalizationKeys.CommonPersonalNamePrefix}{Constants.PersonalCleanerStr}"));
                PutNextReportItem(_spritesProvider.GetRandomUnwashIcon(), $"x{FormattingHelper.ToCommaSeparatedNumber(_viewModel.UnwashesCleanedAmount)}");
            }
        }

        private void ShowActivityTab()
        {
            foreach (var guestOfflineActionsModel in _viewModel.ReportModel.GuestOfflineActionModels)
            {
                var name = _loc.GetLocalization(LocalizationKeys.CommonUnknownName);
                var friendData = _friendsHolder.GetFriendData(guestOfflineActionsModel.UserId);
                if (friendData != null)
                {
                    name = friendData.FullName;
                }

                PutNextCaption($"{name}:");

                foreach (var addedProduct in guestOfflineActionsModel.AddedProducts)
                {
                    PutNextReportGuestTabPositiveActionItem(_spritesProvider.GetProductIcon(addedProduct.Config.Key), $"+{FormattingHelper.ToCommaSeparatedNumber(addedProduct.Amount)}", _loc.GetLocalization(LocalizationKeys.PopupOfflineReportGift));
                }

                foreach (var takenProduct in guestOfflineActionsModel.TakenProducts)
                {
                    PutNextReportGuestTabNegativeActionItem(_spritesProvider.GetProductIcon(takenProduct.Config.Key), $"-{FormattingHelper.ToCommaSeparatedNumber(takenProduct.Amount)}", _loc.GetLocalization(LocalizationKeys.PopupOfflineReportGrabbed));
                }

                if (guestOfflineActionsModel.AddedUnwashes > 0)
                {
                    PutNextReportGuestTabNegativeActionItem(_spritesProvider.GetRandomUnwashIcon(), $"+{FormattingHelper.ToCommaSeparatedNumber(guestOfflineActionsModel.AddedUnwashes)}", _loc.GetLocalization(LocalizationKeys.PopupOfflineReportAddedUnwash));
                }
            }
        }

        private void PutNextOverallItem(string lextText, string cashText, string expText)
        {
            var itemTransform = GetOrCreateItemToDisplay(_prefabsHolder.UIOfflineReportPopupOverallItemPrefab);
            var itemView = itemTransform.GetComponent<UIOfflineReportPopupOverallItemView>();
            itemView.SetLeftText(lextText);
            itemView.SetCashText(cashText);
            itemView.SetExpText(expText);
        }

        private void PutNextReportItem(Sprite iconSprite, string lextText, Color leftTextColor, TextAlignmentOptions leftTextAlignment, string rightText, Color rightTextColor, TextAlignmentOptions rightTextAlignment)
        {
            var itemTransform = GetOrCreateItemToDisplay(_prefabsHolder.UIOfflineReportPopupItemPrefab);
            var itemView = itemTransform.GetComponent<UIOfflineReportPopupReportItemView>();
            itemView.SetImageSprite(iconSprite);
            itemView.SetLeftText(lextText, leftTextAlignment);
            itemView.SetLeftTextColor(leftTextColor);
            itemView.SetRightText(rightText, rightTextAlignment);
            itemView.SetRightTextColor(rightTextColor);
        }

        private void PutNextSellItem(Sprite iconSprite, string amountText, string cashText, string expText)
        {
            var itemTransform = GetOrCreateItemToDisplay(_prefabsHolder.UIOfflineReportPopupSellItemPrefab);
            var itemView = itemTransform.GetComponent<UIOfflineReportPopupSellItemView>();
            itemView.SetImageSprite(iconSprite);
            itemView.SetAmountText(amountText);
            itemView.SetCashText(cashText);
            itemView.SetExpText(expText);
        }

        private void PutNextReportItem(Sprite iconSprite, string lextText, string rightText = "")
        {
            PutNextReportItem(iconSprite, lextText, Color.white, TextAlignmentOptions.Left, rightText, _colorsHolder.ReportPopupItemRightTextProfitColor, TextAlignmentOptions.Right);
        }

        private void PutNextReportGuestTabPositiveActionItem(Sprite iconSprite, string lextText, string rightText)
        {
            PutNextReportItem(iconSprite, lextText, Color.green, TextAlignmentOptions.Left, rightText, _colorsHolder.ReportPopupItemPositiveActionColor, TextAlignmentOptions.Right);
        }

        private void PutNextReportGuestTabNegativeActionItem(Sprite iconSprite, string lextText, string rightText)
        {
            PutNextReportItem(iconSprite, lextText, Color.red, TextAlignmentOptions.Left, rightText, _colorsHolder.ReportPopupItemNegativeActionColor, TextAlignmentOptions.Right);
        }

        private void PutNextCaption(string captionText)
        {
            var captionTransform = GetOrCreateItemToDisplay(_prefabsHolder.UIOfflineReportPopupCaptionItemPrefab);
            var captionView = captionTransform.GetComponent<UIPopupCaptionItemView>();
            captionView.SetText(captionText);
        }

        private string ToTabName(OfflineReportTabType tabType)
        {
            string tabKey = "tab key";
            switch (tabType)
            {
                case OfflineReportTabType.SellProfit:
                    tabKey = LocalizationKeys.PopupOfflineReportProfitTab;
                    break;
                case OfflineReportTabType.Personal:
                    tabKey = LocalizationKeys.PopupOfflineReportPersonalTab;
                    break;
                case OfflineReportTabType.Guests:
                    tabKey = LocalizationKeys.PopupOfflineReportGuestsTab;
                    break;
            }

            return _loc.GetLocalization(tabKey);
        }
    }
}
