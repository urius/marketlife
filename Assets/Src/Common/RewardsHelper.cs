using UnityEngine;

namespace Src.Common
{
    public static class RewardsHelper
    {
        public static int GetGoldRewardForOfflineShare()
        {
            var userModel = PlayerModelHolder.Instance.UserModel;
            var gameStateModel = GameStateModel.Instance;
            var defaultReward = GameConfigManager.Instance.MainConfig.DefaultShareOfflineReportRewardGold;
            
            var deltaHours = (gameStateModel.ServerTime - userModel.StatsData.LastVisitTimestamp) / 3600;

            var result = defaultReward;
            if (deltaHours > 0)
            {
                result = Mathf.Clamp(deltaHours * defaultReward / 6, defaultReward, defaultReward * 8);
            }

            return result;
        }
    }
}