using System.Collections.Generic;

namespace Src.Model.Configs
{
    public class MainConfigDto
    {
        public int GameplayAtlasVersion;
        public int InterfaceAtlasVersion;
        public int AudioBundleVersion;    
        public int RemoveUnwashesRewardExp;    
        public int QuickDeliverPriceGoldPerHour;
        public int ActionDefaultAmount;
        public int ActionDefaultCooldownMinutes;
        public int BillboardUnlockLevel;
        public int GoldToCashConversionRate;
        public int AdvertDefaultWatchesCount;
        public int AdvertWatchCooldownMinutes;

        public Dictionary<string, FriendActionConfigDto> FriendActionsConfig;
        public Dictionary<string, ShelfConfigDto> ShelfsConfig;
        public Dictionary<string, ShopObjectConfigDto> ShopObjectsConfig;
        public Dictionary<string, ShopDecorationConfigDto> FloorsConfig;
        public Dictionary<string, ShopDecorationConfigDto> WallsConfig;
        public Dictionary<string, ShopDecorationConfigDto> WindowsConfig;
        public Dictionary<string, ShopDecorationConfigDto> DoorsConfig;
        public Dictionary<string, ProductConfigDto> ProductsConfig;
        public Dictionary<string, PersonalConfigDto> PersonalConfig;
        public Dictionary<string, float> LevelsConfig;
        public List<UpgradeConfigDto> WarehouseVolumeUpgradesConfig; //Workarond for AOT on WebGL
        public UpgradeConfigDto[] WarehouseSlotsUpgradesConfig;
        public UpgradeConfigDto[] ExtendShopXUpgradesConfig;
        public UpgradeConfigDto[] ExtendShopYUpgradesConfig;
        public List<string> DailyBonusConfig;
        public List<MissionConfigDto> MissionsConfig;
    }



    public class FriendActionConfigDto
    {
        public int action_id;
        public int amount;
        public int cooldown_minutes;
        public int reset_price_gold;
        public bool is_disabled;
    }

    public class PlaceableItemConfigDto
    {
        public string price;
        public int unlock_level;
    }

    public class ShopObjectConfigDto : PlaceableItemConfigDto
    {
        public bool two_sides_mode;
        public int[][] build_matrix;
    }

    public class ShelfConfigDto : ShopObjectConfigDto
    {
        public int part_volume;
        public int parts_num;
    }

    public class ShopDecorationConfigDto : PlaceableItemConfigDto
    {
    }

    public struct ProductConfigDto
    {
        public string key;
        public int unlock_level;
        public string price_per_1000v;
        public int profit_per_1000v;
        public float demand_1000v_per_hour;
        public string deliver;
        public int group;
        public int amount_per_1000v;
    }

    public struct PersonalConfigDto
    {
        public string key;
        public string price_per_cell;
        public int unlock_level;
        public int work_hours;
        public int volume_per_hour;
    }

    public struct UpgradeConfigDto
    {
        public int value;
        public string price;
        public int unlock_level;
        public int unlock_friends;
    }

    public struct MissionConfigDto
    {
        public string key;
        public int unlock_level;
        public int frequency;
        public string reward;
        public int max_complexity_factor;
    }
}