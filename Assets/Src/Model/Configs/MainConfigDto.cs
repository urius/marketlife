using System.Collections.Generic;

public class MainConfigDto
{
    public int GameplayAtlasVersion;
    public int InterfaceAtlasVersion;
    public int AudioBundleVersion;    
    public int AutoPlacePriceGold;
    public int RemoveUnwashesRewardExp;    
    public int QuickDeliverPriceGoldPerMinute;
    public int ActionDefaultAmount;
    public int ActionDefaultCooldownMinutes;
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
    public string price;
    public int unlock_level;
    public int work_hours;
}

public struct UpgradeConfigDto
{
    public int value;
    public string price;
    public int unlock_level;
    public int unlock_friends;
}