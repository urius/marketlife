using Cysharp.Threading.Tasks;
using Src.Model;

namespace Src.Commands.LoadSave
{
    public struct LoadFriendShopCommand
    {
        public async UniTask<bool> ExecuteAsync(FriendData friendData)
        {
            var result = false;
            if (friendData != null)
            {
                result = friendData.IsUserModelLoaded;
                if (friendData.IsUserModelLoaded == false)
                {
                    var userDataStr = await new LoadUserDataCommand().ExecuteAsync(friendData.Uid);
                    if (userDataStr != null)
                    {
                        var loadedFriendModel = new CreateUserModelCommand().Execute(userDataStr);
                        if (loadedFriendModel != null)
                        {
                            friendData.SetUserModel(loadedFriendModel);
                            result = true;
                        }
                    }
                }
            }
            return result;
        }
    }
}
