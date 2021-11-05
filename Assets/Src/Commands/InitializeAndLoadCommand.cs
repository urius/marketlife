using Cysharp.Threading.Tasks;

public struct InitializeAndLoadCommand
{
    public async UniTask ExecuteAsync()
    {
        var playerModelHolder = PlayerModelHolder.Instance;
        var gameStateModel = GameStateModel.Instance;
        var loadGameProgressModel = LoadGameProgressModel.Instance;

        gameStateModel.SetGameState(GameStateName.Loading);
        var phasesData = new (LoadGamePhase Phase, IAsyncGameLoadCommand Command)[]
        {
            (LoadGamePhase.LoadTime, new LoadServerTimeCommand()),
            (LoadGamePhase.LoadLocalization, new LoadLocalizationCommand()),
            (LoadGamePhase.LoadConfigs, new LoadConfigsCommand()),
            (LoadGamePhase.LoadAssets, new LoadAssetsCommand()),
            (LoadGamePhase.LoadShopData, new LoadPlayerShopCommand()),
            (LoadGamePhase.LoadCompensationData, new LoadCompensationDataCommand()),
        };
        loadGameProgressModel.SetupPartsCount(phasesData.Length);
        for (var i = 0; i < phasesData.Length; i++)
        {
            loadGameProgressModel.SetCurrentPhaseName(phasesData[i].Phase);
            var result = await phasesData[i].Command.ExecuteAsync();
            if (result == true)
            {
                loadGameProgressModel.SetCurrentPartLoaded();
            }
            else
            {
                loadGameProgressModel.SetErrorState();
                return;
            }
        }

        await UniTask.Delay(500);
        gameStateModel.SetGameState(GameStateName.Loaded);
        new ActualizePlayerDataCommand().Execute();
        gameStateModel.SetViewingUserModel(playerModelHolder.UserModel);

        await UniTask.Delay(1500);
        gameStateModel.SetGameState(GameStateName.ReadyForStart);
    }
}
