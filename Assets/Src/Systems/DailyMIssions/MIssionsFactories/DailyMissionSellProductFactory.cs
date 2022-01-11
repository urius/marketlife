using System.Linq;
using UnityEngine;

public class DailyMissionSellProductFactory : DailyMissionFactoryBase<DailyMissionSellProductProcessor>
{
    protected override bool IsMultipleAllowed => true;
    protected override string Key => MissionKeys.SellProduct;

    public override bool CanAdd()
    {
        return base.CanAdd()
            && ExtraCanAddCondition();
    }

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        var productsConfig = GameConfigManager.Instance.ProductsConfig;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var productConfigs = productsConfig.GetProductConfigsForLevel(playerModel.ProgressModel.Level)
            .Where(HaveNoSameMissionWithProduct)
            .ToArray();        
        if (productConfigs.Length > 0)
        {
            var chosenProductConfig = productConfigs[Random.Next(0, productConfigs.Length)];
            var sellAmountPerHour = chosenProductConfig.Demand;
            var targetValue = (int)Mathf.Max(1, Mathf.Lerp(sellAmountPerHour, MissionConfig.MaxComplexityFactor * sellAmountPerHour, complexityMultiplier));
            var reward = ChooseReward(complexityMultiplier);
            return new DailyMissionSellProductModel(Key, 0, targetValue, 0, reward, isRewardTaken: false, chosenProductConfig);
        }

        return null;
    }

    private bool HaveNoSameMissionWithProduct(ProductConfig productConfig)
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var missionsList = playerModel.DailyMissionsModel.MissionsList;
        var sameKeyMissions = missionsList.Where(m => m.Key == Key);
        if (sameKeyMissions.Any())
        {
            var haveSameMission = sameKeyMissions.Any(m => (m as DailyMissionSellProductModel).ProductConfig.NumericId == productConfig.NumericId);
            return haveSameMission == false;
        }
        else
        {
            return true;
        }
    }

    private bool ExtraCanAddCondition()
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        if (playerModel.DailyMissionsModel.MissionsList.Any(m => m.Key == Key))
        {
            var productsConfig = GameConfigManager.Instance.ProductsConfig;
            var productConfigs = productsConfig.GetProductConfigsForLevel(playerModel.ProgressModel.Level).ToArray();
            var sameKeyMissionsCount = playerModel.DailyMissionsModel.MissionsList.Count(m => m.Key == Key);
            return productConfigs.Length > sameKeyMissionsCount;
        }
        else
        {
            return true;
        }
    }
}
