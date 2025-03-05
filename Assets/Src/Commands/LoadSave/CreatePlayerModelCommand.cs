using Cysharp.Threading.Tasks;
using Src.Managers;
using Src.Model;

namespace Src.Commands.LoadSave
{
    public struct CreatePlayerModelCommand : IAsyncGameLoadCommand
    {
        public UniTask<bool> ExecuteAsync()
        {
            var result = false;
            var playerModelHolder = PlayerModelHolder.Instance;

            if (playerModelHolder.UserDataStr != null)
            {
                var loadedUserModel = new CreateUserModelCommand().Execute(playerModelHolder.UserDataStr);
                if (loadedUserModel != null)
                {
                    playerModelHolder.SetUserModel(loadedUserModel);
                    AnalyticsManager.Instance.SetupMetaParameter(AnalyticsManager.LevelParamName, loadedUserModel.ProgressModel.Level);
                    result = true;
                }
            }

            return UniTask.FromResult(result);
        }
    }
}
