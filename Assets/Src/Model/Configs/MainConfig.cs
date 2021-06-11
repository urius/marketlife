using System;
using System.Collections.Generic;
using System.Linq;

public class MainConfig : IProductsConfig, IShelfsConfig, IFloorsConfig, IWallsConfig, IWindowsConfig, IDoorsConfig
{
    public readonly int GameplayAtlasVersion;
    public readonly int InterfaceAtlasVersion;
    public readonly int AutoPlacePriceGold;
    public readonly Dictionary<string, ProductConfig> ProductsConfig;
    public readonly Dictionary<string, ItemConfig<ShelfConfigDto>> ShelfsConfig;
    public readonly Dictionary<string, ItemConfig<ShopObjectConfigDto>> ShopObjectsConfig;
    public readonly Dictionary<string, ItemConfig<ShopDecorationConfigDto>> FloorsConfig;
    public readonly Dictionary<string, ItemConfig<ShopDecorationConfigDto>> WallsConfig;
    public readonly Dictionary<string, ItemConfig<ShopDecorationConfigDto>> WindowsConfig;
    public readonly Dictionary<string, ItemConfig<ShopDecorationConfigDto>> DoorsConfig;

    public MainConfig(
        int gameplayAtlasVersion,
        int interfaceAtlasVersion,
        int autoPlacePriceGold,
        Dictionary<string, ProductConfig> productsConfig,
        Dictionary<string, ItemConfig<ShelfConfigDto>> shelfsConfig,
        Dictionary<string, ItemConfig<ShopObjectConfigDto>> shopObjectsConfig,
        Dictionary<string, ItemConfig<ShopDecorationConfigDto>> floorsConfig,
        Dictionary<string, ItemConfig<ShopDecorationConfigDto>> wallsConfig,
        Dictionary<string, ItemConfig<ShopDecorationConfigDto>> windowsConfig,
        Dictionary<string, ItemConfig<ShopDecorationConfigDto>> doorsConfig)
    {
        GameplayAtlasVersion = gameplayAtlasVersion;
        InterfaceAtlasVersion = interfaceAtlasVersion;
        AutoPlacePriceGold = autoPlacePriceGold;
        ProductsConfig = productsConfig;
        ShelfsConfig = shelfsConfig;
        ShopObjectsConfig = shopObjectsConfig;
        FloorsConfig = floorsConfig;
        WallsConfig = wallsConfig;
        WindowsConfig = windowsConfig;
        DoorsConfig = doorsConfig;
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
    public readonly string Key;
    public readonly int GroupId;
    public readonly int UnlockLevel;
    public readonly int Volume;
    public readonly Price PricePer1000v;
    public readonly int ProfitPer1000v;
    public readonly int SellPricePer1000v;
    public readonly float Demand;
    public readonly float DemandPer1000v;
    public readonly int DeliverTimeSeconds;

    public ProductConfig(int numericId, ProductConfigDto dto)
    {
        NumericId = numericId;
        Key = dto.key;
        GroupId = dto.group;
        UnlockLevel = dto.unlock_level;
        var amountIn1000Volume = dto.amount_per_1000v;
        Volume = 1000 / amountIn1000Volume;
        DemandPer1000v = dto.demand_1000v_per_hour;

        PricePer1000v = Price.FromString(dto.price_per_1000v);

        ProfitPer1000v = dto.profit_per_1000v;
        SellPricePer1000v = PricePer1000v.IsGold ? ProfitPer1000v : PricePer1000v.Value + ProfitPer1000v;

        Demand = (int)(DemandPer1000v * amountIn1000Volume);

        DeliverTimeSeconds = GetTotalSeconds(dto.deliver);
    }

    public int GroupIndex => GroupId - 1;

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