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
}
