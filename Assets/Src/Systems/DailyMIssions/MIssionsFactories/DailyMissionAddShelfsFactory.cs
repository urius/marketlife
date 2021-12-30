using System;
using System.Linq;
using UnityEngine;

public class DailyMissionAddShelfsFactory : DailyMissionFactoryBase
{
    private readonly PlayerModelHolder _playerModelHolder;
    private readonly GameConfigManager _configManager;

    public DailyMissionAddShelfsFactory()
    {
        _playerModelHolder = PlayerModelHolder.Instance;
        _configManager = GameConfigManager.Instance;
    }

    protected override string Key => MissionKeys.AddShelfs;

    public override bool CanAdd()
    {
        return base.CanAdd()
            && CanAddConsiderSame();
    }

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        DailyMissionModel result = null;
        var playerModel = _playerModelHolder.UserModel;
        var shelfConfigsForLevel = _configManager.ShelfsConfig.GetShelfConfigsForLevel(playerModel.ProgressModel.Level);
        var currentMissionsList = playerModel.DailyMissionsModel.MissionsList;
        var availableShelfsConfigsForMission = shelfConfigsForLevel.Where(
            c => false == currentMissionsList.Any(m => (m as DailyMissionAddShelfsModel).ShelfNumericId == c.NumericId))
            .ToArray();
        if (availableShelfsConfigsForMission.Length > 0)
        {
            var chosenShelfConfigIndex = Random.Next(0, availableShelfsConfigsForMission.Length);
            var chosenShelfConfig = availableShelfsConfigsForMission[chosenShelfConfigIndex];
            var chosenShelfNumericId = chosenShelfConfig.NumericId;
            var currentShelfCount = playerModel.ShopModel.ShopObjects
                .Count(s => s.Value.Type == ShopObjectType.Shelf && s.Value.NumericId == chosenShelfNumericId);
            var additionalShelfsCount = Math.Max(1, (int)Mathf.Lerp(1, 2 * playerModel.ProgressModel.Level, complexityMultiplier));
            var targetShelfCount = currentShelfCount + additionalShelfsCount;
            var reward = ChooseReward(complexityMultiplier);
            result = new DailyMissionAddShelfsModel(
                Key,
                currentShelfCount,
                targetShelfCount,
                currentShelfCount,
                reward,
                isRewardTaken: false,
                chosenShelfNumericId);
        }
        else
        {
#if UNITY_EDITOR
            throw new InvalidOperationException($"{nameof(DailyMissionAddShelfsFactory)}{nameof(CreateModel)}: cant create model for mission: no available shelf configs");
#endif
        }

        return result;
    }

    public override DailyMissionProcessorBase CreateProcessor(DailyMissionModel mission)
    {
        return new DailyMissionAddShelfsProcessor(mission as DailyMissionAddShelfsModel);
    }

    private bool CanAddConsiderSame()
    {
        var sameMissionsCount = _playerModelHolder.UserModel.DailyMissionsModel.MissionsList
            .Count(m => m.Key == Key);
        if (sameMissionsCount > 0)
        {
            var availableShelfConfigs = _configManager.ShelfsConfig.GetShelfConfigsForLevel(_playerModelHolder.UserModel.ProgressModel.Level);
            return availableShelfConfigs.Count() > sameMissionsCount;
        }
        else
        {
            return true;
        }
    }
}
