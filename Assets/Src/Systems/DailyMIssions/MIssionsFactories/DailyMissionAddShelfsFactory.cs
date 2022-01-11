using System;
using System.Linq;
using UnityEngine;

public class DailyMissionAddShelfsFactory : DailyMissionFactoryBase<DailyMissionAddShelfsProcessor>
{
    protected override bool IsMultipleAllowed => true;
    protected override string Key => MissionKeys.AddShelfs;

    public override bool CanAdd()
    {
        return base.CanAdd()
            && CanAddConsiderSame();
    }

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        DailyMissionModel result = null;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var shelfConfigsForLevel = GameConfigManager.Instance.ShelfsConfig.GetShelfConfigsForLevel(playerModel.ProgressModel.Level);
        var currentMissionsList = playerModel.DailyMissionsModel.MissionsList;
        var availableShelfsConfigsForMission = shelfConfigsForLevel.Where(
            c => false == currentMissionsList.Any(
                m => m.Key == MissionKeys.AddShelfs
                && (m as DailyMissionAddShelfsModel).ShelfNumericId == c.NumericId))
            .ToArray();
        if (availableShelfsConfigsForMission.Length > 0)
        {
            var chosenShelfConfigIndex = Random.Next(0, availableShelfsConfigsForMission.Length);
            var chosenShelfConfig = availableShelfsConfigsForMission[chosenShelfConfigIndex];
            var chosenShelfNumericId = chosenShelfConfig.NumericId;
            var currentShelfCount = playerModel.ShopModel.ShopObjects
                .Count(s => s.Value.Type == ShopObjectType.Shelf && s.Value.NumericId == chosenShelfNumericId);
            var additionalShelfsCount = Math.Max(1, (int)Mathf.Lerp(1, 2 + playerModel.ProgressModel.Level, complexityMultiplier));
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

    private bool CanAddConsiderSame()
    {
        var playerModelHolder = PlayerModelHolder.Instance;
        var configManager = GameConfigManager.Instance;
        var sameMissionsCount = playerModelHolder.UserModel.DailyMissionsModel.MissionsList
            .Count(m => m.Key == Key);
        if (sameMissionsCount > 0)
        {
            var availableShelfConfigs = configManager.ShelfsConfig.GetShelfConfigsForLevel(playerModelHolder.UserModel.ProgressModel.Level);
            return availableShelfConfigs.Count() > sameMissionsCount;
        }
        else
        {
            return true;
        }
    }
}
