public class DailyMissionAddCashProcessor : DailyMissionProcessorBase
{
    private readonly PlayerModelHolder _playerModelHolder;

    public DailyMissionAddCashProcessor(DailyMissionModel missionModel) : base(missionModel)
    {
        _playerModelHolder = PlayerModelHolder.Instance;
    }

    public override void Start()
    {
        _playerModelHolder.UserModel.ProgressModel.CashChanged += OnCashChanged;
    }

    public override void Stop()
    {
        _playerModelHolder.UserModel.ProgressModel.CashChanged -= OnCashChanged;
    }

    private void OnCashChanged(int prevValue, int currentValue)
    {
        var delta = currentValue - prevValue;
        if (delta > 0)
        {
            MissionModel.AddValue(delta);
        }
    }
}
