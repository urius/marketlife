using System;

namespace Src.Common
{
    public class Urls
    {
        public static readonly Urls Instance = new();
        
        public const string BaseHostUrl = "https://twin-pixel.ru";

        private const string GetTimePostfix = "/DataProvider.php?command=get_time";
        private const string GetDataPostfix = "/DataProvider.php?command=get_data&id={0}";
        private const string MainConfigPostfixFormat = "/GameConfigs/MainConfig{0}.json";
        private const string GetVisitTimesPostfix = "/DataProvider.php?command=get_users_last_visit&ids={0}";
        private const string SaveExternalDataPostfix = "/DataProvider.php?command=save_external_data&id={0}";
        private const string SaveLeaderboardDataPostfix = "/DataProvider.php?command=save_leaderboard_data&id={0}";
        private const string ResetNotificationsPostfix = "/vk/VKNotificationsProcessor.php?command=reset&id={0}";
        private const string VkBankDataPostfix = "/vk/VKBank.json";
        private const string GetLeaderboardsPostfix = "/DataProvider.php?command=get_leaderboards&id={0}";
        private const string AddNotificationsPostfix = "/vk/VKNotificationsProcessor.php?command=add&ids={0}&type={1}";
        private const string SaveDataPostfix = "/DataProvider.php?command=save_data&id={0}";

        private static readonly string BasePath = $"{BaseHostUrl}/marketVK/unity";
        
        public static readonly string AssetBundlesBaseUrl = $"{BasePath}/AssetBundles";

        
        public static string GetTimeURL => BasePath + GetTimePostfix;
        public static string MainConfigUrlFormat => BasePath + MainConfigPostfixFormat;
        public static string GetDataURL => BasePath + GetDataPostfix;
        public static string GetVisitTimesURL => BasePath + GetVisitTimesPostfix;
        public static string GetFriendDataOldURL => $"{BaseHostUrl}/marketVK/dataProvider.php?command=get_friend_data&id={0}";
        public static string SaveDataURL => BasePath + SaveDataPostfix;
        public static string SaveExternalDataURL => BasePath + SaveExternalDataPostfix;
        public static string SaveLeaderboardDataURL => BasePath + SaveLeaderboardDataPostfix;
        public string ResetNotificationsURL => BasePath + ResetNotificationsPostfix;
        public static string AddNotificationsURL => BasePath + AddNotificationsPostfix;
        public static string GetLeaderboardsURL => BasePath + GetLeaderboardsPostfix;

        public string DebugMainConfigUrl { get; } = $"{BaseHostUrl}/marketVK/unity/GameConfigs/MainConfig_Debug.json";

        public string GetBankDataURL(SocialType platformType)
        {
            switch (platformType)
            {
                case SocialType.Undefined:
                case SocialType.VK:
                    return BasePath + VkBankDataPostfix;
                default:
                    throw new ArgumentException($"{nameof(Urls)}::{nameof(GetBankDataURL)}: unsupported {nameof(platformType)} {platformType}");
            }
        }
    }
}