using Src.Model;
using Src.Model.Missions;
using Src.Systems.DailyMIssions.Processors;
using UnityEngine;

namespace Src.Systems.DailyMIssions.MIssionsFactories
{
    public class DailyMissionGiftToFriendFactory : DailyMissionFactoryBase<DailyMissionGiftToFriendProcessor>
    {
        protected override string Key => MissionKeys.GiftToFriend;

        public override bool CanAdd()
        {
            return base.CanAdd()
                   && ExtraCanAddCondition();
        }

        public override DailyMissionModel CreateModel(float complexityMultiplier)
        {
            var friendsDataHolder = FriendsDataHolder.Instance;
            var activeFriendsCount = friendsDataHolder.InGameActiveFriendsCount;
            if (activeFriendsCount > 0)
            {
                var targetFriendsCount = (int)Mathf.Max(1, Mathf.Lerp(1, activeFriendsCount, complexityMultiplier));
                var reward = ChooseReward(complexityMultiplier);
                return new DailyMissionModel(Key, 0, targetFriendsCount, 0, reward);
            }
            else
            {
                return null;
            }
        }

        private bool ExtraCanAddCondition()
        {
            var friendsDataHolder = FriendsDataHolder.Instance;
            var activeFriendsCount = friendsDataHolder.InGameActiveFriendsCount;
            return activeFriendsCount > 0;
        }
    }
}
