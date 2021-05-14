using System.Collections.Generic;

public class MainConfig : IShelfsConfig, IFloorsConfig
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

    public ShelfConfigDto GetShelfConfigByLevelId(int shelfLevelId)
    {
        return ShelfsConfig[$"s_{shelfLevelId}"];
    }

    public ShopObjectConfigDto GetCashDeskConfigByLevelId(int cashDeskLevelId)
    {
        return ShopObjectsConfig[$"cd_{cashDeskLevelId}"];
    }

    public IEnumerable<(int id, ShopDecorationConfigDto config)> GetFloorsConfigsForLevel(int level)
    {
        return GetConfigsForLevel(FloorsConfig, level);
    }

    public ShopDecorationConfigDto GetFloorConfigByLevelId(int floorLevelId)
    {
        return FloorsConfig[$"f_{floorLevelId}"];
    }

    public IEnumerable<(int id, ShopDecorationConfigDto config)> GetWallsConfigsForLevel(int level)
    {
        return GetConfigsForLevel(WallsConfig, level);
    }

    public ShopDecorationConfigDto GetWallConfigByLevelId(int wallLevelId)
    {
        return WallsConfig[$"w_{wallLevelId}"];
    }

    public IEnumerable<(int id, ShopDecorationConfigDto config)> GetWindowsConfigsForLevel(int level)
    {
        return GetConfigsForLevel(WindowsConfig, level);
    }

    public ShopDecorationConfigDto GetWindowConfigByLevelId(int windowLevelId)
    {
        return WindowsConfig[$"wnd_{windowLevelId}"];
    }

    public IEnumerable<(int id, ShopDecorationConfigDto config)> GetDoorsConfigsForLevel(int level)
    {
        return GetConfigsForLevel(DoorsConfig, level);
    }

    public ShopDecorationConfigDto GetDoorConfigByLevelId(int doorLevelId)
    {
        return DoorsConfig[$"d_{doorLevelId}"];
    }

    private IEnumerable<(int id, T config)> GetConfigsForLevel<T>(Dictionary<string, T> configsDictionary, int level)
        where T : PlacableItemConfigDto
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

public class PlacableItemConfigDto
{
    public string price;
    public int unlock_level;
}

public class ShopObjectConfigDto : PlacableItemConfigDto
{
    public bool two_sides_mode;
    public int[][] build_matrix;
}

public class ShelfConfigDto : ShopObjectConfigDto
{
    public int part_volume;
    public int parts_num;
}

public class ShopDecorationConfigDto : PlacableItemConfigDto
{
}

public interface IShelfsConfig
{
    IEnumerable<(int id, ShelfConfigDto config)> GetShelfConfigsForLevel(int level);
    ShelfConfigDto GetShelfConfigByLevelId(int shelfId);
}

public interface IFloorsConfig
{
    IEnumerable<(int id, ShopDecorationConfigDto config)> GetFloorsConfigsForLevel(int level);
    ShopDecorationConfigDto GetFloorConfigByLevelId(int levelId);
}

public interface IWallsConfig
{
    IEnumerable<(int id, ShopDecorationConfigDto config)> GetWallsConfigsForLevel(int level);
    ShopDecorationConfigDto GetWallConfigByLevelId(int levelId);
}

public interface IWindowsConfig
{
    IEnumerable<(int id, ShopDecorationConfigDto config)> GetWindowsConfigsForLevel(int level);
    ShopDecorationConfigDto GetWwindowConfigByLevelId(int levelId);
}

public interface IDoorsConfig
{
    IEnumerable<(int id, ShopDecorationConfigDto config)> GetWindowsConfigsForLevel(int level);
    ShopDecorationConfigDto GetWwindowConfigByLevelId(int levelId);
}