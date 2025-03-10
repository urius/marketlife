using Src.Model.Missions;
using Src.Systems.DailyMission.Processors;
using UnityEngine;

namespace Src.Systems.DailyMission.MissionFactories
{
    public class DailyMissionAddGoldFactory : DailyMissionFactoryBase<DailyMissionAddGoldProcessor>
    {
        protected override string Key => MissionKeys.AddGold;

        public override DailyMissionModel CreateModel(float complexityMultiplier)
        {
            var reward = ChooseReward(complexityMultiplier);
            var goldToAdd = (int)Mathf.Max(1, Mathf.Lerp(1, MissionConfig.MaxComplexityFactor, complexityMultiplier));
            return new DailyMissionModel(Key, 0, goldToAdd, 0, reward);
        }
    }
}
