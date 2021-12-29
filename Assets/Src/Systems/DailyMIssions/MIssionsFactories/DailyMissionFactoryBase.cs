using System;
using System.Linq;
using UnityEngine;

public abstract class DailyMissionFactoryBase
{
    private readonly GameConfigManager _configManager;
    private readonly PlayerModelHolder _playerModelHolder;
    private readonly System.Random _random;

    public DailyMissionFactoryBase()
    {
        _configManager = GameConfigManager.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _random = new System.Random();
    }

    public abstract DailyMissionModel CreateModel(float complexityMultiplier);
    public abstract DailyMissionProcessorBase CreateProcessor(DailyMissionModel mission);

    protected abstract string Key { get; }
    protected MissionConfig MissionConfig => _configManager.DailyMissionsConfig.GetMissionConfig(Key);

    public virtual bool CanAdd()
    {
        var dailyMissionsModel = _playerModelHolder.UserModel.DailyMissionsModel;
        return dailyMissionsModel.MissionsList.Any(m => m.Key == Key) == false;
    }

    protected Reward ChooseReward(MissionRewardConfig[] rewardConfigs, float complexityMultiplier)
    {
        var rewardConfig = rewardConfigs[_random.Next(0, rewardConfigs.Length)];
        var amount = Math.Max(rewardConfig.MinReward.Amount, (int)Mathf.Lerp(rewardConfig.MinReward.Amount, rewardConfig.MaxReward.Amount, complexityMultiplier));
        amount += amount * rewardConfig.RewardLevelMultiplier * _playerModelHolder.UserModel.ProgressModel.Level;
        return new Reward(amount, rewardConfig.MinReward.Type);
    }
}
