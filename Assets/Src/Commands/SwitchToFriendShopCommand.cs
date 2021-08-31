using Cysharp.Threading.Tasks;

public struct SwitchToFriendShopCommand
{
    public async void Execute(FriendData friendData)
    {
        var gameStateModel = GameStateModel.Instance;
        var dispatcher = Dispatcher.Instance;
        if (gameStateModel.ViewingUserModel.Uid == friendData.Uid) return;

        dispatcher.UIRequestBlockRaycasts();

        if (friendData.IsUserModelLoaded == false)
        {
            await new LoadFriendShopCommand().ExecuteAsync(friendData);

            if (friendData.IsUserModelLoaded)
            {
                friendData.UserModel.ApplyExternalActions();
                var calculationResult = friendData.UserModel.CalculateOfflineToTime(gameStateModel.ServerTime);
                friendData.UserModel.ShopModel.RemoveProducts(calculationResult.SoldFromShelfs);
            }
        }

        if (friendData.IsUserModelLoaded)
        {
            gameStateModel.SetViewingUserModel(friendData.UserModel);
            gameStateModel.SetGameState(GameStateName.ShopFriend);
        }

        dispatcher.UIRequestUnblockRaycasts();
    }
}
