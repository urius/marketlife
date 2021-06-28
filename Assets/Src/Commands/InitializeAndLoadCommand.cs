using Cysharp.Threading.Tasks;

public struct InitializeAndLoadCommand
{
    public async UniTask ExecuteAsync()
    {
        var playerModel = PlayerModel.Instance;
        var gameStateModel = GameStateModel.Instance;
        gameStateModel.SetGameState(GameStateName.Loading);

        new InitializeSystemsCommand().Execute();

        await new LoadServerTimeCommand().ExecuteAsync();
        await new LoadLocalizationCommand().ExecuteAsync();
        await new LoadConfigsCommand().ExecuteAsync();
        await new LoadGraphicsCommand().ExecuteAsync();
        await new LoadPlayerShopCommand().ExecuteAsync();

        gameStateModel.SetPlayerShopModel(playerModel.ShopModel);
        gameStateModel.SetGameState(GameStateName.Loaded);

        gameStateModel.SetViewingShopModel(playerModel.ShopModel);
        gameStateModel.SetGameState(GameStateName.ShopSimulation);
    }
}
