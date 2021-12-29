using System;

public class DailyMissionModel
{
    public event Action ValueChanged = delegate { };
    public event Action RewardTaken = delegate { };

    public readonly string Key;
    public readonly Reward Reward;
    public readonly int TargetValue;

    public DailyMissionModel(string key, int value, int targetValue, Reward reward, bool isRewardTaken = false)
    {
        Key = key;
        Value = value;
        TargetValue = targetValue;
        Reward = reward;
        IsRewardTaken = isRewardTaken;
    }

    public int Value { get; private set; }
    public bool IsRewardTaken { get; private set; }

    public void SetValue(int value)
    {
        if (IsRewardTaken) return;

        var newValue = Math.Min(value, TargetValue);
        if (newValue != Value)
        {
            Value = newValue;
            ValueChanged();
        }
    }

    public void SetRewardTaken()
    {
        IsRewardTaken = true;
        RewardTaken();
    }
}
