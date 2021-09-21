using Cysharp.Threading.Tasks;

public struct InitializeAndLoadCommand
{
    public async UniTask ExecuteAsync()
    {
        var playerModelHolder = PlayerModelHolder.Instance;
        var gameStateModel = GameStateModel.Instance;
        gameStateModel.SetGameState(GameStateName.Loading);

        await new LoadServerTimeCommand().ExecuteAsync();
        await new LoadLocalizationCommand().ExecuteAsync();
        await new LoadConfigsCommand().ExecuteAsync();
        await new LoadAssetsCommand().ExecuteAsync();
        await new LoadPlayerShopCommand().ExecuteAsync();
        await new LoadCompensationDataCommand().ExecuteAsync();

        gameStateModel.SetGameState(GameStateName.Loaded);
        gameStateModel.SetViewingUserModel(playerModelHolder.UserModel);
        gameStateModel.SetGameState(GameStateName.ReadyForStart);
    }
}
