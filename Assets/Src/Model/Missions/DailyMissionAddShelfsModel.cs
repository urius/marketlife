public class DailyMissionAddShelfsModel : DailyMissionModel
{
    public readonly int ShelfNumericId;

    public DailyMissionAddShelfsModel(
        string key,
        int startValue,
        int targetValue,
        int currentValue,
        Reward reward,
        bool isRewardTaken,
        int shelfNumericId)
        : base(key, startValue, targetValue, currentValue, reward, isRewardTaken)
    {
        ShelfNumericId = shelfNumericId;
    }
}
