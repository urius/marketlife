using UnityEngine;

public class DailyMissionVisitFriendsFactory : DailyMissionFactoryBase<DailyMissionVisitFriendsProcessor>
{
    protected override string Key => MissionKeys.VisitFriends;

    public override bool CanAdd()
    {
        return base.CanAdd()
            && ExtraCanAddCondition();
    }

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        var friendsDataHolder = FriendsDataHolder.Instance;
        var activeFriendsCount = friendsDataHolder.InGameActiveFriendsCount;
        var targetValue = (int)Mathf.Max(1, Mathf.Lerp(1, activeFriendsCount, complexityMultiplier));
        var reward = ChooseReward(complexityMultiplier);
        return new DailyMissionModel(Key, 0, targetValue, 0, reward);
    }

    private bool ExtraCanAddCondition()
    {
        var friendsDataHolder = FriendsDataHolder.Instance;
        var activeFriendsCount = friendsDataHolder.InGameActiveFriendsCount;
        return activeFriendsCount > 0;
    }
}
