using System;
using System.Collections.Generic;

public class MainConfig : IShelfsConfig, IFloorsConfig, IWallsConfig, IWindowsConfig, IDoorsConfig
{
    public readonly int GameplayAtlasVersion;
    public readonly int InterfaceAtlasVersion;
    public readonly Dictionary<string, ItemConfig<ShelfConfigDto>> ShelfsConfig;
    public readonly Dictionary<string, ItemConfig<ShopObjectConfigDto>> ShopObjectsConfig;
    public readonly Dictionary<string, ItemConfig<ShopDecorationConfigDto>> FloorsConfig;
    public readonly Dictionary<string, ItemConfig<ShopDecorationConfigDto>> WallsConfig;
    public readonly Dictionary<string, ItemConfig<ShopDecorationConfigDto>> WindowsConfig;
    public readonly Dictionary<string, ItemConfig<ShopDecorationConfigDto>> DoorsConfig;

    public MainConfig(
        int gameplayAtlasVersion,
        int interfaceAtlasVersion,
        Dictionary<string, ItemConfig<ShelfConfigDto>> shelfsConfig,
        Dictionary<string, ItemConfig<ShopObjectConfigDto>> shopObjectsConfig,
        Dictionary<string, ItemConfig<ShopDecorationConfigDto>> floorsConfig,
        Dictionary<string, ItemConfig<ShopDecorationConfigDto>> wallsConfig,
        Dictionary<string, ItemConfig<ShopDecorationConfigDto>> windowsConfig,
        Dictionary<string, ItemConfig<ShopDecorationConfigDto>> doorsConfig)
    {
        GameplayAtlasVersion = gameplayAtlasVersion;
        InterfaceAtlasVersion = interfaceAtlasVersion;
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