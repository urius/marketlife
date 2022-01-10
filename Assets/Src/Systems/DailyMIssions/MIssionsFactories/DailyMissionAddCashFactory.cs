using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DailyMissionAddCashFactory : DailyMissionFactoryBase<DailyMissionAddCashProcessor>
{
    protected override string Key => MissionKeys.AddCash;

    public override DailyMissionModel CreateModel(float complexityMultiplier)
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var shopModel = playerModel.ShopModel;
        var productsConfig = GameConfigManager.Instance.ProductsConfig;

        var availableVolume = shopModel.ShopObjects.Values
            .Where(so => so.Type == ShopObjectType.Shelf)
            .Sum(s => (s as ShelfModel).TotalVolume);
        var availableProductConfigs = productsConfig.GetProductConfigsForLevel(playerModel.ProgressModel.Level).ToList();
        var productConfigsToUse = new List<ProductConfig>(availableProductConfigs.Count);

        while (availableVolume > 0 && availableProductConfigs.Count > 0)
        {
            var maxRevenueConfig = GetMaxRevenueConfig(availableProductConfigs);
            productConfigsToUse.Add(maxRevenueConfig);
            availableProductConfigs.Remove(maxRevenueConfig);
            availableVolume -= (int)(maxRevenueConfig.DemandPer1000v * 1000);
        }

        var targetProfitPerHour = productConfigsToUse.Sum(p => p.ProfitPer1000v);
        if (targetProfitPerHour > 0)
        {
            var targetValue = targetProfitPerHour * (int)Mathf.Max(1, Mathf.Lerp(1, 6, complexityMultiplier));
            var reward = ChooseReward(complexityMultiplier);
            return new DailyMissionModel(Key, 0, targetValue, 0, reward);
        }
        else
        {
            return null;
        }
    }

    private ProductConfig GetMaxRevenueConfig(List<ProductConfig> availableProductConfigs)
    {
        ProductConfig result = null;
        if (availableProductConfigs.Count > 0)
        {
            result = availableProductConfigs[0];
            foreach (var config in availableProductConfigs)
            {
                if (config.DemandPer1000v * config.ProfitPer1000v > result.DemandPer1000v * result.ProfitPer1000v)
                {
                    result = config;
                }
            }
        }

        return result;
    }
}
