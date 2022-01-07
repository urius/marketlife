using System;

public class DailyMissionModel
{
    public event Action ValueChanged = delegate { };
    public event Action RewardTaken = delegate { };

    public readonly string Key;
    public readonly Reward Reward;
    public readonly int StartValue;
    public readonly int TargetValue;

    public DailyMissionModel(
        string key,
        int startValue,
        int targetValue,
        int currentValue,
        Reward reward,
        bool isRewardTaken = false)
    {
        Key = key;
        StartValue = startValue;
        TargetValue = targetValue;
        Value = currentValue;
        Reward = reward;
        IsRewardTaken = isRewardTaken;
    }

    public bool IsRewardTaken { get; private set; }
    public int Value { get; private set; }
    public int ValueRelative => Value - StartValue;
    public float Progress => Math.Min(1, Math.Max(0f, ValueRelative) / (TargetValue - StartValue));
    public bool IsCompleted => Value >= TargetValue;

    public void SetValue(int newValue)
    {
        if (IsRewardTaken) return;

        if (newValue != Value)
        {
            Value = newValue;
            ValueChanged();
        }
    }

    public void AddValue(int delta)
    {
        SetValue(Value + delta);
    }

    public void SetRewardTaken()
    {
        IsRewardTaken = true;
        RewardTaken();
    }
}
