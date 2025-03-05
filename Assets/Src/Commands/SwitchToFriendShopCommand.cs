using Src.Commands.LoadSave;
using Src.Common;
using Src.Managers;
using Src.Model;

namespace Src.Commands
{
    public struct SwitchToFriendShopCommand
    {
        public async void Execute(FriendData friendData)
        {
            var gameStateModel = GameStateModel.Instance;
            var dispatcher = Dispatcher.Instance;
            var analyticsManager = AnalyticsManager.Instance;

            if (gameStateModel.ViewingUserModel.Uid == friendData.Uid
                || friendData.IsApp == false) return;

            analyticsManager.SendCustom(AnalyticsManager.EventNameVisitFriendClicked);

            dispatcher.UIRequestBlockRaycasts();
            if (friendData.IsUserModelLoaded == false)
            {
                await new LoadFriendShopCommand().ExecuteAsync(friendData);

                if (friendData.IsUserModelLoaded)
                {
                    var friendModel = friendData.UserModel;
                    var calculationResult = friendModel.CalculateOfflineToTime(gameStateModel.ServerTime);
                    friendModel.ShopModel.RemoveProducts(calculationResult.SoldFromShelfs);
                    friendModel.ApplyExternalActions();
                }
            }
            if (friendData.IsUserModelLoaded)
            {
                gameStateModel.SetViewingUserModel(friendData.UserModel);
                gameStateModel.SetGameState(GameStateName.ShopFriend);
            }
            else
            {
                analyticsManager.SendCustom(AnalyticsManager.EventNameVisitFriendFailed);
            }
            dispatcher.UIRequestUnblockRaycasts();
        }
    }
}
