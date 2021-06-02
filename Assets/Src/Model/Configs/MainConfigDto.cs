using System.Collections.Generic;

public class MainConfigDto
{
    public int GameplayAtlasVersion;
    public int InterfaceAtlasVersion;
    public Dictionary<string, ShelfConfigDto> ShelfsConfig;
    public Dictionary<string, ShopObjectConfigDto> ShopObjectsConfig;
    public Dictionary<string, ShopDecorationConfigDto> FloorsConfig;
    public Dictionary<string, ShopDecorationConfigDto> WallsConfig;
    public Dictionary<string, ShopDecorationConfigDto> WindowsConfig;
    public Dictionary<string, ShopDecorationConfigDto> DoorsConfig;
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
