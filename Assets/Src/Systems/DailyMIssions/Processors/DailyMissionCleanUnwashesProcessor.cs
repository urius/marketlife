using UnityEngine;

public class DailyMissionCleanUnwashesProcessor : DailyMissionProcessorBase
{
    private readonly PlayerModelHolder _playerModelHolder;
    private readonly GameStateModel _gameStateModel;
    private readonly PlayerOfflineReportHolder _offlineReportHolder;

    public DailyMissionCleanUnwashesProcessor()
    {
        _playerModelHolder = PlayerModelHolder.Instance;
        _gameStateModel = GameStateModel.Instance;
        _offlineReportHolder = PlayerOfflineReportHolder.Instance;
    }

    public override void Start()
    {
        _playerModelHolder.ShopModel.UnwashRemoved += OnUnwashRemoved;
        _gameStateModel.GameStateChanged += OnGameStateChanged;
    }

    public override void Stop()
    {
        _playerModelHolder.ShopModel.UnwashRemoved -= OnUnwashRemoved;
        _gameStateModel.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        if (prevState == GameStateName.ReadyForStart && _gameStateModel.IsPlayingState)
        {
            if (_offlineReportHolder.PlayerOfflineReport != null)
            {
                var unwashesCleaned = _offlineReportHolder.PlayerOfflineReport.UnwashesCleanedAmount;
                if (unwashesCleaned > 0)
                {
                    MissionModel.AddValue(unwashesCleaned);
                }
            }
        }
    }

    private void OnUnwashRemoved(Vector2Int coords)
    {
        if (_gameStateModel.GameState == GameStateName.PlayerShopSimulation)
        {
            MissionModel.AddValue(1);
        }
    }
}
