using System.Collections.Generic;

public class OfflineReportPopupViewModel : PopupViewModelBase
{
    public readonly SoldProductViewModel[] SoldProducts;
    public readonly int TotalProfitFromSell;
    public readonly OfflineReportTabType[] Tabs;
    public readonly ItemViewModel[] MerchandiserResult;
    public readonly UserOfflineReportModel ReportModel;

    public OfflineReportPopupViewModel(UserOfflineReportModel reportModel)
    {
        ReportModel = reportModel;

        var tabs = new List<OfflineReportTabType>(3);
        if (ReportModel.HasSellInfo) tabs.Add(OfflineReportTabType.SellProfit);
        if (ReportModel.HasPersonalInfo) tabs.Add(OfflineReportTabType.Personal);
        Tabs = tabs.ToArray();

        (SoldProducts, TotalProfitFromSell) = GetDataForProfitTab(reportModel);
        MerchandiserResult = GetMerchandiserResult(reportModel);
    }

    private ItemViewModel[] GetMerchandiserResult(UserOfflineReportModel reportModel)
    {
        var result = new List<ItemViewModel>(reportModel.SoldFromWarehouse.Count);
        foreach (var kvp in reportModel.SoldFromWarehouse)
        {
            result.Add(new ItemViewModel(kvp.Key.Key, kvp.Value));
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
            totalProfit += sellPrice;
            result.Add(new SoldProductViewModel(productConfig.Key, kvp.Value, sellPrice));
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

    public SoldProductViewModel(string productKey, int amount, int profit)
        : base(productKey, amount)
    {
        Profit = profit;
    }
}

public enum OfflineReportTabType
{
    None,
    SellProfit,
    Personal,
    Activity,
}
