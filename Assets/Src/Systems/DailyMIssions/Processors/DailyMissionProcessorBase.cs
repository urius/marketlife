public abstract class DailyMissionProcessorBase
{
    public DailyMissionModel MissionModel { get; private set; }

    public virtual void SetupMissionModel(DailyMissionModel missionModel)
    {
        MissionModel = missionModel;
    }

    public abstract void Start();
    public abstract void Stop();
}
