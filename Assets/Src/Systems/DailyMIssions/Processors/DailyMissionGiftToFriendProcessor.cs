using System;
using System.Collections.Generic;
using Src.Model;

namespace Src.Systems.DailyMIssions.Processors
{
    public class DailyMissionGiftToFriendProcessor : DailyMissionProcessorBase
    {
        private readonly GameStateModel _gameStateModel;
        private readonly PlayerModelHolder _playerModelHolder;
        private readonly LinkedList<string> _giftedUids = new LinkedList<string>();

        private Action _unsubscribeAction = null;

        public DailyMissionGiftToFriendProcessor()
        {
            _gameStateModel = GameStateModel.Instance;
            _playerModelHolder = PlayerModelHolder.Instance;
        }

        public override void Start()
        {
            _gameStateModel.ViewingUserModelChanged += OnViewingUserModelChanged;
        }

        public override void Stop()
        {
            _gameStateModel.ViewingUserModelChanged -= OnViewingUserModelChanged;
            CallUnsubscribeAction();
        }

        private void OnViewingUserModelChanged(UserModel userModel)
        {
            CallUnsubscribeAction();
            if (_playerModelHolder.Uid != userModel.Uid)
            {
                SubscribeForFriendShop(userModel);
            }
        }

        private void CallUnsubscribeAction()
        {
            _unsubscribeAction?.Invoke();
            _unsubscribeAction = null;
        }

        private void SubscribeForFriendShop(UserModel userModel)
        {
            userModel.ExternalActionsModel.ActionAdded += OnExternalActionAdded;
            _unsubscribeAction = () => userModel.ExternalActionsModel.ActionAdded -= OnExternalActionAdded;
        }

        private void OnExternalActionAdded(ExternalActionModelBase actionModel)
        {
            if (actionModel.ActionId == FriendShopActionId.AddProduct)
            {
                if (_giftedUids.Contains(_gameStateModel.ViewingUserModel.Uid) == false)
                {
                    _giftedUids.AddLast(_gameStateModel.ViewingUserModel.Uid);
                    MissionModel.AddValue(1);
                }
            }
        }
    }
}
