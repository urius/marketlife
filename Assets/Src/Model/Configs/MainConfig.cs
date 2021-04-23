using System.Collections.Generic;

public class MainConfig
{
    public int GameplayAtlasVersion;
    public int InterfaceAtlasVersion;    
    public Dictionary<string, ShelfConfigDto> ShelfsConfig;
    public Dictionary<string, ShopObjectConfigDto> ShopObjectsConfig;
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