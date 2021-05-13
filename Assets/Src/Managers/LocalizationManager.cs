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

public class LocalizationDto
{
    public string hint_bottom_panel_interior_button;
    public string hint_bottom_panel_manage_button;
    public string hint_bottom_panel_exit_interior_node_button;
    public string hint_bottom_panel_floors_button;
    public string hint_bottom_panel_doors_button;
    public string hint_bottom_panel_shop_objects_button;
    public string hint_bottom_panel_walls_button;
    public string hint_bottom_panel_windows_button;
}
