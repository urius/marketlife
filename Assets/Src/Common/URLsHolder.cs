using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "URLsHolder", menuName = "Scriptable Objects/URLsHolder")]
public class URLsHolder : ScriptableObject
{
    public static URLsHolder Instance { get; private set; }

    [SerializeField] private string _getTimeURL = "https://devman.ru/marketVK/getTime.php";
    public string GetTimeURL => _getTimeURL;
    [SerializeField] private string _getDataURL = "https://devman.ru/marketVK/dataProvider.php?command=get_data&id={0}";
    public string GetDataURL => _getDataURL;

    private void OnEnable()
    {
        Instance = this;
    }
}
