using System.Linq;
using Src.Common;
using Src.Helpers;
using Src.Managers;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Popups;
using UnityEngine;

namespace Src.View.UI.Popups.DailyBonus_Popup
{
    public class UIDailyBonusPopupMediator : IMediator
    {
        private readonly RectTransform _contentTransform;
        private readonly PrefabsHolder _prefabsHolder;
        private readonly LocalizationManager _loc;
        private readonly SpritesProvider _spritesProvider;
        private readonly Dispatcher _dispatcher;
        private readonly UserModel _playerModel;
        private readonly AdvertViewStateModel _advertWatchStateModel;

        //
        private DailyBonusPopupViewModel _viewModel;
        private UIDailyBonusPopupView _popupView;

        public UIDailyBonusPopupMediator(RectTransform contentTransform)
        {
            _contentTransform = contentTransform;

            _prefabsHolder = PrefabsHolder.Instance;
            _loc = LocalizationManager.Instance;
            _spritesProvider = SpritesProvider.Instance;
            _dispatcher = Dispatcher.Instance;
            _playerModel = PlayerModelHolder.Instance.UserModel;
            _advertWatchStateModel = AdvertViewStateModel.Instance;
        }

        public async void Mediate()
        {
            _viewModel = GameStateModel.Instance.ShowingPopupModel as DailyBonusPopupViewModel;

            var popupGo = GameObject.Instantiate(_prefabsHolder.UIDailyBonusPopupPrefab, _contentTransform);
            _popupView = popupGo.GetComponent<UIDailyBonusPopupView>();

            SetupView();

            await _popupView.Appear2Async();

            Activate();
        }

        public async void Unmediate()
        {
            Deactivate();

            await _popupView.Disppear2Async();

            GameObject.Destroy(_popupView.gameObject);
        }

        private void Activate()
        {
            _popupView.ButtonCloseClicked += OnButtonCloseClicked;
            _popupView.TakeButtonClicked += OnTakeButtonClicked;
            _popupView.TakeButtonX2Clicked += OnTakeX2ButtonClicked;
            _playerModel.BonusStateUpdated += OnBonusStateUpdated;
            _advertWatchStateModel.WatchStateChanged += OnWatchStateChanged;
        }

        private void Deactivate()
        {
            _popupView.ButtonCloseClicked -= OnButtonCloseClicked;
            _popupView.TakeButtonClicked -= OnTakeButtonClicked;
            _popupView.TakeButtonX2Clicked -= OnTakeX2ButtonClicked;
            _playerModel.BonusStateUpdated -= OnBonusStateUpdated;
            _advertWatchStateModel.WatchStateChanged -= OnWatchStateChanged;
        }

        private void OnWatchStateChanged(AdvertTargetType targetType)
        {
            UpdateItemViews();
        }

        private void OnBonusStateUpdated()
        {
            if (DateTimeHelper.IsSameDays(_playerModel.BonusState.LastBonusTakeTimestamp, _viewModel.OpenTimestamp))
            {
                _popupView.SetTakeButtonsInteractable(false);
            }
        }

        private void OnButtonCloseClicked()
        {
            _dispatcher.UIRequestRemoveCurrentPopup();
        }

        private void SetupView()
        {
            _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupDailyBonusTitle));
            UpdateItemViews();
        }

        private void UpdateItemViews()
        {
            var configItems = _viewModel.BonusConfig.DailyBonusConfig;
            for (var i = 0; i < configItems.Length; i++)
            {
                SetupItemView(_popupView.ItemViews[i], configItems[i]);
            }
        }

        private void SetupItemView(UIDailyBonusPopupPrizeItemView itemView, DailyBonusConfig itemConfig)
        {
            var itemNum = itemConfig.DayNum;
            var currentDayItemNum = DailyBonusRewardHelper.GetDayItemIndex(_viewModel.CurrentBonusDay) + 1;
            var multiplier = DailyBonusRewardHelper.Get5DayMultiplier(_viewModel.CurrentBonusDay);
            var dayNum = itemNum + 5 * (multiplier - 1);
            var reward = DailyBonusRewardHelper.GetRewardForDay(dayNum);
            var isGold = reward.IsGold;
            ColorUtility.TryParseHtmlString(
                isGold ? Constants.GoldAmountTextRedColor : Constants.CashAmountTextGreenColor, out var valueTextColor);

            itemView.SetDayText(
                string.Format(_loc.GetLocalization(LocalizationKeys.PopupDailyBonusDayFormat), dayNum, valueTextColor));
            itemView.SetIconSprite(isGold ? _spritesProvider.GetGoldIcon() : _spritesProvider.GetCashIcon());
            itemView.SetValueTextColor(valueTextColor);
            itemView.SetValueText($"+{FormattingHelper.ToCommaSeparatedNumber(reward.Value)}");
            itemView.SetAlpha(itemNum == currentDayItemNum ? 1 : 0.4f);
        }

        private void OnTakeButtonClicked()
        {
            var itemViewsPositions = _popupView.ItemViews.Select(v => v.transform.position).ToArray();
            _dispatcher.UIDailyBonusTakeClicked(itemViewsPositions);
        }

        private void OnTakeX2ButtonClicked()
        {
            var itemViewsPositions = _popupView.ItemViews.Select(v => v.transform.position).ToArray();
            _dispatcher.UIDailyBonusTakeX2Clicked(itemViewsPositions);
        }
    }
}