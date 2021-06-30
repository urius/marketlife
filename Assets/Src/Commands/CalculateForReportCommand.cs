using System;
using System.Collections.Generic;

public struct CalculateForReportCommand
{
    public void Execute()
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var plaerShopModel = playerModel.ShopModel;
        var gameStateModel = GameStateModel.Instance;

        var (restProducts, totalVolume, restUsedVolume) = GetAllProductsInfo(plaerShopModel);
        var secondsSinceLastVisit = gameStateModel.ServerTime - playerModel.StatsData.LastVisitTimestamp;
        var hoursSinceLastVisit = secondsSinceLastVisit / 3600f;
        var hoursCeilSinceLastVisit = (int)Math.Ceiling(hoursSinceLastVisit);
        var soldProducts = new Dictionary<ProductConfig, int>();
        foreach (var productModel in restProducts)
        {
            soldProducts[productModel.Config] = 0;
        }

        for (var i = 0; i < hoursCeilSinceLastVisit; i++)
        {
            var moodMultiplier = CalculationHelper.CalculateMood(restUsedVolume, totalVolume);
            var hourMultiplier = Math.Min(1, hoursSinceLastVisit - i);
            var buyMultiplier = moodMultiplier * hourMultiplier;

            foreach (var productModel in restProducts)
            {
                if (productModel.Amount > 0)
                {
                    var amountToSell = (int)Math.Min(productModel.Config.Demand * buyMultiplier, productModel.Amount);
                    soldProducts[productModel.Config] += amountToSell;
                    productModel.Amount -= amountToSell;
                    restUsedVolume -= productModel.Config.Volume * amountToSell;
                }
            }
        }

    }

    private (ProductModel[] Products, int TotalVolume, int UsedVolume) GetAllProductsInfo(ShopModel shopModel)
    {
        var totalVolume = 0;
        var usedVolume = 0;
        var productsDictionary = new Dictionary<ProductConfig, int>();
        foreach (var kvp in shopModel.ShopObjects)
        {
            if (kvp.Value.Type == ShopObjectType.Shelf)
            {
                var shelfModel = kvp.Value as ShelfModel;
                foreach (var slot in shelfModel.Slots)
                {
                    totalVolume += slot.Volume;
                    if (slot.HasProduct)
                    {
                        var productConfig = slot.Product.Config;
                        usedVolume += slot.Product.Amount * productConfig.Volume;
                        if (productsDictionary.ContainsKey(productConfig))
                        {
                            productsDictionary[productConfig] += slot.Product.Amount;
                        }
                        else
                        {
                            productsDictionary[productConfig] = slot.Product.Amount;
                        }
                    }
                }
            }
        }

        var productsList = new List<ProductModel>(productsDictionary.Count);
        foreach (var kvp in productsDictionary)
        {
            productsList.Add(new ProductModel(kvp.Key, kvp.Value));
        }

        return (productsList.ToArray(), totalVolume, usedVolume);
    }
}
