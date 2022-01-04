public class DailyMissionAddGoldProcessor : DailyMissionProcessorBase
{
    private readonly PlayerModelHolder _playerModelHolder;

    public DailyMissionAddGoldProcessor(DailyMissionModel missionModel) : base(missionModel)
    {
        _playerModelHolder = PlayerModelHolder.Instance;
    }

    public override void Start()
    {
        _playerModelHolder.UserModel.ProgressModel.GoldChanged += OnGoldChanged;
    }

    public override void Stop()
    {
        _playerModelHolder.UserModel.ProgressModel.GoldChanged -= OnGoldChanged;
    }

    private void OnGoldChanged(int prevValue, int currentValue)
    {
        var delta = currentValue - prevValue;
        if (delta > 0)
        {
            MissionModel.AddValue(delta);
        }
    }
}
