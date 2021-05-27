using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationManager", menuName = "Scriptable Objects/Managers/LocalizationManager")]
public class LocalizationManager : ScriptableObject
{
    public static LocalizationManager Instance { get; private set; }

    [SerializeField] private string _localizationUrlRu;

    public Dictionary<string, string> CurrentLocalization { get; private set; }

    public async UniTask<bool> LoadLocalizationAsync()
    {
        var localizationUrl = _localizationUrlRu;
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Russian:
            case SystemLanguage.Belarusian:
                localizationUrl = _localizationUrlRu;
                break;
        }

        var getLocaleOperation = await new WebRequestsSender().GetAsync(localizationUrl);
        if (getLocaleOperation.IsSuccess)
        {
            CurrentLocalization = JsonConvert.DeserializeObject<Dictionary<string, string>>(getLocaleOperation.Result);
        }

        return getLocaleOperation.IsSuccess;
    }

    public string GetLocalization(string key)
    {
        if (CurrentLocalization.TryGetValue(key, out var localization))
        {
            return localization;
        }

        return key;
    }

    private void OnEnable()
    {
        Instance = this;
    }
}

public class LocalizationKeys
{
    public static string HintBottomPanelShelfDescription = "hint_bottom_panel_shelf_description";
    public static string NameShopObjectPrefix = "name_shop_object_";    
    public static string FlyingTextInsufficientFunds = "flying_text_insufficient_funds";
    public static string FlyingTextWrongPlace = "flying_text_wrong_place";
}
