//----- old format
public struct GetDataOldResponseDto
{
    public string response;
    public GetDataOldResponseDataDto data;
    public string update_stats_report;
}

public struct GetDataOldResponseDataDto
{
    public string uid;
    public string cash;
    public string gold;
    public string exp;
    public string level;
    public string design;
    public string objects;
    public string warehouse;
    public string config;
    public string activity;
    public string last_visit_time;
    public string bonus_days;
    public string days_play;
    public string first_visit_time;
    public string last_bonus_time;
    public string new_day;
}

//----- new format
public struct CommonResponseDto
{
    public string response;
    public string hash;
    public int v;
}

public struct BoolSuccessResponseDto
{
    public bool success;
}

public struct GetDataResponseDto
{
    public string uid;
    public UserDataDto data;
    public ExternalDataDto external_data;
    public int days_play;
    public int first_visit_time;
    public int last_visit_time;
}

public class UserDataDto
{
    public ShopProgressDto progress;
    public string[] personal;
    public ShopWarehouseDto warehouse;
    public ShopDesignDto design;
    public string[] objects;
    public string[] unwashes;
    public int[] tutorial_steps;
    public string[] actions_data;
}

public class ExternalDataDto
{
}

public class ShopProgressDto
{
    public int cash;
    public int gold;
    public int exp;
    public int level;

    public ShopProgressDto(int cash, int gold, int exp, int level)
    {
        this.cash = cash;
        this.gold = gold;
        this.exp = exp;
        this.level = level;
    }
}

public class ShopWarehouseDto
{
    public int size;
    public int volume;
    public string[] slots;

    public ShopWarehouseDto(int volume, string[] slots)
    {
        size = slots.Length;
        this.volume = volume;
        this.slots = slots;
    }
}

public class ShopDesignDto
{
    public int size_x;
    public int size_y;
    public string[] floors;
    public string[] walls;
    public string[] windows;
    public string[] doors;

    public ShopDesignDto(
        int sizeX,
        int sizeY,
        string[] floors,
        string[] walls,
        string[] windows,
        string[] doors)
    {
        size_x = sizeX;
        size_y = sizeY;
        this.floors = floors;
        this.doors = doors;
        this.windows = windows;
        this.walls = walls;
    }
}