using System;
using System.Linq;
using Src.Managers;
using Src.Model;
using Src.Model.Common;
using Src.Model.Configs;
using Src.Model.Missions;
using Src.Systems.DailyMission.Processors;
using UnityEngine;

namespace Src.Systems.DailyMission.MissionFactories
{
    public abstract class DailyMissionFactoryBase<TProcessor> : IDailyMissionFactory
        where TProcessor : DailyMissionProcessorBase, new()
    {
        private readonly GameConfigManager _configManager = GameConfigManager.Instance;
        private readonly PlayerModelHolder _playerModelHolder = PlayerModelHolder.Instance;
        private readonly System.Random _random = new();

        public abstract DailyMissionModel CreateModel(float complexityMultiplier);

        protected virtual bool IsMultipleAllowed => false;
        protected abstract string Key { get; }
        protected MissionConfig MissionConfig => _configManager.DailyMissionsConfig.GetMissionConfig(Key);
        protected System.Random Random => _random;

        public virtual bool CanAdd()
        {
            if (IsMultipleAllowed == false)
            {
                return _playerModelHolder.UserModel.DailyMissionsModel.MissionsList
                    .Any(m => m.Key == Key) == false;
            }
            else
            {
                return true;
            }
        }

        public DailyMissionProcessorBase CreateProcessor(DailyMissionModel mission)
        {
            var result = new TProcessor();
            result.SetupMissionModel(mission);
            return result;
        }

        protected Reward ChooseReward(float complexityMultiplier)
        {
            var rewardConfigs = MissionConfig.RewardConfigs;
            var rewardConfig = rewardConfigs[_random.Next(0, rewardConfigs.Length)];
            var amount = Math.Max(rewardConfig.MinReward.Amount, (int)Mathf.Lerp(rewardConfig.MinReward.Amount, rewardConfig.MaxReward.Amount, complexityMultiplier));
            amount += amount * rewardConfig.RewardLevelMultiplier * _playerModelHolder.UserModel.ProgressModel.Level;
            return new Reward(amount, rewardConfig.MinReward.Type);
        }
    }

    public interface IDailyMissionFactory
    {
        bool CanAdd();
        DailyMissionModel CreateModel(float complexityMultiplier);
        DailyMissionProcessorBase CreateProcessor(DailyMissionModel mission);
    }
}