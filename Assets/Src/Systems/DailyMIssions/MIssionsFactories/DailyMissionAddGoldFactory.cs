using UnityEngine;

public class DailyMissionAddGoldFactory : DailyMissionFactoryBase
{
    private readonly PlayerModelHolder _playerModelHolder;

    public DailyMissionAddGoldFactory()
    {
        _playerModelHolder = PlayerModelHolder.Instance;
    }

    protected override string Key => MissionKeys.AddGold;

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        var reward = ChooseReward(complexityMultiplier);
        var goldToAdd = (int)Mathf.Max(1, Mathf.Lerp(1, 100, complexityMultiplier));
        return new DailyMissionModel(Key, 0, goldToAdd, 0, reward);
    }

    public override DailyMissionProcessorBase CreateProcessor(DailyMissionModel mission)
    {
        return new DailyMissionAddGoldProcessor(mission);
    }
}
