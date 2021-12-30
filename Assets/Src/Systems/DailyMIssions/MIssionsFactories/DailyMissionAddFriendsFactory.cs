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
        return base.CanAdd()
            && IsNewKeyInList();
    }

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        var additionalFriendsCount = Math.Max(1, (int)Mathf.Lerp(1, 5, complexityMultiplier));
        var totalFriendsCount = _friendsDataHolder.InGameFriendsCount;
        var targetFriendsCount = totalFriendsCount + additionalFriendsCount;
        var reward = ChooseReward(complexityMultiplier);
        return new DailyMissionModel(
            Key,
            totalFriendsCount,
            targetFriendsCount,
            totalFriendsCount,
            reward);
    }

    public override DailyMissionProcessorBase CreateProcessor(DailyMissionModel model)
    {
        return new DailyMissionAddFriendsProcessor(model);
    }
}
