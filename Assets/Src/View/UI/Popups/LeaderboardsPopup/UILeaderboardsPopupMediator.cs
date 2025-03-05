using System;
using System.Collections.Generic;
using System.Linq;
using Src.Common;
using Src.Model.Popups;
using UnityEngine;

namespace Src.View.UI.Popups.LeaderboardsPopup
{
    public class UILeaderboardsPopupMediator : IMediator
    {
        private readonly RectTransform _contentTransform;
        private readonly GameStateModel _gameStateModel;
        private readonly PrefabsHolder _prefabsHolder;
        private readonly LocalizationManager _loc;
        private readonly UpdatesProvider _updatesProvider;
        private readonly SpritesProvider _spritesProvider;
        private readonly AvatarsManager _avatarsManager;
        private readonly Dispatcher _dispatcher;
        private readonly PlayerModelHolder _playerModelHolder;
        private readonly ColorsHolder _colorsHolder;
        private readonly Dictionary<TabType, float> _contentPositionsByTab = new();

        //
        private UILeaderboardsPopupView _popupView;
        private LeaderboardsPopupViewModel _viewModel;
        private VirtualListDisplayer<UILeaderboardsPopupItemView, LeaderboardUserData> _virtualListDisplayer;
        private TabType _currentTabType;
        private Sprite _currentLBTypeIconSprite;
        private float _lastContentPosition = -1;

        public UILeaderboardsPopupMediator(RectTransform contentTransform)
        {
            _contentTransform = contentTransform;

            _gameStateModel = GameStateModel.Instance;
            _prefabsHolder = PrefabsHolder.Instance;
            _loc = LocalizationManager.Instance;
            _updatesProvider = UpdatesProvider.Instance;
            _spritesProvider = SpritesProvider.Instance;
            _avatarsManager = AvatarsManager.Instance;
            _dispatcher = Dispatcher.Instance;
            _playerModelHolder = PlayerModelHolder.Instance;
            _colorsHolder = ColorsHolder.Instance;
        }

        public async void Mediate()
        {
            _viewModel = _gameStateModel.ShowingPopupModel as LeaderboardsPopupViewModel;
            var popupGo = GameObject.Instantiate(_prefabsHolder.UILeaderboardsPopupPrefab, _contentTransform);
            _popupView = popupGo.GetComponent<UILeaderboardsPopupView>();
            _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupLeaderboardsTitle));
            SetupTabs();

            _virtualListDisplayer = new VirtualListDisplayer<UILeaderboardsPopupItemView, LeaderboardUserData>(
                _popupView.ContentRectTransform,
                _prefabsHolder.UILeaderboardsPopupItemPrefab,
                11,
                ShowItemDelegate,
                HideItemDelegate);

            ShowTab(0);
            Activate();

            await _popupView.Appear2Async();
        }

        private void SetupTabs()
        {
            var tabNames = new[] {
                _loc.GetLocalization(LocalizationKeys.PopupLeaderboardsExpTab),
                _loc.GetLocalization(LocalizationKeys.PopupLeaderboardsCashTab),
                _loc.GetLocalization(LocalizationKeys.PopupLeaderboardsGoldTab),
                _loc.GetLocalization(LocalizationKeys.PopupLeaderboardsFriendsTab),
            };
            _popupView.SetupTabButtons(tabNames);

            for (var i = 0; i < tabNames.Length; i++)
            {
                var tabType = GetTabTypeByTabIndex(i);
                if (DisabledLogicFlags.IsFriendsLogicDisabled && tabType == TabType.Friends)
                {
                    _popupView.SetTabButtonVisibility(i, false);
                    continue;
                }

                var itemsData = GetDataByTab(tabType);
                var existInLB = itemsData.Any(d => d.UserSocialData.Uid == _playerModelHolder.Uid);
                var tabView = _popupView.GetTabButton(i) as UILeaderboardsPopupTabButtonView;
                tabView.SetPlaceIndicatorVisibility(existInLB);
            }
        }

        public async void Unmediate()
        {
            Deactivate();

            await _popupView.Disppear2Async();

            GameObject.Destroy(_popupView.gameObject);
        }

        private void Activate()
        {
            _popupView.TabButtonClicked += OnTabButtonClicked;
            _popupView.ButtonCloseClicked += OnButtonCloseClicked;
            _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
            _avatarsManager.AvatarLoadedForId += OnAvatarLoadedForId;
        }

        private void Deactivate()
        {
            _popupView.TabButtonClicked -= OnTabButtonClicked;
            _popupView.ButtonCloseClicked -= OnButtonCloseClicked;
            _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
            _avatarsManager.AvatarLoadedForId -= OnAvatarLoadedForId;
        }

        private void OnButtonCloseClicked()
        {
            _dispatcher.UIRequestRemoveCurrentPopup();
        }

        private void OnAvatarLoadedForId(string uid)
        {
            var viewModels = _virtualListDisplayer.ViewModels;
            foreach (var displayedItem in _virtualListDisplayer.DisplayedItems)
            {
                if (viewModels[displayedItem.Index].UserSocialData.Uid == uid)
                {
                    var avatarSprite = _avatarsManager.GetAvatarSprite(uid);
                    displayedItem.View.SetAvatar(avatarSprite);
                    break;
                }
            }
        }

        private void OnTabButtonClicked(int tabIndex)
        {
            ShowTab(tabIndex);
        }

        private void ShowTab(int tabIndex)
        {
            _contentPositionsByTab[_currentTabType] = _lastContentPosition;

            _popupView.SetTabButtonSelected(tabIndex);
            _currentTabType = GetTabTypeByTabIndex(tabIndex);
            _currentLBTypeIconSprite = GetLeaderboardIconSprite();
            LeaderboardUserData[] items = GetDataByTab(_currentTabType);
            if (items == null)
            {
                throw new Exception($"{nameof(UILeaderboardsPopupMediator)} unsupported tabIndex: {tabIndex}");
            }
            _virtualListDisplayer.SetupViewModels(items);
            SetContentPosition(GetSavedContentPosition(_currentTabType));
            _virtualListDisplayer.UpdateDisplayedItems();
        }

        private LeaderboardUserData[] GetDataByTab(TabType tabType)
        {
            return tabType switch
            {
                TabType.Exp => _viewModel.ExpLBData,
                TabType.Cash => _viewModel.CashLBData,
                TabType.Gold => _viewModel.GoldLBData,
                TabType.Friends => _viewModel.FriendsLBData,
                _ => null,
            };
        }

        private void SetContentPosition(float newPos)
        {
            _lastContentPosition = newPos;

            var pos = _popupView.ContentRectTransform.anchoredPosition;
            pos.y = newPos;
            _popupView.ContentRectTransform.anchoredPosition = pos;
        }

        private float GetSavedContentPosition(TabType tab)
        {
            if (_contentPositionsByTab.ContainsKey(tab))
            {
                return _contentPositionsByTab[tab];
            }

            var playerItem = _virtualListDisplayer.ViewModels.FirstOrDefault(vm => vm.UserSocialData.Uid == _playerModelHolder.Uid);
            if (playerItem != null)
            {
                var index = playerItem.Rank - 1;
                return _virtualListDisplayer.GetContentPositionOnIndex(index - 3);
            }
            else
            {
                return 0f;
            }
        }

        private void OnRealtimeUpdate()
        {
            var pos = _popupView.ContentRectTransform.anchoredPosition.y;
            if (Math.Abs(_lastContentPosition - pos) > 0.05f)
            {
                _lastContentPosition = pos;
                _virtualListDisplayer.UpdateDisplayedItems();
            }
        }

        private void ShowItemDelegate(UILeaderboardsPopupItemView itemView, LeaderboardUserData itemModel)
        {
            itemView.SetRankText(itemModel.Rank.ToString());
            itemView.SetNameText($"{itemModel.UserSocialData.FirstName} {itemModel.UserSocialData.LastName}");
            itemView.SetValueText(FormattingHelper.ToCommaSeparatedNumber(itemModel.LeaderboardValue));
            itemView.SetTypeImageSprite(_currentLBTypeIconSprite);
            itemView.SetBgImageColor(_playerModelHolder.Uid == itemModel.UserSocialData.Uid ? _colorsHolder.LeaderboardUserItemBgColor : Color.white);

            var avatarSprite = _avatarsManager.GetAvatarSprite(itemModel.UserSocialData.Uid);
            if (avatarSprite != null)
            {
                itemView.SetAvatar(avatarSprite);
            }
            else
            {
                itemView.SetAvatar(null);
                _avatarsManager.LoadAvatarForUid(itemModel.UserSocialData.Uid);
            }
        }

        private void HideItemDelegate(UILeaderboardsPopupItemView itemView, LeaderboardUserData itemModel)
        {
        }

        private Sprite GetLeaderboardIconSprite()
        {
            return _currentTabType switch
            {
                TabType.Exp => _spritesProvider.GetStarIcon(isBig: false),
                TabType.Friends => _spritesProvider.GetFriendsIcon(),
                TabType.Cash => _spritesProvider.GetCashIcon(),
                TabType.Gold => _spritesProvider.GetGoldIcon(),
                _ => null,
            };
        }

        private TabType GetTabTypeByTabIndex(int tabIndex)
        {
            return tabIndex switch
            {
                0 => TabType.Exp,
                1 => TabType.Cash,
                2 => TabType.Gold,
                3 => TabType.Friends,
                _ => TabType.None,
            };
        }

        private enum TabType
        {
            None,
            Exp,
            Friends,
            Cash,
            Gold,
        }
    }
}
