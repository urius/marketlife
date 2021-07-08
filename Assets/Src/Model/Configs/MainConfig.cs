using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainConfig :
    IProductsConfig,
    IShelfsConfig,
    IFloorsConfig,
    IWallsConfig,
    IWindowsConfig,
    IDoorsConfig,
    IPersonalConfig,
    IUpgradesConfig,
    ILevelsConfig
{
    public readonly int GameplayAtlasVersion;
    public readonly int InterfaceAtlasVersion;
    public readonly int AutoPlacePriceGold;
    public readonly int RemoveUnwashesRewardExp;
    public readonly int QuickDeliverPriceGoldPerMinute;

    public readonly Dictionary<string, ProductConfig> ProductsConfig;
    public readonly Dictionary<string, ItemConfig<ShelfConfigDto>> ShelfsConfig;
    public readonly Dictionary<string, ItemConfig<ShopObjectConfigDto>> ShopObjectsConfig;
    public readonly Dictionary<string, ItemConfig<ShopDecorationConfigDto>> FloorsConfig;
    public readonly Dictionary<string, ItemConfig<ShopDecorationConfigDto>> WallsConfig;
    public readonly Dictionary<string, ItemConfig<ShopDecorationConfigDto>> WindowsConfig;
    public readonly Dictionary<string, ItemConfig<ShopDecorationConfigDto>> DoorsConfig;
    public readonly Dictionary<string, PersonalConfig> PersonalConfig;
    public readonly UpgradeConfig[] WarehouseVolumeUpgradesConfig;
    public readonly UpgradeConfig[] WarehouseSlotsUpgradesConfig;
    public readonly UpgradeConfig[] ExtendShopXUpgradesConfig;
    public readonly UpgradeConfig[] ExtendShopYUpgradesConfig;

    private readonly float[] _levelsConfig;

    public MainConfig(
        int gameplayAtlasVersion,
        int interfaceAtlasVersion,
        int autoPlacePriceGold,
        int removeUnwashesRewardExp,
        int quickDeliverPriceGoldPerMinute,
        Dictionary<string, ProductConfig> productsConfig,
        Dictionary<string, ItemConfig<ShelfConfigDto>> shelfsConfig,
        Dictionary<string, ItemConfig<ShopObjectConfigDto>> shopObjectsConfig,
        Dictionary<string, ItemConfig<ShopDecorationConfigDto>> floorsConfig,
        Dictionary<string, ItemConfig<ShopDecorationConfigDto>> wallsConfig,
        Dictionary<string, ItemConfig<ShopDecorationConfigDto>> windowsConfig,
        Dictionary<string, ItemConfig<ShopDecorationConfigDto>> doorsConfig,
        Dictionary<string, PersonalConfig> personalConfig,
        UpgradeConfig[] warehouseVolumeUpgradesConfig,
        UpgradeConfig[] warehouseSlotsUpgradesConfig,
        UpgradeConfig[] extendShopXUpgradesConfig,
        UpgradeConfig[] extendShopYUpgradesConfig,
        float[] levelsConfig)
    {
        GameplayAtlasVersion = gameplayAtlasVersion;
        InterfaceAtlasVersion = interfaceAtlasVersion;
        AutoPlacePriceGold = autoPlacePriceGold;
        RemoveUnwashesRewardExp = removeUnwashesRewardExp;
        QuickDeliverPriceGoldPerMinute = quickDeliverPriceGoldPerMinute;
        ProductsConfig = productsConfig;
        ShelfsConfig = shelfsConfig;
        ShopObjectsConfig = shopObjectsConfig;
        FloorsConfig = floorsConfig;
        WallsConfig = wallsConfig;
        WindowsConfig = windowsConfig;
        DoorsConfig = doorsConfig;
        PersonalConfig = personalConfig;
        WarehouseVolumeUpgradesConfig = warehouseVolumeUpgradesConfig;
        WarehouseSlotsUpgradesConfig = warehouseSlotsUpgradesConfig;
        ExtendShopXUpgradesConfig = extendShopXUpgradesConfig;
        ExtendShopYUpgradesConfig = extendShopYUpgradesConfig;
        _levelsConfig = levelsConfig;
    }

    public UpgradeConfig GetCurrentUpgradeForValue(UpgradeType upgradeType, int value)
    {
        return GetCurrentUpgradeForValueInternal(GetUpgradesFor(upgradeType), value);
    }

    public UpgradeConfig GetNextUpgradeForValue(UpgradeType upgradeType, int value)
    {
        return GetNextUpgradeForValueInternal(GetUpgradesFor(upgradeType), value);
    }

    public IEnumerable<ItemConfig<ShelfConfigDto>> GetShelfConfigsForLevel(int level)
    {
        return GetConfigsForLevel(ShelfsConfig, level);
    }

    public ItemConfig<ShelfConfigDto> GetShelfConfigByNumericId(int numericId)
    {
        return ShelfsConfig[$"s_{numericId}"];
    }

    public IEnumerable<ProductConfig> GetProductConfigsForLevel(int level)
    {
        foreach (var kvp in ProductsConfig)
        {
            if (kvp.Value.UnlockLevel <= level)
            {
                yield return kvp.Value;
            }
        }
    }

    public ProductConfig GetProductConfigByNumericId(int numericId)
    {
        return ProductsConfig[$"p{numericId}"];
    }

    public IEnumerable<PersonalConfig> GetPersonalConfigsForLevel(int level)
    {
        foreach (var kvp in PersonalConfig)
        {
            if (kvp.Value.UnlockLevel <= level)
            {
                yield return kvp.Value;
            }
        }
    }

    public PersonalConfig GetPersonalConfigByIds(PersonalType typeId, int numericId)
    {
        foreach (var kvp in PersonalConfig)
        {
            if (kvp.Value.TypeId == typeId && kvp.Value.NumericId == numericId)
            {
                return kvp.Value;
            }
        }
        return null;
    }

    public PersonalConfig GetPersonalConfigByStringId(string stringId)
    {
        return PersonalConfig[stringId];
    }

    public ProductConfig GetProductConfigByKey(string key)
    {
        return ProductsConfig.First(c => c.Key == key).Value;
    }

    public ItemConfig<ShopObjectConfigDto> GetCashDeskConfigByNumericId(int cashDeskNumericId)
    {
        return ShopObjectsConfig[$"cd_{cashDeskNumericId}"];
    }

    public IEnumerable<ItemConfig<ShopDecorationConfigDto>> GetFloorsConfigsForLevel(int level)
    {
        return GetConfigsForLevel(FloorsConfig, level);
    }

    public ItemConfig<ShopDecorationConfigDto> GetDecorationConfigBuNumericId(ShopDecorationObjectType decorationType, int numericId)
    {
        switch (decorationType)
        {
            case ShopDecorationObjectType.Floor:
                return GetFloorConfigByNumericId(numericId);
            case ShopDecorationObjectType.Wall:
                return GetWallConfigByNumericId(numericId);
            case ShopDecorationObjectType.Window:
                return GetWindowConfigByNumericId(numericId);
            case ShopDecorationObjectType.Door:
                return GetDoorConfigByNumericId(numericId);
            default:
                throw new ArgumentException($"GetDecorationConfigBuNumericId: unsupported {nameof(ShopDecorationObjectType)} '{decorationType}'");
        }
    }

    public ItemConfig<ShopDecorationConfigDto> GetFloorConfigByNumericId(int floorNumericId)
    {
        return FloorsConfig[$"f_{floorNumericId}"];
    }

    public IEnumerable<ItemConfig<ShopDecorationConfigDto>> GetWallsConfigsForLevel(int level)
    {
        return GetConfigsForLevel(WallsConfig, level);
    }

    public ItemConfig<ShopDecorationConfigDto> GetWallConfigByNumericId(int wallNumericId)
    {
        return WallsConfig[$"w_{wallNumericId}"];
    }

    public IEnumerable<ItemConfig<ShopDecorationConfigDto>> GetWindowsConfigsForLevel(int level)
    {
        return GetConfigsForLevel(WindowsConfig, level);
    }

    public ItemConfig<ShopDecorationConfigDto> GetWindowConfigByNumericId(int windowNumericId)
    {
        return WindowsConfig[$"wnd_{windowNumericId}"];
    }

    public IEnumerable<ItemConfig<ShopDecorationConfigDto>> GetDoorsConfigsForLevel(int level)
    {
        return GetConfigsForLevel(DoorsConfig, level);
    }

    public ItemConfig<ShopDecorationConfigDto> GetDoorConfigByNumericId(int doorNumericId)
    {
        return DoorsConfig[$"d_{doorNumericId}"];
    }

    public int GetExpByLevel(int level)
    {
        if (level <= 0) return 0;
        if (_levelsConfig.Length > level)
        {
            return (int)_levelsConfig[level];
        }
        else
        {
            var multiplier = _levelsConfig[0];
            var lastDefinedLevelIndex = _levelsConfig.Length - 1;
            var extraLevels = level - lastDefinedLevelIndex;
            var pow = Math.Pow(multiplier, extraLevels);
            var resultRaw = (int)(_levelsConfig[lastDefinedLevelIndex] * pow);
            return (int)(1000 * Math.Ceiling(resultRaw * 0.001f));
        }
    }

    public int GetLevelByExp(int exp)
    {
        var result = 1;
        while (GetExpByLevel(result + 1) <= exp)
        {
            result++;
        }
        return result;
    }

    private IEnumerable<ItemConfig<T>> GetConfigsForLevel<T>(Dictionary<string, ItemConfig<T>> configsDictionary, int level)
        where T : PlaceableItemConfigDto
    {
        foreach (var kvp in configsDictionary)
        {
            if (kvp.Value.ConfigDto.unlock_level <= level)
            {
                yield return kvp.Value;
            }
        }
    }

    private UpgradeConfig GetCurrentUpgradeForValueInternal(UpgradeConfig[] upgradeConfigs, int value)
    {
        foreach (var upgrade in upgradeConfigs)
        {
            if (upgrade.Value >= value)
            {
                return upgrade;
            }
        }
        return null;
    }

    private UpgradeConfig GetNextUpgradeForValueInternal(UpgradeConfig[] upgradeConfigs, int value)
    {
        foreach (var upgrade in upgradeConfigs)
        {
            if (upgrade.Value > value)
            {
                return upgrade;
            }
        }
        return null;
    }

    private UpgradeConfig[] GetUpgradesFor(UpgradeType upgradeType)
    {
        return upgradeType switch
        {
            UpgradeType.ExpandX => ExtendShopXUpgradesConfig,
            UpgradeType.ExpandY => ExtendShopYUpgradesConfig,
            UpgradeType.WarehouseVolume => WarehouseVolumeUpgradesConfig,
            UpgradeType.WarehouseSlots => WarehouseSlotsUpgradesConfig,
            _ => null,
        };
    }
}

public class ItemConfig<TConfigDto> where TConfigDto : PlaceableItemConfigDto
{
    public readonly int NumericId;
    public readonly TConfigDto ConfigDto;
    public readonly Price Price;

    public ItemConfig(int numericId, TConfigDto configDto)
    {
        NumericId = numericId;
        ConfigDto = configDto;
        Price = Price.FromString(configDto.price);
    }
}

public class ProductConfig
{
    public readonly int NumericId;
    public readonly string Key;//Sprite name
    public readonly int GroupId;
    public readonly int UnlockLevel;
    public readonly int Volume;
    public readonly Price PricePer1000v;
    public readonly int ProfitPer1000v;
    public readonly int SellPricePer1000v;
    public readonly float SellPrice;
    public readonly float Demand;
    public readonly float DemandPer1000v;
    public readonly int DeliverTimeSeconds;

    private readonly int _amountIn1000Volume;

    public ProductConfig(int numericId, ProductConfigDto dto)
    {
        NumericId = numericId;
        Key = dto.key;
        GroupId = dto.group;
        UnlockLevel = dto.unlock_level;
        _amountIn1000Volume = dto.amount_per_1000v;
        Volume = 1000 / _amountIn1000Volume;
        DemandPer1000v = dto.demand_1000v_per_hour;

        PricePer1000v = Price.FromString(dto.price_per_1000v);

        ProfitPer1000v = dto.profit_per_1000v;
        SellPricePer1000v = PricePer1000v.IsGold ? ProfitPer1000v : PricePer1000v.Value + ProfitPer1000v;
        SellPrice = (float)SellPricePer1000v / _amountIn1000Volume;

        Demand = (int)(DemandPer1000v * _amountIn1000Volume);

        DeliverTimeSeconds = GetTotalSeconds(dto.deliver);
    }

    public int GroupIndex => GroupId - 1;

    public int GetSellPriceForAmount(int amount)
    {
        return (int)Math.Ceiling((amount / (float)_amountIn1000Volume) * SellPricePer1000v);
    }

    public Price GetPriceForAmount(int amount)
    {
        var newValue = (int)Math.Ceiling((amount / (float)_amountIn1000Volume) * PricePer1000v.Value);
        return new Price(newValue, PricePer1000v.IsGold);
    }

    public float GetDemandForVolume(int volume)
    {
        return (int)(DemandPer1000v / (volume * 0.001) * 100) * 0.01f;
    }

    public int GetAmountInVolume(int volume)
    {
        return volume / Volume;
    }

    public Price GetPriceForVolume(int volume)
    {
        var newValue = (int)(PricePer1000v.Value * (float)volume / 1000);
        return new Price(newValue, PricePer1000v.IsGold);
    }

    public int GetSellPriceForVolume(int volume)
    {
        return (int)(SellPricePer1000v * (float)volume / 1000);
    }

    private int GetTotalSeconds(string deliverTimeStr)
    {
        var result = 0;
        var splitted = deliverTimeStr.Split(':').Select(int.Parse).ToArray();
        var length = splitted.Length;
        for (var i = 0; i < length; i++)
        {
            result += splitted[i] * (int)Math.Pow(60, length - i - 1);
        }

        return result;
    }
}

public class PersonalConfig
{
    public readonly string RawIdStr;
    public readonly string TypeIdStr;
    public readonly PersonalType TypeId;
    public readonly int NumericId;
    public readonly string Key;
    public readonly int UnlockLevel;
    public readonly int WorkHours;
    public readonly Price Price;

    public PersonalConfig(string id, PersonalConfigDto dto)
    {
        RawIdStr = id;
        var splitted = id.Split('_');
        TypeIdStr = splitted[0];
        TypeId = GetPersonalTypeByString(TypeIdStr);
        NumericId = int.Parse(splitted[1]);
        Key = dto.key;
        UnlockLevel = dto.unlock_level;
        WorkHours = dto.work_hours;
        Price = Price.FromString(dto.price);
    }

    private static PersonalType GetPersonalTypeByString(string personalTypeStr)
    {
        return personalTypeStr switch
        {
            Constants.PersonalMerchandiserStr => PersonalType.Merchandiser,
            Constants.PersonalCleanerStr => PersonalType.Cleaner,
            Constants.PersonalSecurityStr => PersonalType.Security,
            _ => PersonalType.Undefined,
        };
    }
}

public class UpgradeConfig
{
    public readonly UpgradeType UpgradeType;
    public readonly int Value;
    public readonly Price Price;
    public readonly int UnlockLevel;
    public readonly int UnlockFriends;

    public UpgradeConfig(UpgradeType upgradeType, UpgradeConfigDto dto)
    {
        UpgradeType = upgradeType;
        Value = dto.value;
        Price = Price.FromString(dto.price);
        UnlockLevel = dto.unlock_level;
        UnlockFriends = dto.unlock_friends;
    }

    public string UpgradeTypeStr => UpgradeType switch
    {
        UpgradeType.ExpandX => "expand_x",
        UpgradeType.ExpandY => "expand_y",
        UpgradeType.WarehouseSlots => "add_wh_slots",
        UpgradeType.WarehouseVolume => "add_wh_volume",
        _ => "Undefined upgrade type",
    };
}

public enum UpgradeType
{
    Undefined,
    ExpandX,
    ExpandY,
    WarehouseSlots,
    WarehouseVolume,
}

public enum PersonalType
{
    Undefined,
    Merchandiser,
    Cleaner,
    Security,
}

public interface IProductsConfig
{
    IEnumerable<ProductConfig> GetProductConfigsForLevel(int level);
    ProductConfig GetProductConfigByNumericId(int numericId);
    ProductConfig GetProductConfigByKey(string key);
}

public interface IShelfsConfig
{
    IEnumerable<ItemConfig<ShelfConfigDto>> GetShelfConfigsForLevel(int level);
    ItemConfig<ShelfConfigDto> GetShelfConfigByNumericId(int shelfId);
}

public interface IFloorsConfig
{
    IEnumerable<ItemConfig<ShopDecorationConfigDto>> GetFloorsConfigsForLevel(int level);
    ItemConfig<ShopDecorationConfigDto> GetFloorConfigByNumericId(int levelId);
}

public interface IWallsConfig
{
    IEnumerable<ItemConfig<ShopDecorationConfigDto>> GetWallsConfigsForLevel(int level);
    ItemConfig<ShopDecorationConfigDto> GetWallConfigByNumericId(int levelId);
}

public interface IWindowsConfig
{
    IEnumerable<ItemConfig<ShopDecorationConfigDto>> GetWindowsConfigsForLevel(int level);
    ItemConfig<ShopDecorationConfigDto> GetWindowConfigByNumericId(int levelId);
}

public interface IDoorsConfig
{
    IEnumerable<ItemConfig<ShopDecorationConfigDto>> GetDoorsConfigsForLevel(int level);
    ItemConfig<ShopDecorationConfigDto> GetDoorConfigByNumericId(int levelId);
}

public interface IPersonalConfig
{
    IEnumerable<PersonalConfig> GetPersonalConfigsForLevel(int level);
    PersonalConfig GetPersonalConfigByIds(PersonalType typeId, int numericId);
    PersonalConfig GetPersonalConfigByStringId(string typeIdStr);
}

public interface IUpgradesConfig
{
    UpgradeConfig GetCurrentUpgradeForValue(UpgradeType upgradeType, int value);
    UpgradeConfig GetNextUpgradeForValue(UpgradeType upgradeType, int value);
}

public interface ILevelsConfig
{
    int GetExpByLevel(int level);
    int GetLevelByExp(int exp);
}