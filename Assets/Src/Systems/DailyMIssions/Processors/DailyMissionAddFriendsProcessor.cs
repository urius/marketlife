using System;

public class DailyMissionAddFriendsProcessor : DailyMissionProcessorBase
{
    private readonly GameStateModel _gameStateModel;
    private readonly FriendsDataHolder _friendsDataHolder;

    public DailyMissionAddFriendsProcessor()
    {
        _gameStateModel = GameStateModel.Instance;
        _friendsDataHolder = FriendsDataHolder.Instance;
    }

    public override async void Start()
    {
        await _friendsDataHolder.FriendsDataSetupTask;
        _gameStateModel.GameStateChanged += OnGameStateChanged;
        Update();
    }

    public override void Stop()
    {
        _gameStateModel.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        Update();
    }

    private void Update()
    {
        if (_gameStateModel.IsPlayingState
            && _friendsDataHolder.FriendsDataIsSet)
        {
            _gameStateModel.GameStateChanged -= OnGameStateChanged;
            MissionModel.SetValue(_friendsDataHolder.InGameFriendsCount);
        }
    }
}
