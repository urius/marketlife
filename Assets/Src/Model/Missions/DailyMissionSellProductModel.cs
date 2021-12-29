public class DailyMissionSellProductModel : DailyMissionModel
{
    public readonly ProductConfig ProductConfig;

    public DailyMissionSellProductModel(
        string key,
        int value,
        int targetValue,
        Reward reward,
        bool isRewardTaken,
        ProductConfig productModel)
        : base(key, value, targetValue, reward, isRewardTaken)
    {
        ProductConfig = productModel;
    }
}
