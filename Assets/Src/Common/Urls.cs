using Src.Model;
using UnityEngine;

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
        private const string GetLeaderboardsPostfix = "/DataProvider.php?command=get_leaderboards&id={0}";
        private const string AddNotificationsPostfix = "/vk/VKNotificationsProcessor.php?command=add&ids={0}&type={1}";
        private const string SaveDataPostfix = "/DataProvider.php?command=save_data&id={0}";
        private const string SetUserInfoPostfix = "/DataProvider.php?command=set_user_info&id={0}&name={1}&picture_url={2}";
        private const string GetUsersInfoPostfix = "/DataProvider.php?command=get_users_info";
        
        private static string _basePath = $"{BaseHostUrl}/marketVK/unity";

        public static void UpdateBasePathPostfix(string basePathPostfix)
        {
            _basePath = $"{BaseHostUrl}{basePathPostfix}";
        }

        public static string GetTimeURL => _basePath + GetTimePostfix;
        public static string MainConfigUrlFormat => _basePath + MainConfigPostfixFormat;
        public static string GetDataURL => _basePath + GetDataPostfix;
        public static string GetVisitTimesURL => _basePath + GetVisitTimesPostfix;
        public static string GetFriendDataOldURL => $"{BaseHostUrl}/marketVK/dataProvider.php?command=get_friend_data&id={0}";
        public static string SaveDataURL => _basePath + SaveDataPostfix;
        public static string SaveExternalDataURL => _basePath + SaveExternalDataPostfix;
        public static string SaveLeaderboardDataURL => _basePath + SaveLeaderboardDataPostfix;
        public string ResetNotificationsURL => _basePath + ResetNotificationsPostfix;
        public static string AddNotificationsURL => _basePath + AddNotificationsPostfix;
        public static string GetLeaderboardsURL => _basePath + GetLeaderboardsPostfix;
        public static string SetUserInfoURL => _basePath + SetUserInfoPostfix;
        public static string GetUsersInfoURL => _basePath + GetUsersInfoPostfix;
        
        private static string AssetBundlesBaseUrl => $"{_basePath}/AssetBundles";
        private static string AssetBundlesWebGlUrl => $"{AssetBundlesBaseUrl}/WebGL";
        private static string AssetBundlesOsxUrl => $"{AssetBundlesBaseUrl}/OSX";
        private static string AssetBundlesAndroidUrl => $"{AssetBundlesBaseUrl}/Android";
        private static string LocalizationUrlFormat =>
            $"{_basePath}/GameConfigs/localizations/localization_{{0}}.json";

        public string DebugMainConfigUrl { get; } = $"{BaseHostUrl}/marketVK/unity/GameConfigs/MainConfig_Debug.json";

        public string GetBankDataURL(SocialType platformType)
        {
            switch (platformType)
            {
                case SocialType.VK:
                    return _basePath + "/vk/VKBank.json";
                case SocialType.YG:
                case SocialType.Undefined:
                default:
                    return _basePath + "/GameConfigs/BankConfig.json";
                    //throw new ArgumentException($"{nameof(Urls)}::{nameof(GetBankDataURL)}: unsupported {nameof(platformType)} {platformType}");
            }
        }

        public static string GetAssetBundleUrl()
        {
            return Application.platform switch
            {
                RuntimePlatform.OSXEditor => AssetBundlesOsxUrl,
                RuntimePlatform.WebGLPlayer => AssetBundlesWebGlUrl,
                RuntimePlatform.Android => AssetBundlesAndroidUrl,
                _ => throw new System.Exception($"AssetBundlesLoader {nameof(GetAssetBundleUrl)}: Unsupported platform type: {Application.platform}"),
            };
        }
    }
}