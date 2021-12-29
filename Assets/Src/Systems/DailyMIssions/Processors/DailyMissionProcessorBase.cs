public abstract class DailyMissionProcessorBase
{
    public readonly DailyMissionModel MissionModel;

    public DailyMissionProcessorBase(DailyMissionModel missionModel)
    {
        MissionModel = missionModel;
    }

    public abstract void Activate();
    public abstract void Deactivate();
}
