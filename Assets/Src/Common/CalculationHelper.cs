using UnityEngine;

public class CalculationHelper
{
    public static Price GetPriceForDeliver(int quickDeliverPriceGoldPerMinute, int restDeliverTimeSecnds)
    {
        return new Price(Mathf.Max(1, quickDeliverPriceGoldPerMinute * restDeliverTimeSecnds / 60), true);
    }

    public static int GetAmountForProductInVolume(ProductConfig productConfig, int targetVolume)
    {
        return targetVolume / productConfig.Volume;
    }

    public static int GetSellPrice(Price originalPrice)
    {
        return originalPrice.IsGold ? originalPrice.Value * 1000 : (int)(originalPrice.Value * 0.5f);
    }
}
