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
                yield return (shelfId, GetShelfConfigById(shelfId));
            }
        }
    }

    public ShelfConfigDto GetShelfConfigById(int shelfId)
    {
        return ShelfsConfig[$"s_{shelfId}"];
    }

    public ShopObjectConfigDto GetCashDeskConfigById(int shelfId)
    {
        return ShopObjectsConfig[$"cd_{shelfId}"];
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
    ShelfConfigDto GetShelfConfigById(int shelfId);
}