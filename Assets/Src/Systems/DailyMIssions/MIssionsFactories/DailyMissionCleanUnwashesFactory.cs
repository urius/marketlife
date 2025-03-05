using System;
using Src.Common;
using Src.Model;
using Src.Model.Missions;
using Src.Systems.DailyMIssions.Processors;
using UnityEngine;

namespace Src.Systems.DailyMIssions.MIssionsFactories
{
    public class DailyMissionCleanUnwashesFactory : DailyMissionFactoryBase<DailyMissionCleanUnwashesProcessor>
    {
        protected override string Key => MissionKeys.CleanUnwashes;

        public override bool CanAdd()
        {
            var currentHour = DateTimeHelper.GetDateTimeByUnixTimestamp(GameStateModel.Instance.ServerTime).Hour;
            return base.CanAdd()
                   && currentHour < 24;
        }

        public override DailyMissionModel CreateModel(float complexityMultiplier)
        {
            var currentHour = new DateTime().Hour;
            var restHoursBeforeNextDay = 24 - currentHour;
            var targetValue = (int)Mathf.Max(1, Mathf.Lerp(1, Mathf.Max(1, restHoursBeforeNextDay), complexityMultiplier));
            var reward = ChooseReward(complexityMultiplier);
            return new DailyMissionModel(Key, 0, targetValue, 0, reward);
        }
    }
}
