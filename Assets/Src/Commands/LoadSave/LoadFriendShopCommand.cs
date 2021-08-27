using Cysharp.Threading.Tasks;

public struct LoadFriendShopCommand
{
    public async UniTask<bool> ExecuteAsync(FriendData friendData)
    {
        if (friendData != null && friendData.IsUserModelLoaded == false)
        {
            var loadedFriendModel = await new LoadUserModelCommand().ExecuteAsync(friendData.Uid);
            if (loadedFriendModel != null)
            {
                friendData.SetUserModel(loadedFriendModel);
                return true;
            }
        }
        return true;
    }
}
