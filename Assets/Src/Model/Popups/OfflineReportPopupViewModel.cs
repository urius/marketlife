using System.Collections.Generic;
using Src.Common;

public class OfflineReportPopupViewModel : PopupViewModelBase
{
    public readonly SoldProductViewModel[] SoldProducts;
    public readonly int ProfitFromSell;
    public readonly OfflineReportTabType[] Tabs;
    public readonly ItemViewModel[] MerchandiserResult;
    public readonly UserOfflineReportModel ReportModel;

    public OfflineReportPopupViewModel(UserOfflineReportModel reportModel)
    {
        ReportModel = reportModel;

        var tabs = new List<OfflineReportTabType>(3);
        if (ReportModel.HasSellInfo) tabs.Add(OfflineReportTabType.SellProfit);
        if (ReportModel.HasPersonalInfo) tabs.Add(OfflineReportTabType.Personal);
        if (ReportModel.HasActivityInfo) tabs.Add(OfflineReportTabType.Guests);
        Tabs = tabs.ToArray();

        (SoldProducts, ProfitFromSell) = GetDataForProfitTab(reportModel);
        MerchandiserResult = GetMerchandiserResult(reportModel);
    }

    public int UnwashesCleanedAmount => ReportModel.UnwashesCleanedAmount;

    private ItemViewModel[] GetMerchandiserResult(UserOfflineReportModel reportModel)
    {
        var result = new List<ItemViewModel>(reportModel.SoldFromWarehouse.Count);
        foreach (var kvp in reportModel.SoldFromWarehouse)
        {
            if (kvp.Value > 0)
            {
                result.Add(new ItemViewModel(kvp.Key.Key, kvp.Value));
            }
        }
        return result.ToArray();
    }

    public override PopupType PopupType => PopupType.OfflineReport;

    private (SoldProductViewModel[], int) GetDataForProfitTab(UserOfflineReportModel reportModel)
    {
        var resultDictionary = new Dictionary<ProductConfig, int>(reportModel.SoldFromShelfs.Count);
        foreach (var kvp in reportModel.SoldFromShelfs)
        {
            var soldFromShelfsCount = reportModel.SoldFromShelfs[kvp.Key];
            var soldFromWarehouseCount = reportModel.SoldFromWarehouse[kvp.Key];
            if (soldFromShelfsCount + soldFromWarehouseCount > 0)
            {
                resultDictionary[kvp.Key] = soldFromShelfsCount + soldFromWarehouseCount;
            }
        }

        var result = new List<SoldProductViewModel>(reportModel.SoldFromShelfs.Count);
        var totalProfit = 0;
        foreach (var kvp in resultDictionary)
        {
            var productConfig = kvp.Key;
            var sellPrice = productConfig.GetSellPriceForAmount(kvp.Value);
            var expToAdd = CalculationHelper.CalculateExpToAdd(productConfig, kvp.Value);
            totalProfit += sellPrice;
            result.Add(new SoldProductViewModel(productConfig.Key, kvp.Value, sellPrice, expToAdd));
        }

        return (result.ToArray(), totalProfit);
    }
}

public class ItemViewModel
{
    public readonly string ProductKey;
    public readonly int Amount;

    public ItemViewModel(string productKey, int amount)
    {
        ProductKey = productKey;
        Amount = amount;
    }
}

public class SoldProductViewModel : ItemViewModel
{
    public readonly int Profit;
    public readonly int ExpToAdd;

    public SoldProductViewModel(string productKey, int amount, int profit, int expToAdd)
        : base(productKey, amount)
    {
        Profit = profit;
        ExpToAdd = expToAdd;
    }
}

public enum OfflineReportTabType
{
    None,
    SellProfit,
    Personal,
    Guests,
}
