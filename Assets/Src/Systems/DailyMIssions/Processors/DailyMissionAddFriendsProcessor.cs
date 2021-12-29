public class DailyMissionAddFriendsProcessor : DailyMissionProcessorBase
{
    private readonly GameStateModel _gameStateModel;
    private readonly FriendsDataHolder _friendsDataHolder;

    public DailyMissionAddFriendsProcessor(DailyMissionModel missionModel)
        : base(missionModel)
    {
        _gameStateModel = GameStateModel.Instance;
        _friendsDataHolder = FriendsDataHolder.Instance;
    }

    public override void Activate()
    {
        _gameStateModel.GameStateChanged += OnGameStateChanged;
    }

    public override void Deactivate()
    {
        _gameStateModel.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        if (_gameStateModel.IsPlayingState
            && _friendsDataHolder.FriendsDataIsSet)
        {
            _gameStateModel.GameStateChanged -= OnGameStateChanged;
            MissionModel.SetValue(_friendsDataHolder.InGameFriendsCount);
        }
    }
}
