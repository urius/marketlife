using System;
using UnityEngine;

[CreateAssetMenu(fileName = "URLsHolder", menuName = "Scriptable Objects/URLsHolder")]
public class URLsHolder : ScriptableObject
{
    public static URLsHolder Instance { get; private set; }

    [SerializeField] private string _getTimeURL = "https://devman.ru/marketVK/unity/DataProvider.php?command=get_time";
    public string GetTimeURL => _getTimeURL;
    //https://devman.ru/marketVK/dataProvider.php?command=get_data&id={0}
    //https://devman.ru/marketVK/unity/DataProvider.php?command=get_data&id={0}
    [SerializeField] private string _getDataURL = "https://devman.ru/marketVK/unity/DataProvider.php?command=get_data&id={0}";
    public string GetDataURL => _getDataURL;
    [SerializeField] private string _getFriendDataOldURL = "https://devman.ru/marketVK/dataProvider.php?command=get_friend_data&id={0}";
    public string GetFriendDataOldURL => _getFriendDataOldURL;
    [SerializeField] private string _saveDataURL = "https://devman.ru/marketVK/unity/DataProvider.php?command=save_data&id={0}";
    public string SaveDataURL => _saveDataURL;
    [SerializeField] private string _saveExternalDataURL = "https://devman.ru/marketVK/unity/DataProvider.php?command=save_external_data&id={0}";
    public string SaveExternalDataURL => _saveExternalDataURL;
    [SerializeField] private string _vkBankDataURL = "https://devman.ru/marketVK/unity/vk/VKBank.json";

    public string GetBankDataURL(SocialType platformType)
    {
        switch (platformType)
        {
            case SocialType.Undefined:
            case SocialType.VK:
                return _vkBankDataURL;
            default:
                throw new ArgumentException($"{nameof(URLsHolder)}::{nameof(GetBankDataURL)}: unsupported {nameof(platformType)} {platformType}");
        }
    }

    private void OnEnable()
    {
        Instance = this;
    }
}
