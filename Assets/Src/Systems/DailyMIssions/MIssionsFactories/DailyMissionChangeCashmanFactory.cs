using Src.Model.Missions;
using Src.Systems.DailyMIssions.Processors;

namespace Src.Systems.DailyMIssions.MIssionsFactories
{
    public class DailyMissionChangeCashmanFactory : DailyMissionFactoryBase<DailyMissionChangeCashmanProcessor>
    {
        protected override string Key => MissionKeys.ChangeCashman;

        public override DailyMissionModel CreateModel(float complexityMultiplier)
        {
            return new DailyMissionModel(Key, 0, 1, 0, ChooseReward(complexityMultiplier));
        }
    }
}
