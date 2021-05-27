using System;
using System.Collections.Generic;

public class MainConfig : IShelfsConfig, IFloorsConfig, IWallsConfig, IWindowsConfig, IDoorsConfig
{
    public int GameplayAtlasVersion;
    public int InterfaceAtlasVersion;
    public Dictionary<string, ShelfConfigDto> ShelfsConfig;
    public Dictionary<string, ShopObjectConfigDto> ShopObjectsConfig;
    public Dictionary<string, ShopDecorationConfigDto> FloorsConfig;
    public Dictionary<string, ShopDecorationConfigDto> WallsConfig;
    public Dictionary<string, ShopDecorationConfigDto> WindowsConfig;
    public Dictionary<string, ShopDecorationConfigDto> DoorsConfig;

    public IEnumerable<(int id, ShelfConfigDto config)> GetShelfConfigsForLevel(int level)
    {
        return GetConfigsForLevel(ShelfsConfig, level);
    }

    public ShelfConfigDto GetShelfConfigByNumericId(int numericId)
    {
        return ShelfsConfig[$"s_{numericId}"];
    }

    public ShopObjectConfigDto GetCashDeskConfigByNumericId(int cashDeskNumericId)
    {
        return ShopObjectsConfig[$"cd_{cashDeskNumericId}"];
    }

    public IEnumerable<(int id, ShopDecorationConfigDto config)> GetFloorsConfigsForLevel(int level)
    {
        return GetConfigsForLevel(FloorsConfig, level);
    }

    public ShopDecorationConfigDto GetDecorationConfigBuNumericId(ShopDecorationObjectType decorationType, int numericId)
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

    public ShopDecorationConfigDto GetFloorConfigByNumericId(int floorNumericId)
    {
        return FloorsConfig[$"f_{floorNumericId}"];
    }

    public IEnumerable<(int id, ShopDecorationConfigDto config)> GetWallsConfigsForLevel(int level)
    {
        return GetConfigsForLevel(WallsConfig, level);
    }

    public ShopDecorationConfigDto GetWallConfigByNumericId(int wallNumericId)
    {
        return WallsConfig[$"w_{wallNumericId}"];
    }

    public IEnumerable<(int id, ShopDecorationConfigDto config)> GetWindowsConfigsForLevel(int level)
    {
        return GetConfigsForLevel(WindowsConfig, level);
    }

    public ShopDecorationConfigDto GetWindowConfigByNumericId(int windowNumericId)
    {
        return WindowsConfig[$"wnd_{windowNumericId}"];
    }

    public IEnumerable<(int id, ShopDecorationConfigDto config)> GetDoorsConfigsForLevel(int level)
    {
        return GetConfigsForLevel(DoorsConfig, level);
    }

    public ShopDecorationConfigDto GetDoorConfigByNumericId(int doorNumericId)
    {
        return DoorsConfig[$"d_{doorNumericId}"];
    }

    private IEnumerable<(int id, T config)> GetConfigsForLevel<T>(Dictionary<string, T> configsDictionary, int level)
        where T : PlaceableItemConfigDto
    {
        foreach (var kvp in configsDictionary)
        {
            if (kvp.Value.unlock_level <= level)
            {
                var id = int.Parse(kvp.Key.Split('_')[1]);
                yield return (id, kvp.Value);
            }
        }
    }
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

public interface IShelfsConfig
{
    IEnumerable<(int id, ShelfConfigDto config)> GetShelfConfigsForLevel(int level);
    ShelfConfigDto GetShelfConfigByNumericId(int shelfId);
}

public interface IFloorsConfig
{
    IEnumerable<(int id, ShopDecorationConfigDto config)> GetFloorsConfigsForLevel(int level);
    ShopDecorationConfigDto GetFloorConfigByNumericId(int levelId);
}

public interface IWallsConfig
{
    IEnumerable<(int id, ShopDecorationConfigDto config)> GetWallsConfigsForLevel(int level);
    ShopDecorationConfigDto GetWallConfigByNumericId(int levelId);
}

public interface IWindowsConfig
{
    IEnumerable<(int id, ShopDecorationConfigDto config)> GetWindowsConfigsForLevel(int level);
    ShopDecorationConfigDto GetWindowConfigByNumericId(int levelId);
}

public interface IDoorsConfig
{
    IEnumerable<(int id, ShopDecorationConfigDto config)> GetDoorsConfigsForLevel(int level);
    ShopDecorationConfigDto GetDoorConfigByNumericId(int levelId);
}