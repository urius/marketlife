using System;
using UnityEngine;

[CreateAssetMenu(fileName = "URLsHolder", menuName = "Scriptable Objects/URLsHolder")]
public class URLsHolder : ScriptableObject
{
    public static URLsHolder Instance { get; private set; }

    [SerializeField] private string _basePath = "https://devman.ru/marketVK/unity/";
    [SerializeField] private string _getTimePostfix = "DataProvider.php?command=get_time";
    public string GetTimeURL => _basePath + _getTimePostfix;
    [SerializeField] private string _mainConfigPostfixFormat = "GameConfigs/MainConfig{0}.json";
    public string MainConfigUrlFormat => _basePath + _mainConfigPostfixFormat;
    [SerializeField] private string _getDataPostfix = "DataProvider.php?command=get_data&id={0}";
    public string GetDataURL => _basePath + _getDataPostfix;
    [SerializeField] private string _getVisitTimesPostfix = "DataProvider.php?command=get_users_last_visit&ids={0}";
    public string GetVisitTimesURL => _basePath + _getVisitTimesPostfix;
    [SerializeField] private string _getFriendDataOldURL = "https://devman.ru/marketVK/dataProvider.php?command=get_friend_data&id={0}";
    public string GetFriendDataOldURL => _getFriendDataOldURL;
    [SerializeField] private string _saveDataPostfix = "DataProvider.php?command=save_data&id={0}";
    public string SaveDataURL => _basePath + _saveDataPostfix;
    [SerializeField] private string _saveExternalDataPostfix = "DataProvider.php?command=save_external_data&id={0}";
    public string SaveExternalDataURL => _basePath + _saveExternalDataPostfix;
    [SerializeField] private string _saveLeaderboardDataPostfix = "DataProvider.php?command=save_leaderboard_data&id={0}";
    public string SaveLeaderboardDataURL => _basePath + _saveLeaderboardDataPostfix;
    [SerializeField] private string _resetNotificationsPostfix = "vk/VKNotificationsProcessor.php?command=reset&id={0}";
    public string ResetNotificationsURL => _basePath + _resetNotificationsPostfix;
    [SerializeField] private string _addNotificationsPostfix = "vk/VKNotificationsProcessor.php?command=add&ids={0}&type={1}";
    public string AddNotificationsURL => _basePath + _addNotificationsPostfix;
    [SerializeField] private string _vkBankDataPostfix = "vk/VKBank.json";

    [Space]
    [SerializeField] private string _debugMainConfigUrl = "https://devman.ru/marketVK/unity/GameConfigs/MainConfig_Debug.json";
    public string DebugMainConfigUrl => _debugMainConfigUrl;

    public string GetBankDataURL(SocialType platformType)
    {
        switch (platformType)
        {
            case SocialType.Undefined:
            case SocialType.VK:
                return _basePath + _vkBankDataPostfix;
            default:
                throw new ArgumentException($"{nameof(URLsHolder)}::{nameof(GetBankDataURL)}: unsupported {nameof(platformType)} {platformType}");
        }
    }

    private void OnEnable()
    {
        Instance = this;
    }
}
