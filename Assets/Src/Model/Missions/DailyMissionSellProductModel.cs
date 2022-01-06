public class DailyMissionSellProductModel : DailyMissionModel
{
    public readonly ProductConfig ProductConfig;

    public DailyMissionSellProductModel(
        string key,
        int startValue,
        int targetValue,
        int currentValue,
        Reward reward,
        bool isRewardTaken,
        ProductConfig productConfig)
        : base(key, startValue, targetValue, currentValue, reward, isRewardTaken)
    {
        ProductConfig = productConfig;
    }
}
