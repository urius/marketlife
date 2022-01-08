using System;
using UnityEngine;

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
