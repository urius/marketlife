using System;
using System.Collections.Generic;

public class UserModel
{
    public readonly string Uid;
    public readonly UserProgressModel ProgressModel;
    public readonly ShopModel ShopModel;
    public readonly UserStatsData StatsData;

    public UserModel(string uid, UserProgressModel progressModel, ShopModel shopModel, UserStatsData statsData)
    {
        Uid = uid;
        ProgressModel = progressModel;
        ShopModel = shopModel;
        StatsData = statsData;
    }

    public bool CanSpendMoney(string price)
    {
        return CanSpendMoney(Price.FromString(price));
    }

    public bool CanSpendMoney(Price price)
    {
        return price.IsGold ? ProgressModel.CanSpendGold(price.Value) : ProgressModel.CanSpendCash(price.Value);
    }

    public bool TrySpendMoney(string price)
    {
        return TrySpendMoney(Price.FromString(price));
    }

    public bool TrySpendMoney(Price price)
    {
        return price.IsGold ? ProgressModel.TrySpendGold(price.Value) : ProgressModel.TrySpendCash(price.Value);
    }

    public void AddCash(int amount)
    {
        ProgressModel.AddCash(amount);
    }

    public (Dictionary<ProductConfig, int> SoldFromShelfs, Dictionary<ProductConfig, int> SoldFromWarehouse) CalculateSellsToTime(int targetTime)
    {
        var userShopModel = ShopModel;
        var personalModel = userShopModel.PersonalModel;
        var warehouseModel = userShopModel.WarehouseModel;

        var (restProductsOnShelfs, totalShelfsVolume, restUsedShelfsVolume) = userShopModel.GetAllProductsInfo();
        var uniqueProductsOnShelfsCount = restProductsOnShelfs.Length;
        var startCalculationTime = StatsData.LastVisitTimestamp;
        var secondsSinceLastVisit = targetTime - startCalculationTime;
        var hoursSinceLastVisit = secondsSinceLastVisit / 3600f;
        var hoursCeilSinceLastVisit = (int)Math.Ceiling(hoursSinceLastVisit);
        var soldFromShelfsProducts = new Dictionary<ProductConfig, int>(uniqueProductsOnShelfsCount);
        var soldFromWarehouseProducts = new Dictionary<ProductConfig, int>(uniqueProductsOnShelfsCount);
        var restWarehouseProductsForMerchandiser = new Dictionary<ProductConfig, int>(uniqueProductsOnShelfsCount);
        foreach (var productModel in restProductsOnShelfs)
        {
            soldFromShelfsProducts[productModel.Config] = 0;
            soldFromWarehouseProducts[productModel.Config] = 0;
            restWarehouseProductsForMerchandiser[productModel.Config] = warehouseModel.GetDeliveredProductAmount(productModel.Config.NumericId, targetTime);
        }

        for (var i = 0; i < hoursCeilSinceLastVisit; i++)
        {
            var moodMultiplier = CalculationHelper.CalculateMood(restUsedShelfsVolume, totalShelfsVolume);
            var hourMultiplier = Math.Min(1, hoursSinceLastVisit - i);
            var buyMultiplier = moodMultiplier * hourMultiplier;
            var iterationStartTime = startCalculationTime + i * 3600;
            var isMerchandiserActive = personalModel.GetMaxEndWorkTimeForPersonalType(PersonalType.Merchandiser) > iterationStartTime;

            var haveProductsOnShefsFlag = false;
            foreach (var productModel in restProductsOnShelfs)
            {
                if (productModel.Amount > 0)
                {
                    haveProductsOnShefsFlag = true;

                    var productConfig = productModel.Config;
                    var demandedRestAmountToSell = (int)(productConfig.Demand * buyMultiplier);

                    if (isMerchandiserActive && restWarehouseProductsForMerchandiser[productConfig] > 0)
                    {
                        var sellFromWarehouseAmount = Math.Min(restWarehouseProductsForMerchandiser[productConfig], demandedRestAmountToSell);
                        restWarehouseProductsForMerchandiser[productConfig] -= sellFromWarehouseAmount;
                        soldFromWarehouseProducts[productConfig] += sellFromWarehouseAmount;
                        demandedRestAmountToSell -= sellFromWarehouseAmount;
                    }

                    var sellFromShelfsAmount = Math.Min(productModel.Amount, demandedRestAmountToSell);
                    if (sellFromShelfsAmount > 0)
                    {
                        soldFromShelfsProducts[productConfig] += sellFromShelfsAmount;
                        productModel.Amount -= sellFromShelfsAmount;
                        restUsedShelfsVolume -= productConfig.Volume * sellFromShelfsAmount;
                        demandedRestAmountToSell -= sellFromShelfsAmount;
                    }
                }
            }

            if (haveProductsOnShefsFlag == false)
            {
                break;
            }
        }

        return (soldFromShelfsProducts, soldFromWarehouseProducts);
    }
}

public struct UserStatsData
{
    public int FirstVisitTimestamp;
    public int LastVisitTimestamp;
    public int TotalDaysPlayCount;

    public UserStatsData(
        int firstVisitTimestamp,
        int lastVisitTimestamp,
        int totalDaysPlayCount)
    {
        FirstVisitTimestamp = firstVisitTimestamp;
        LastVisitTimestamp = lastVisitTimestamp;
        TotalDaysPlayCount = totalDaysPlayCount;
    }
}

public class UserProgressModel
{
    public event Action<int, int> CashChanged = delegate { };
    public event Action<int, int> GoldChanged = delegate { };
    public event Action<int, int> ExpChanged = delegate { };
    public event Action<int, int> LevelChanged = delegate { };

    public int Cash => Decode(_cashEncoded);
    public int Gold => Decode(_goldEncoded);
    public int ExpAmount => Decode(_expEncoded);
    public int Level => Decode(_levelEncoded);

    private string _cashEncoded;
    private string _goldEncoded;
    private string _expEncoded;
    private string _levelEncoded;

    public UserProgressModel(int cash, int gold, int expAmount, int level)
    {
        _cashEncoded = Encode(cash);
        _goldEncoded = Encode(gold);
        _expEncoded = Encode(expAmount);
        _levelEncoded = Encode(level);
    }

    public void SetCash(int newValue)
    {
        var valueBefore = Cash;
        _cashEncoded = Encode(newValue);
        CashChanged(valueBefore, newValue);
    }

    public bool CanSpendCash(int spendAmount)
    {
        var currentValue = Cash;
        return currentValue >= spendAmount;
    }

    public void AddCash(int amount)
    {
        TrySpendCash(-amount);
    }

    public bool TrySpendMoney(Price price)
    {
        return price.IsGold ? TrySpendGold(price.Value) : TrySpendCash(price.Value);
    }

    public bool TrySpendCash(int spendAmount)
    {
        var currentValue = Cash;
        if (currentValue >= spendAmount)
        {
            currentValue -= spendAmount;
            SetCash(currentValue);
            return true;
        }
        return false;
    }

    public void SetGold(int newValue)
    {
        var valueBefore = Gold;
        _goldEncoded = Encode(newValue);
        GoldChanged(valueBefore, newValue);
    }

    public bool CanSpendGold(int spendAmount)
    {
        var currentValue = Gold;
        return currentValue >= spendAmount;
    }

    public void AddGold(int amount)
    {
        TrySpendGold(-amount);
    }

    public bool TrySpendGold(int spendAmount)
    {
        var currentValue = Gold;
        if (currentValue >= spendAmount)
        {
            currentValue -= spendAmount;
            SetGold(currentValue);
            return true;
        }
        return false;
    }

    public void SetExp(int newValue)
    {
        var valueBefore = ExpAmount;
        _expEncoded = Encode(newValue);
        ExpChanged(valueBefore, newValue);
    }

    public void SetLevel(int newValue)
    {
        var valueBefore = Level;
        _levelEncoded = Encode(newValue);
        LevelChanged(valueBefore, newValue);
    }

    private string Encode(int input)
    {
        return Base64Helper.Base64Encode(input.ToString());
    }

    private int Decode(string base64Input)
    {
        return int.Parse(Base64Helper.Base64Decode(base64Input));
    }
}
