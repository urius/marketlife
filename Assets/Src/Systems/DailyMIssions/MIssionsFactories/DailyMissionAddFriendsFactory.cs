using System;
using UnityEngine;

public class DailyMissionAddFriendsFactory : DailyMissionFactoryBase
{
    private readonly FriendsDataHolder _friendsDataHolder;

    public DailyMissionAddFriendsFactory()
    {
        _friendsDataHolder = FriendsDataHolder.Instance;
    }

    protected override string Key => MissionKeys.AddFriends;

    public override bool CanAdd()
    {
        return true;
    }

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        var additionalFriendsCount = Math.Max(1, (int)Mathf.Lerp(1, 5, complexityMultiplier));
        var totalFriendsCount = _friendsDataHolder.InGameFriendsCount;
        var targetFriendsCount = totalFriendsCount + additionalFriendsCount;
        return new DailyMissionModel(
            Key,
            totalFriendsCount,
            targetFriendsCount,
            ChooseReward(MissionConfig.RewardConfigs, complexityMultiplier));
    }

    public override DailyMissionProcessorBase CreateProcessor(DailyMissionModel model)
    {
        return new DailyMissionAddFriendsProcessor(model);
    }
}
