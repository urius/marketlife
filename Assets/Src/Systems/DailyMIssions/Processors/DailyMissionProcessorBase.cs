public abstract class DailyMissionProcessorBase
{
    public readonly DailyMissionModel MissionModel;

    public DailyMissionProcessorBase(DailyMissionModel missionModel)
    {
        MissionModel = missionModel;
    }

    public abstract void Start();
    public abstract void Stop();
}
