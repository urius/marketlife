using System;
using System.Collections.Generic;
using System.Linq;
using Src.Common;
using Src.Managers;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Popups;
using UnityEngine;

namespace Src.View.UI.Popups.Upgrades_Popup
{
    public class UIUpgradesPopupMediator : IMediator
    {
        private readonly RectTransform _parentTransform;
        private readonly GameStateModel _gameStateModel;
        private readonly UserModel _playerModel;
        private readonly ShopPersonalModel _personalModel;
        private readonly PrefabsHolder _prefabsHolder;
        private readonly LocalizationManager _loc;
        private readonly SpritesProvider _spritesProvider;
        private readonly Dispatcher _dispatcher;
        private readonly UpdatesProvider _updatesProvider;
        private readonly PoolCanvasProvider _poolCanvasProvider;
        private readonly FriendsDataHolder _friendsDataHolder;
        private readonly List<(UIUpgradesPopupItemView View, UpgradesPopupItemViewModelBase Model)> _displayedItems = new(3);

        private UpgradesPopupViewModel _viewModel;
        private UITabbedContentPopupView _popupView;
        private Queue<UIUpgradesPopupItemView> _cachedViews = new();
        private int _currentShowingTabIndex = -1;

        public UIUpgradesPopupMediator(RectTransform parentTransform)
        {
            _parentTransform = parentTransform;
            _gameStateModel = GameStateModel.Instance;
            _playerModel = PlayerModelHolder.Instance.UserModel;
            _personalModel = _playerModel.ShopModel.PersonalModel;
            _prefabsHolder = PrefabsHolder.Instance;
            _loc = LocalizationManager.Instance;
            _spritesProvider = SpritesProvider.Instance;
            _dispatcher = Dispatcher.Instance;
            _updatesProvider = UpdatesProvider.Instance;
            _poolCanvasProvider = PoolCanvasProvider.Instance;
            _friendsDataHolder = FriendsDataHolder.Instance;
        }

        public async void Mediate()
        {
            _viewModel = _gameStateModel.ShowingPopupModel as UpgradesPopupViewModel;

            var popupGo = GameObject.Instantiate(_prefabsHolder.UITabbedContentPopupPrefab, _parentTransform);
            _popupView = popupGo.GetComponent<UITabbedContentPopupView>();
            _popupView.SetSize(780, 850);
            _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupUpgradesTitle));
            _popupView.SetupTabButtons(_viewModel.TabKeys.Select(TabTypeToTabName).ToArray());

            ShowTab(Math.Max(0, _viewModel.GetTabIndex(_viewModel.ShowOnTab)));

            await _popupView.Appear2Async();

            Activate();
        }

        public async void Unmediate()
        {
            Deactivate();

            foreach (var cachedView in _cachedViews)
            {
                GameObject.Destroy(cachedView);
            }

            await _popupView.Disppear2Async();

            GameObject.Destroy(_popupView.gameObject);
        }

        private void Activate()
        {
            _popupView.TabButtonClicked += OnTabButtonClicked;
            _updatesProvider.RealtimeSecondUpdate += OnRealtimeSecondPassed;
            _popupView.ButtonCloseClicked += OnButtonCloseClicked;
            _viewModel.ItemsUpdated += OnItemsUpdated;
            _viewModel.TabSelected += OnTabSelected;
            _personalModel.PersonalWorkingTimeUpdated += OnPersonalWorkingTimeUpdated;
        }

        private void Deactivate()
        {
            foreach (var (View, Model) in _displayedItems)
            {
                DeactivateItem(View);
            }
            _popupView.TabButtonClicked -= OnTabButtonClicked;
            _updatesProvider.RealtimeSecondUpdate -= OnRealtimeSecondPassed;
            _popupView.ButtonCloseClicked -= OnButtonCloseClicked;
            _viewModel.ItemsUpdated -= OnItemsUpdated;
            _viewModel.TabSelected -= OnTabSelected;
            _personalModel.PersonalWorkingTimeUpdated -= OnPersonalWorkingTimeUpdated;
        }

        private void OnTabSelected(TabType tabType)
        {
            ShowTab(_viewModel.GetTabIndex(tabType));
        }

        private void OnPersonalWorkingTimeUpdated(PersonalConfig personalConfig)
        {
            if (_viewModel.GetTabIndex(TabType.ManagePersonal) == _currentShowingTabIndex)
            {
                RefreshShowingTabView();
            }
        }

        private void OnItemsUpdated()
        {
            RefreshShowingTabView();
        }

        private void OnButtonCloseClicked()
        {
            _dispatcher.UIRequestRemoveCurrentPopup();
        }

        private void OnRealtimeSecondPassed()
        {
            foreach (var (View, Model) in _displayedItems)
            {
                if (Model.ItemType == UpgradesPopupItemType.Personal)
                {
                    var personalItemViewModel = (Model as UpgradesPopupPersonalItemViewModel);
                    var restWorkTimeSec = _personalModel.GetEndWorkTime(personalItemViewModel.PersonalConfig) - _gameStateModel.ServerTime;
                    if (restWorkTimeSec >= 0)
                    {
                        UpdatePersonalItemState(View, restWorkTimeSec);
                        if (restWorkTimeSec == 0)
                        {
                            SetupItem(View, Model);
                        }
                    }
                }
            }
        }

        private void OnTabButtonClicked(int tabIndex)
        {
            _viewModel.OnTabClicked(tabIndex);
        }

        private void ShowTab(int tabIndex)
        {
            _popupView.SetTabButtonSelected(tabIndex);
            _currentShowingTabIndex = tabIndex;
            RefreshShowingTabView();
        }

        private void RefreshShowingTabView()
        {
            ClearShownItems();

            var itemModels = _viewModel.ItemViewModelsByTabKey[_viewModel.TabKeys[_currentShowingTabIndex]];
            var itemSize = (_prefabsHolder.UIUpgradePopupItemPrefab.transform as RectTransform).sizeDelta;

            for (var i = 0; i < itemModels.Length; i++)
            {
                var itemView = GetOrCreateItem();
                _displayedItems.Add((itemView, itemModels[i]));
                var itemRectTrensform = itemView.transform as RectTransform;
                itemRectTrensform.anchoredPosition = new Vector2(itemRectTrensform.anchoredPosition.x, -i * itemSize.y);
                SetupItem(itemView, itemModels[i]);
                ActivateItem(itemView);
            }

            _popupView.SetContentHeight(itemModels.Length * itemSize.y);
        }

        private UIUpgradesPopupItemView GetOrCreateItem()
        {
            UIUpgradesPopupItemView result;
            if (_cachedViews.Count > 0)
            {
                result = _cachedViews.Dequeue();
                result.transform.SetParent(_popupView.ContentRectTransform);
            }
            else
            {
                var itemViewGo = GameObject.Instantiate(_prefabsHolder.UIUpgradePopupItemPrefab, _popupView.ContentRectTransform);
                result = itemViewGo.GetComponent<UIUpgradesPopupItemView>();
            }
            return result;
        }

        private void ClearShownItems()
        {
            foreach (var (View, _) in _displayedItems)
            {
                DeactivateItem(View);
                View.transform.SetParent(_poolCanvasProvider.PoolCanvasTransform);
                _cachedViews.Enqueue(View);
            }
            _displayedItems.Clear();
        }

        private void ActivateItem(UIUpgradesPopupItemView itemView)
        {
            itemView.BuyClicked += OnItemBuyClicked;
        }

        private void DeactivateItem(UIUpgradesPopupItemView itemView)
        {
            itemView.BuyClicked -= OnItemBuyClicked;
        }

        private void OnItemBuyClicked(UIUpgradesPopupItemView itemView)
        {
            var viewModel = _displayedItems.First(i => i.View == itemView).Model;
            _dispatcher.UIUpgradePopupBuyClicked(viewModel);
        }

        private void SetupItem(UIUpgradesPopupItemView itemView, UpgradesPopupItemViewModelBase viewModel)
        {
            if (viewModel.ItemType == UpgradesPopupItemType.Personal)
            {
                var personalViewModel = viewModel as UpgradesPopupPersonalItemViewModel;
                var personalConfig = personalViewModel.PersonalConfig;
                var restWorkTimeSec = _personalModel.GetEndWorkTime(personalConfig) - _gameStateModel.ServerTime;
                var isWorking = restWorkTimeSec > 0;
                itemView.SetupState(isUnlocked: true, isWorking);
                UpdatePersonalItemState(itemView, restWorkTimeSec);
                itemView.SetIconSprite(_spritesProvider.GetPersonalIcon(personalConfig.Key));
                var hoursStr = string.Format(_loc.GetLocalization(LocalizationKeys.CommonHoursShortFormat), personalConfig.WorkHours);
                var personalNameStr = _loc.GetLocalization($"{LocalizationKeys.CommonPersonalNamePrefix}{personalConfig.TypeIdStr}");
                itemView.SetTitleText($"{personalNameStr} ({hoursStr})");
                itemView.SetDescriptionText(_loc.GetLocalization($"{LocalizationKeys.PopupUpgradesPersonalDescriptionPrefix}{personalConfig.TypeIdStr}"));
                var price = personalConfig.GetPrice(_playerModel.ShopModel.ShopDesign.Square);
                itemView.SetPrice(price.IsGold, price.Value);
            }
            else if (viewModel.ItemType == UpgradesPopupItemType.Upgrade)
            {
                var upgradeViewModel = viewModel as UpgradesPopupUpgradeItemViewModel;
                var upgradeConfig = upgradeViewModel.UpgradeConfig;
                var playerLevel = _playerModel.ProgressModel.Level;
                var playerFriendsCount = _friendsDataHolder.InGameFriendsCount;
                var isLockedByLevel = playerLevel < upgradeConfig.UnlockLevel;
                var isLockedByFriends = playerFriendsCount < upgradeConfig.UnlockFriends;
                var isLocked = isLockedByLevel || isLockedByFriends;
                var isMaxReached = upgradeViewModel.IsMaxReached;
                itemView.SetupState(!isLocked, isMaxReached);
                itemView.SetIconSprite(_spritesProvider.GetUpgradeIcon(upgradeConfig.UpgradeType));

                itemView.SetTitleText(
                    string.Format(
                        _loc.GetLocalization($"{LocalizationKeys.CommonUpgradeNameFormat}{upgradeConfig.UpgradeTypeStr}"),
                        upgradeConfig.Value));
                itemView.SetDescriptionText(
                    string.Format(
                        _loc.GetLocalization($"{LocalizationKeys.PopupUpgradesUpgradeDescriptionPrefix}{upgradeConfig.UpgradeTypeStr}"),
                        upgradeConfig.Value));
                if (isMaxReached)
                {
                    itemView.SetStatusText(_loc.GetLocalization(LocalizationKeys.PopupUpgradesMaxLevel));
                }
                if (isLocked)
                {
                    SetupUpgradeItemUnlockRequirements(itemView, upgradeViewModel, isLockedByLevel, isLockedByFriends);
                }
                itemView.SetPrice(upgradeConfig.Price.IsGold, upgradeConfig.Price.Value);
            }
        }

        private void SetupUpgradeItemUnlockRequirements(
            UIUpgradesPopupItemView itemView,
            UpgradesPopupUpgradeItemViewModel upgradeViewModel,
            bool isLockedByLevel,
            bool isLockedByFriends)
        {
            var upgradeConfig = upgradeViewModel.UpgradeConfig;
            var requirementsCount = (isLockedByLevel ? 1 : 0) + (isLockedByFriends ? 1 : 0);

            itemView.SetUnlockState(_loc.GetLocalization(LocalizationKeys.PopupUpgradesUnlockUpgradeCaption), requirementsCount);
            var i = 0;
            if (isLockedByFriends)
            {
                itemView.SetUnlockRequirementData(
                    i,
                    string.Format(_loc.GetLocalization(LocalizationKeys.PopupUpgradesUnlockUpgradeFriendsFormat), upgradeConfig.UnlockFriends),
                    _spritesProvider.GetFriendsIcon());
                i++;
            }
            if (isLockedByLevel)
            {
                itemView.SetUnlockRequirementData(
                    i,
                    string.Format(_loc.GetLocalization(LocalizationKeys.PopupUpgradesUnlockUpgradeLevelFormat), upgradeConfig.UnlockLevel),
                    _spritesProvider.GetStarIcon(isBig: false));
                i++;
            }
        }

        private void UpdatePersonalItemState(UIUpgradesPopupItemView itemView, int restWorkTimeSec)
        {
            itemView.SetStatusText(
                string.Format(
                    _loc.GetLocalization(LocalizationKeys.PopupUpgradesPersonalIsWorkingFormat),
                    FormattingHelper.ToSeparatedTimeFormat(restWorkTimeSec)));
        }

        private string TabTypeToTabName(TabType tabType)
        {
            string localizationKey = null;
            switch (tabType)
            {
                case TabType.WarehouseUpgrades:
                    localizationKey = LocalizationKeys.PopupUpgradesWarehouseTab;
                    break;
                case TabType.ExpandUpgrades:
                    localizationKey = LocalizationKeys.PopupUpgradesExpandTab;
                    break;
                case TabType.ManagePersonal:
                    localizationKey = LocalizationKeys.PopupUpgradesPersonalTab;
                    break;
            }

            if (localizationKey != null)
            {
                return _loc.GetLocalization(localizationKey);
            }
            return "???";
        }
    }
}
