using Cysharp.Threading.Tasks;

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
