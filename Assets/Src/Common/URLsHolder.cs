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
    [SerializeField] private string _saveDataURL = "https://devman.ru/marketVK/unity/DataProvider.php?command=save_data&id={0}";
    public string SaveDataURL => _saveDataURL;
    [SerializeField] private string _saveExternalDataURL = "https://devman.ru/marketVK/unity/DataProvider.php?command=save_external_data&id={0}";
    public string SaveExternalDataURL => _saveExternalDataURL;

    private void OnEnable()
    {
        Instance = this;
    }
}
