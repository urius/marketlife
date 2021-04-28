using System.Collections.Generic;

public class MainConfig : IShelfsConfig
{
    public int GameplayAtlasVersion;
    public int InterfaceAtlasVersion;
    public Dictionary<string, ShelfConfigDto> ShelfsConfig;
    public Dictionary<string, ShopObjectConfigDto> ShopObjectsConfig;

    public IEnumerable<(int id, ShelfConfigDto config)> GetShelfConfigsForLevel(int level)
    {
        foreach (var kvp in ShelfsConfig)
        {
            if (kvp.Value.unlock_level <= level)
            {
                var shelfId = int.Parse(kvp.Key.Split('_')[1]);
                yield return (shelfId, GetShelfConfigByLevelId(shelfId));
            }
        }
    }

    public ShelfConfigDto GetShelfConfigByLevelId(int shelfLevelId)
    {
        return ShelfsConfig[$"s_{shelfLevelId}"];
    }

    public ShopObjectConfigDto GetCashDeskConfigByLevelId(int cashDeskLevelId)
    {
        return ShopObjectsConfig[$"cd_{cashDeskLevelId}"];
    }
}

public class ShopObjectConfigDto
{
    public string price;
    public int unlock_level;
    public bool two_sides_mode;
    public int[][] build_matrix;
}

public class ShelfConfigDto : ShopObjectConfigDto
{
    public int part_volume;
    public int parts_num;
}

public interface IShelfsConfig
{
    IEnumerable<(int id, ShelfConfigDto config)> GetShelfConfigsForLevel(int level);
    ShelfConfigDto GetShelfConfigByLevelId(int shelfId);
}