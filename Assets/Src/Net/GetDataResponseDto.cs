public struct GetDataResponseDto
{
    public string response;
    public GetDataResponseDataDto data;
    public string update_stats_report;
}

public struct GetDataResponseDataDto
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
