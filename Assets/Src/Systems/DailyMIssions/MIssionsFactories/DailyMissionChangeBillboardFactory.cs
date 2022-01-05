public class DailyMissionChangeBillboardFactory : DailyMissionFactoryBase<DailyMissionChangeBillboardProcessor>
{
    protected override string Key => MissionKeys.ChangeBillboard;

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        return new DailyMissionModel(Key, 0, 1, 0, ChooseReward(complexityMultiplier));
    }
}
