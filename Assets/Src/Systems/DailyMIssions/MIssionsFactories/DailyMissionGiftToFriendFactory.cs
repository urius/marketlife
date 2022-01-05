using UnityEngine;

public class DailyMissionGiftToFriendFactory : DailyMissionFactoryBase<DailyMissionGiftToFriendProcessor>
{
    protected override string Key => MissionKeys.GiftToFriend;

    public override bool CanAdd()
    {
        var friendsDataHolder = FriendsDataHolder.Instance;
        var inGameFriendsCount = friendsDataHolder.InGameFriendsCount;
        return base.CanAdd()
            && inGameFriendsCount > 0;
    }

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        var friendsDataHolder = FriendsDataHolder.Instance;
        var inGameFriendsCount = friendsDataHolder.InGameFriendsCount;
        if (inGameFriendsCount > 0)
        {
            var targetFriendsCount = (int)Mathf.Max(1, Mathf.Lerp(1, inGameFriendsCount, complexityMultiplier));
            var reward = ChooseReward(complexityMultiplier);
            return new DailyMissionModel(Key, 0, targetFriendsCount, 0, reward);
        }
        else
        {
            return null;
        }
    }
}
