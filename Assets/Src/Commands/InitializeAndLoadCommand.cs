using Cysharp.Threading.Tasks;

public struct InitializeAndLoadCommand
{
    public async UniTask ExecuteAsync()
    {
        var playerModelHolder = PlayerModelHolder.Instance;
        var gameStateModel = GameStateModel.Instance;
        var loadGameProgressModel = LoadGameProgressModel.Instance;

        gameStateModel.SetGameState(GameStateName.Loading);
        var phasesData = GetLoadPhases();
        loadGameProgressModel.SetupPartsCount(phasesData.Length);
        for (var i = 0; i < phasesData.Length; i++)
        {
            loadGameProgressModel.SetCurrentPhaseName(phasesData[i].Phase);
            
            await UniTask.Delay(50);
            
            var result = await phasesData[i].Command.ExecuteAsync();
            if (result)
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

    private static (LoadGamePhase Phase, IAsyncGameLoadCommand Command)[] GetLoadPhases()
    {
        var socialType = PlayerModelHolder.Instance.SocialType;
        switch (socialType)
        {
            case SocialType.VK:
                return new (LoadGamePhase Phase, IAsyncGameLoadCommand Command)[]
                {
                    (LoadGamePhase.LoadTime, new LoadServerTimeCommand()),
                    (LoadGamePhase.LoadShopData, new LoadPlayerDataCommand()),
                    (LoadGamePhase.LoadLocalization, new LoadLocalizationCommand()),
                    (LoadGamePhase.LoadConfigs, new LoadConfigsCommand()),
                    (LoadGamePhase.LoadAssets, new LoadAssetsCommand()),
                    (LoadGamePhase.CreatePlayerModel, new CreatePlayerModelCommand()),
                    (LoadGamePhase.LoadCompensationData, new LoadCompensationDataCommand()),
                };
            case SocialType.Undefined:
            case SocialType.YG:
            default:
                return new (LoadGamePhase Phase, IAsyncGameLoadCommand Command)[]
                {
                    (LoadGamePhase.LoadTime, new LoadServerTimeCommand()),
                    (LoadGamePhase.LoadShopData, new LoadPlayerDataCommand()),
                    (LoadGamePhase.LoadLocalization, new LoadLocalizationCommand()),
                    (LoadGamePhase.LoadConfigs, new LoadConfigsCommand()),
                    (LoadGamePhase.LoadAssets, new LoadAssetsCommand()),
                    (LoadGamePhase.CreatePlayerModel, new CreatePlayerModelCommand()),
                };
        }
    }
}
