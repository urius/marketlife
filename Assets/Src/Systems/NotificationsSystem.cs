using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Src.Common;
using Src.Common_Utils;
using Src.Model;

namespace Src.Systems
{
    public class NotificationsSystem
    {
        private readonly GameStateModel _gameStateModel;
        private readonly PlayerModelHolder _playerModelHodler;
        private readonly Dispatcher _dispatcher;
        private readonly FriendsDataHolder _friendsDataHodler;
        private readonly Dictionary<string, NotificationData> _notificationDataByUid = new();

        private Action _saveCompletedDelegate = null;
        private Action _saveExternalDataCompletedDelegate = null;
        private UserModel _currentViewingUserModel = null;

        public NotificationsSystem()
        {
            _gameStateModel = GameStateModel.Instance;
            _playerModelHodler = PlayerModelHolder.Instance;
            _dispatcher = Dispatcher.Instance;
            _friendsDataHodler = FriendsDataHolder.Instance;
        }

        public async void Start()
        {
            await _gameStateModel.GameDataLoadedTask;
            await _friendsDataHodler.FriendsDataSetupTask;
            if (false == DateTimeHelper.IsSameDays(_playerModelHodler.UserModel.StatsData.LastVisitTimestamp, _gameStateModel.ServerTime))
            {
                await AddInactiveFriendsNotifications();
            }

            Activate();
        }

        private UniTask AddInactiveFriendsNotifications()
        {
            var inactiveFriendsUids = _friendsDataHodler.Friends.Where(f => f.IsInactive()).Select(f => f.Uid).ToArray();
            var inactiveFriendsUidsStr = string.Join(",", inactiveFriendsUids);
            return SendAddNotificationAsync(inactiveFriendsUidsStr, NotificationType.NotifyInactive);
        }

        private void Activate()
        {
            _dispatcher.SaveCompleted += OnSaveCompleted;
            _dispatcher.SaveExternalDataCompleted += OnSaveExternalDataCompleted;
            _gameStateModel.ViewingUserModelChanged += OnViewingUserModelChanged;
        }

        private void OnViewingUserModelChanged(UserModel user)
        {
            DeactivateCurrentFriendModel();
            _currentViewingUserModel = user;
            ActivateCurrentFriendModel();
        }

        private void ActivateCurrentFriendModel()
        {
            if (_currentViewingUserModel.Uid != _playerModelHodler.Uid)
            {
                _currentViewingUserModel.ExternalActionsModel.ActionAdded += OnExternalActionAdded;
            }
        }

        private void DeactivateCurrentFriendModel()
        {
            if (_currentViewingUserModel != null)
            {
                _currentViewingUserModel.ExternalActionsModel.ActionAdded -= OnExternalActionAdded;
            }
        }

        private void OnExternalActionAdded(ExternalActionModelBase actionModel)
        {
            switch (actionModel.ActionId)
            {
                case FriendShopActionId.TakeProduct:
                case FriendShopActionId.AddUnwash:
                    AddNotification(NotificationType.YouHaveGuests);
                    break;
                case FriendShopActionId.AddProduct:
                    AddNotification(NotificationType.Gift);
                    break;
            }
        }

        private void AddNotification(NotificationType notificationType)
        {
            var priority = GetNotificationPriority(notificationType);
            var friendUid = _currentViewingUserModel.Uid;
            if (_notificationDataByUid.ContainsKey(friendUid) == false
                || priority > GetNotificationPriority(_notificationDataByUid[friendUid].Type))
            {
                var notificationData = new NotificationData
                {
                    Type = notificationType
                };
                _notificationDataByUid[friendUid] = notificationData;
                _saveExternalDataCompletedDelegate = AddFriendNotificationDelegate;
            }
        }

        private void AddFriendNotificationDelegate()
        {
            if (_currentViewingUserModel != null)
            {
                var uid = _currentViewingUserModel.Uid;
                if (_notificationDataByUid.TryGetValue(uid, out var notificationData))
                {
                    SendAddNotificationAsync(uid, notificationData.Type);
                }
            }
        }

        private int GetNotificationPriority(NotificationType notificationType)
        {
            return (int)notificationType;
        }

        private void OnSaveExternalDataCompleted(bool isSuccess)
        {
            if (isSuccess)
            {
                _saveExternalDataCompletedDelegate?.Invoke();
                _saveExternalDataCompletedDelegate = null;
            }
        }

        private void OnSaveCompleted(bool isSuccess, SaveField _)
        {
            if (isSuccess)
            {
                _saveCompletedDelegate?.Invoke();
                _saveCompletedDelegate = null;
            }
        }

        private UniTask SendAddNotificationAsync(string uids, NotificationType type)
        {
            var url = string.Format(Urls.AddNotificationsURL, uids, (int)type);
            return new WebRequestsSender().GetAsync(url);
        }
    }

    public struct NotificationData
    {
        public NotificationType Type;
    }

    public enum NotificationType
    {
        None = 0,
        ReturnToGame = 1,
        NeedRefreshProducts = 2,
        YouHaveGuests = 3,
        Grabbed = 4,
        AddedUnwash = 5,
        Gift = 6,
        NotifyInactive = 7,
    }
}