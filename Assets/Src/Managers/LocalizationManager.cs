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
    public static string BottomPanelWarehouseEmptySlot = "bottom_panel_warehouse_empty_slot";
    public static string BottomPanelWarehouseEmptySlotHint = "bottom_panel_warehouse_empty_slot_hint";
    public static string BottomPanelWarehouseQuickDeliveryHint = "bottom_panel_warehouse_quick_delivery_hint";
    public static string FlyingTextInsufficientFunds = "flying_text_insufficient_funds";
    public static string FlyingTextWrongPlace = "flying_text_wrong_place";
    public static string FlyingTextCantSellLastDoor = "flying_text_cant_sell_last_door";
    public static string FlyingTextShelfDoesntHaveFreeSpace = "flying_text_shelf_doesn_have_free_space";    
    public static string CommonYes = "common_yes";
    public static string CommonNo = "common_no";
    public static string PopupRemoveObjectTitle = "popup_remove_object_title";
    public static string PopupRemoveObjectText = "popup_remove_object_text";
    public static string PopupOrderProductItemDescriptionText = "popup_order_product_item_description_text";
    public static string NameShopObjectPrefix = "name_shop_object_";
    public static string NameProductGroupIdPrefix = "name_product_group_id_";
    public static string NameProductIdPrefix = "name_product_id_";
}
