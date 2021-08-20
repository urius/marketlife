using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerOfflineReportHolder
{
    public static PlayerOfflineReportHolder Instance => _instance.Value;
    private static Lazy<PlayerOfflineReportHolder> _instance = new Lazy<PlayerOfflineReportHolder>();

    public UserOfflineReportModel PlayerOfflineReport { get; private set; }
    public void SetReport(UserOfflineReportModel report)
    {
        PlayerOfflineReport = report;
    }
}

public class UserOfflineReportModel
{
    public readonly int TimeFrom;
    public readonly int TimeTo;
    public readonly float HoursPassed;
    public readonly int MinutesPassed;
    public readonly Dictionary<ProductConfig, int> SoldFromShelfs;
    public readonly Dictionary<ProductConfig, int> SoldFromWarehouse;
    public readonly int SellProfit;
    public readonly int ExpToAdd;
    public readonly bool HasSellInfo;
    public readonly bool HasPersonalInfo;

    public UserOfflineReportModel(
        int timeFrom,
        int timeTo,
        Dictionary<ProductConfig, int> soldFromShelfs,
        Dictionary<ProductConfig, int> soldFromWarehouse)
    {
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        SoldFromShelfs = soldFromShelfs;
        SoldFromWarehouse = soldFromWarehouse;
        HoursPassed = (0.1f * (int)Math.Ceiling(10 * (TimeTo - TimeFrom) / 3600f));
        MinutesPassed = (int)Math.Ceiling((TimeTo - TimeFrom) / 60f);
        SellProfit = CalculateSellProfit(SoldFromWarehouse) + CalculateSellProfit(SoldFromShelfs);
        ExpToAdd = CalculationHelper.CalculateExpToAdd(SoldFromWarehouse) + CalculationHelper.CalculateExpToAdd(SoldFromShelfs);

        var hasSoldFromWarehouse = SoldFromWarehouse.Any(kvp => kvp.Value > 0);
        HasSellInfo = SoldFromShelfs.Any(kvp => kvp.Value > 0) || hasSoldFromWarehouse;
        HasPersonalInfo = hasSoldFromWarehouse;
    }

    public bool IsEmpty => !HasSellInfo && !HasPersonalInfo; //TODO add activity info


    private int CalculateSellProfit(Dictionary<ProductConfig, int> soldProducts)
    {
        var result = 0;
        foreach (var kvp in soldProducts)
        {
            result += kvp.Key.GetSellPriceForAmount(kvp.Value);
        }
        return result;
    }
}
