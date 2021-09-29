using Cysharp.Threading.Tasks;

public struct LoadPlayerShopCommand : IAsyncGameLoadCommand
{
    public async UniTask<bool> ExecuteAsync()
    {
        var playerModelHolder = PlayerModelHolder.Instance;

        var loadedUserModel = await new LoadUserModelCommand().ExecuteAsync(playerModelHolder.Uid);
        if (loadedUserModel != null)
        {
            playerModelHolder.SetUserModel(loadedUserModel);
            AnalyticsManager.Instance.SetupMetaParameter(AnalyticsManager.LevelParamName, loadedUserModel.ProgressModel.Level);
        }

        return loadedUserModel != null;
    }
}
