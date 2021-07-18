using Cysharp.Threading.Tasks;

public struct InitializeAndLoadCommand
{
    public async UniTask ExecuteAsync()
    {
        var playerModelHolder = PlayerModelHolder.Instance;
        var gameStateModel = GameStateModel.Instance;
        gameStateModel.SetGameState(GameStateName.Loading);

        new InitializeSystemsCommand().Execute();

        await new LoadServerTimeCommand().ExecuteAsync();
        await new LoadLocalizationCommand().ExecuteAsync();
        await new LoadConfigsCommand().ExecuteAsync();
        await new LoadAssetsCommand().ExecuteAsync();
        await new LoadPlayerShopCommand().ExecuteAsync();

        gameStateModel.SetPlayerShopModel(playerModelHolder.ShopModel);
        gameStateModel.SetGameState(GameStateName.Loaded);

        gameStateModel.SetViewingShopModel(playerModelHolder.ShopModel);
        gameStateModel.SetGameState(GameStateName.ReadyForStart);
    }
}
