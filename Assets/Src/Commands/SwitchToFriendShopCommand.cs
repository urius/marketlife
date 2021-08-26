using Cysharp.Threading.Tasks;

public struct SwitchToFriendShopCommand
{
    public async UniTaskVoid Execute(FriendData friendData)
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
