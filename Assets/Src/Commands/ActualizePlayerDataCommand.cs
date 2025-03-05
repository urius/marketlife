using Src.Managers;
using Src.Model;

namespace Src.Commands
{
    public struct ActualizePlayerDataCommand
    {
        public void Execute()
        {
            var mainConfig = GameConfigManager.Instance.MainConfig;
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var billboardModel = playerModel.ShopModel.BillboardModel;
            var loc = LocalizationManager.Instance;
            var friendsDataHolder = FriendsDataHolder.Instance;

            if (billboardModel.IsAvailable == false)
            {
                if (playerModel.ProgressModel.Level >= mainConfig.BillboardUnlockLevel)
                {
                    billboardModel.SetText(loc.GetLocalization(LocalizationKeys.BillboardDefaultText));
                    billboardModel.SetAvailable(true);
                }
            }

            if (friendsDataHolder.FriendsDataIsSet == true)
            {
                ActualizeFriendsActions();
            }
            else
            {
                friendsDataHolder.FriendsDataWasSetup += OnFriendsDataWasSetup;
            }
        }

        private void OnFriendsDataWasSetup()
        {
            var friendsDataHolder = FriendsDataHolder.Instance;
            friendsDataHolder.FriendsDataWasSetup -= OnFriendsDataWasSetup;
            ActualizeFriendsActions();
        }

        private void ActualizeFriendsActions()
        {
            var playerModel = PlayerModelHolder.Instance.UserModel;
            var allFriendsActionsModel = playerModel.FriendsActionsDataModels;
            var friendsData = FriendsDataHolder.Instance.Friends;
            var friendActionsConfig = GameConfigManager.Instance.FriendActionsConfig;

            foreach (var friendData in friendsData)
            {
                if (friendData.IsApp == false) continue;
                if (allFriendsActionsModel.FriendShopActionsModelByUid.ContainsKey(friendData.Uid) == false)
                {
                    allFriendsActionsModel.AddActionsModelForUid(friendData.Uid);
                }

                var friendActionsModel = allFriendsActionsModel.GetFriendShopActionsModel(friendData.Uid);
                foreach (var actionId in FriendShopActionsModel.SupportedActions)
                {
                    if (friendActionsModel.ActionsById.ContainsKey(actionId) == false)
                    {
                        var actionData = new FriendShopActionData(actionId, friendActionsConfig.GetDefaultActionAmount(actionId));
                        friendActionsModel.AddActionData(actionData);
                    }
                }
            }
        }
    }
}
