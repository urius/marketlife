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
    public static string HintBottomPanelVisitFriend = "hint_bottom_panel_visit_friend";
    public static string HintBottomPanelShelfDescription = "hint_bottom_panel_shelf_description";
    public static string HintOldGameBonus = "hint_old_game_bonus";
    public static string HintDailyBonus = "hint_daily_bonus";    
    public static string BottomPanelWarehouseEmptySlot = "bottom_panel_warehouse_empty_slot";
    public static string BottomPanelWarehouseEmptySlotHint = "bottom_panel_warehouse_empty_slot_hint";
    public static string BottomPanelWarehouseQuickDeliveryHint = "bottom_panel_warehouse_quick_delivery_hint";    
    public static string BottomPanelFriendsButton = "bottom_panel_friends_button";    
    public static string BottomPanelInteriorButton = "bottom_panel_interior_button";
    public static string BottomPanelManageButton = "bottom_panel_manage_button";    
    public static string FlyingTextInsufficientFunds = "flying_text_insufficient_funds";
    public static string FlyingTextWrongPlace = "flying_text_wrong_place";
    public static string FlyingTextCantSellLastDoor = "flying_text_cant_sell_last_door";
    public static string FlyingTextShelfDoesntHaveFreeSpace = "flying_text_shelf_doesn_have_free_space";
    public static string FlyingTextWarehouseDoesntHaveFreeSpace = "flying_text_warehouse_doesn_have_free_space";
    public static string FlyingTextNothingToTakeFromShelf = "flying_text_shelf_nothing_to_take";
    public static string FlyingTextUnwashAlreadyAdded = "flying_text_unwash_already_added";
    public static string FlyingTextUnwashCantBePlaced = "flying_text_unwash_cant_be_placed";
    public static string CommonYes = "common_yes";
    public static string CommonNo = "common_no";
    public static string CommonSaving = "common_saving";
    public static string CommonMinutesShortFormat = "common_minutes_short_format";
    public static string CommonHoursShortFormat = "common_hours_short_format";
    public static string CommonPersonalNamePrefix = "common_personal_name_";
    public static string CommonContinue = "common_continue";
    public static string CommonUpgradeNameFormat = "common_upgrade_name_format_";
    public static string CommonPayCurrencyNamePlural = "common_pay_currency_plural_";    
    public static string HintTopPanelExpFomat = "hint_top_panel_exp_format";
    public static string HintTopPanelMoodFormat = "hint_top_panel_mood_format";    
    public static string PopupRemoveObjectTitle = "popup_remove_object_title";
    public static string PopupRemoveObjectText = "popup_remove_object_text";
    public static string PopupRemoveProductTitle = "popup_remove_product_title";
    public static string PopupRemoveProductText = "popup_remove_product_text";
    public static string PopupShelfContentTitle = "popup_shelf_content_title";
    public static string PopupShelfContentProductDescriptionFormat = "popup_shelf_content_product_description_format";
    public static string PopupShelfContentProductEmpty = "popup_shelf_content_product_empty";
    public static string PopupShelfContentProductTakeFromWarehouse = "popup_shelf_content_product_take_from_warehouse";
    public static string PopupOrderProductItemDescriptionText = "popup_order_product_item_description_text";
    public static string PopupWarehouseTitle = "popup_warehouse_title";
    public static string PopupWarehouseItemAmountFormat = "popup_warehouse_item_amount_format";
    public static string PopupWarehouseItemHintFormat = "popup_warehouse_item_hint_format";
    public static string PopupUpgradesTitle = "popup_upgrades_title";
    public static string PopupUpgradesWarehouseTab = "popup_upgrades_warehouse_tab_name";
    public static string PopupUpgradesExpandTab = "popup_upgrades_expand_tab_name";
    public static string PopupUpgradesPersonalTab = "popup_upgrades_personal_tab_name";
    public static string PopupUpgradesPersonalDescriptionPrefix = "popup_upgrades_personal_description_";
    public static string PopupUpgradesUpgradeDescriptionPrefix = "popup_upgrades_upgrade_description_";
    public static string PopupUpgradesPersonalIsWorkingFormat = "popup_upgrades_personal_is_working_format";
    public static string PopupUpgradesUnlockUpgradeCaption = "popup_upgrades_unlock_upgrade_caption";
    public static string PopupUpgradesUnlockUpgradeFriendsFormat = "popup_upgrades_unlock_upgrade_friends_format";
    public static string PopupUpgradesUnlockUpgradeLevelFormat = "popup_upgrades_unlock_upgrade_level_format";
    public static string PopupUpgradesMaxLevel = "popup_upgrades_max_level";
    public static string PopupOfflineReportTitleFormat = "popup_offline_report_title_format";
    public static string PopupOfflineReportProfitTab = "popup_offline_report_profit_tab_name";
    public static string PopupOfflineReportPersonalTab = "popup_offline_report_personal_tab_name";
    public static string PopupOfflineReportActivityTab = "popup_offline_report_activity_tab_name";
    public static string PopupOfflineReportProfitTabOverallProfit = "popup_offline_report_profit_tab_overall_profit";
    public static string PopupLevelUpTitle = "popup_levelup_title";
    public static string PopupLevelUpMessage = "popup_levelup_message";
    public static string PopupLevelUpNewElements = "popup_levelup_new_elements";
    //public static string PopupLevelUpNewProducts = "popup_levelup_new_products";
    //public static string PopupLevelUpNewShelfs = "popup_levelup_new_shelfs";
    //public static string PopupLevelUpNewDecor = "popup_levelup_new_decor";
    public static string PopupLevelUpFloorFormat = "popup_levelup_floor_format";
    public static string PopupLevelUpWallFormat = "popup_levelup_wall_format";
    public static string PopupLevelUpWindowFormat = "popup_levelup_window_format";
    public static string PopupLevelUpDoorFormat = "popup_levelup_door_format";
    //public static string PopupLevelUpNewPersonal = "popup_levelup_new_personal";
    //public static string PopupLevelUpNewUpgrades = "popup_levelup_new_upgrades";    
    public static string PopupBankGoldTab = "popup_bank_gold_tab";
    public static string PopupBankCashTab = "popup_bank_cash_tab";
    public static string PopupBankTitle = "popup_bank_title";
    public static string PopupErrorTitle = "popup_error_title";
    public static string PopupErrorOk = "popup_error_ok";
    public static string PopupErrorLoadDataCommon = "popup_error_load_data_common";
    public static string PopupCompensationTitle = "popup_compensation_title";
    public static string PopupCompensationMessage = "popup_compensation_message";    

    public static string TutorialTitleDefault = "tutorial_title_default";
    public static string TutorialMessagePrefix = "tutorial_message_";
    public static string TutorialButtonPrefix = "tutorial_button_";

    public static string NameShopObjectPrefix = "name_shop_object_";
    public static string NameProductGroupIdPrefix = "name_product_group_id_";
    public static string NameProductIdPrefix = "name_product_id_";
}
