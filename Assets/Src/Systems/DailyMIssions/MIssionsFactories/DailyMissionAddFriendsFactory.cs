using System;
using UnityEngine;

public class DailyMissionAddFriendsFactory : DailyMissionFactoryBase<DailyMissionAddFriendsProcessor>
{
    protected override string Key => MissionKeys.AddFriends;

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        var friendsDataHolder = FriendsDataHolder.Instance; ;
        var additionalFriendsCount = Math.Max(1, (int)Mathf.Lerp(1, MissionConfig.MaxComplexityFactor, complexityMultiplier));
        var totalFriendsCount = friendsDataHolder.InGameFriendsCount;
        var targetFriendsCount = totalFriendsCount + additionalFriendsCount;
        var reward = ChooseReward(complexityMultiplier);
        return new DailyMissionModel(
            Key,
            totalFriendsCount,
            targetFriendsCount,
            totalFriendsCount,
            reward);
    }
}
