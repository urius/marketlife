using System.Collections.Generic;
using UnityEngine;

public class CalculationHelper
{
    public static int GetGoldToCashConversionRate()
    {
        var mainConfig = GameConfigManager.Instance.MainConfig;
        return mainConfig.GoldToCashConversionRate;
    }

    public static Price GetPriceForDeliver(int quickDeliverPriceGoldPerHour, int restDeliverTimeSecnds)
    {
        return new Price(Mathf.Max(1, Mathf.CeilToInt(quickDeliverPriceGoldPerHour * restDeliverTimeSecnds / 3600f)), true);
    }

    public static int GetAmountForProductInVolume(ProductConfig productConfig, int targetVolume)
    {
        return targetVolume / productConfig.Volume;
    }

    public static int GetSellPrice(Price originalPrice)
    {
        return originalPrice.IsGold ? originalPrice.Value * GetGoldToCashConversionRate() : (int)(originalPrice.Value * 0.5f);
    }

    public static float CalculateMood(int usedValue, int totalValue, int unwashesCount, int totalShopSquare)
    {
        var volumeValue = (float)usedValue / totalValue;
        var unwashesValue = (totalShopSquare - unwashesCount) / (float)totalShopSquare;
        var result = volumeValue * unwashesValue;
        return result;
    }

    public static int GetIntegerDemand(float demandMultiplier)
    {
        if (demandMultiplier > 1)
        {
            return (int)demandMultiplier;
        }
        else
        {
            return (Random.Range(0, 1f) <= demandMultiplier) ? 1 : 0;
        }
    }

    public static int CalculateExpToAdd(Dictionary<ProductConfig, int> soldProducts)
    {
        var result = 0;
        foreach (var kvp in soldProducts)
        {
            result += CalculateExpToAdd(kvp.Key, kvp.Value);
        }
        return result;
    }

    public static int CalculateExpToAdd(ProductConfig productConfig, int amount)
    {
        var result = 0;
        if (amount > 0)
        {
            result += System.Math.Max(productConfig.GetPriceForAmount(amount).Value, 1);
        }
        return result;
    }

    public static int GetLevelUpShareReward(int level)
    {
        return level;
    }
}
