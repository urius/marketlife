using Src.Managers;
using Src.Model;
using Src.Model.Common;

namespace Src.Helpers
{
    public static class DailyBonusRewardHelper
    {
        public static Price GetRewardForDay(int dayNum)
        {
            var baseRewardsConfig = GameConfigManager.Instance.DailyBonusConfig.DailyBonusConfig;

            var dayItemIndex = GetDayItemIndex(dayNum);
            var multiplier = Get5DayMultiplier(dayNum);
            var adsMultiplier = IsRewardDoubled() ? 2 : 1;

            var itemBaseConfig = baseRewardsConfig[dayItemIndex];
            var result = itemBaseConfig.Reward * multiplier * adsMultiplier;
            
            return result;
        }

        public static int GetDayItemIndex(int dayNum)
        {
            return (dayNum - 1) % 5;
        }

        public static int Get5DayMultiplier(int dayNum)
        {
            var dayIndex = dayNum - 1;

            return 1 + dayIndex / 5;
        }

        public static bool IsRewardDoubled()
        {
            return AdvertViewStateModel.Instance.IsDailyBonusAdsWatched();
        }
    }
}