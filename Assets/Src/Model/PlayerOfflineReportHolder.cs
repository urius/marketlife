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
    public readonly Dictionary<ProductConfig, int> GrabbedProducts;
    public readonly int SellProfit;
    public readonly int ExpToAdd;
    public readonly bool HasSellInfo;
    public readonly bool HasPersonalInfo;
    public readonly bool HasActivityInfo;
    public readonly int UnwashesCleanedAmount;
    public readonly IEnumerable<GuestOfflineReportActionModel> GuestOfflineActionModels;

    public UserOfflineReportModel(
        int timeFrom,
        int timeTo,
        Dictionary<ProductConfig, int> soldFromShelfs,
        Dictionary<ProductConfig, int> soldFromWarehouse,
        Dictionary<ProductConfig, int> grabbedProducts,
        int unwashesCleanedAmount,
        IEnumerable<GuestOfflineReportActionModel> guestOfflineActionModels)
    {
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        SoldFromShelfs = soldFromShelfs;
        SoldFromWarehouse = soldFromWarehouse;
        GrabbedProducts = grabbedProducts;
        UnwashesCleanedAmount = unwashesCleanedAmount;
        GuestOfflineActionModels = guestOfflineActionModels;
        HoursPassed = (0.1f * (int)Math.Ceiling(10 * (TimeTo - TimeFrom) / 3600f));
        MinutesPassed = (int)Math.Ceiling((TimeTo - TimeFrom) / 60f);
        SellProfit = CalculateSellProfit(SoldFromWarehouse) + CalculateSellProfit(SoldFromShelfs);
        ExpToAdd = CalculationHelper.CalculateExpToAddOffline(SoldFromWarehouse) + CalculationHelper.CalculateExpToAddOffline(SoldFromShelfs);

        var hasSoldFromWarehouse = SoldFromWarehouse.Any(kvp => kvp.Value > 0);
        HasSellInfo = SoldFromShelfs.Any(kvp => kvp.Value > 0) || hasSoldFromWarehouse;
        HasPersonalInfo = (UnwashesCleanedAmount > 0) || hasSoldFromWarehouse;
        HasActivityInfo = GrabbedProducts.Any();
    }

    public bool IsEmpty => !HasSellInfo && !HasPersonalInfo && !HasActivityInfo; //TODO add activity info


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

public class GuestOfflineReportActionModel
{
    public readonly string UserId;
    public readonly IEnumerable<ProductModel> TakenProducts;
    public readonly IEnumerable<ProductModel> AddedProducts;
    public readonly int AddedUnwashes;

    public GuestOfflineReportActionModel(
        string userId,
        IEnumerable<ProductModel> takenProducts,
        IEnumerable<ProductModel> addedProducts,
        int addedUnwashes)
    {
        UserId = userId;
        TakenProducts = takenProducts;
        AddedProducts = addedProducts;
        AddedUnwashes = addedUnwashes;
    }
}
