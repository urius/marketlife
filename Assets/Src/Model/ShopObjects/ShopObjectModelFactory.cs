using UnityEngine;

public struct ShopObjectModelFactory
{
    public ShelfModel CreateShelf(int numericId, Vector2Int coords, int side = 3, ProductModel[] products = null)
    {
        var config = GameConfigManager.Instance.MainConfig.GetShelfConfigByNumericId(numericId);
        var result = new ShelfModel(numericId, config.ConfigDto, coords, side);
        if(products != null)
        {
            for(var i = 0; i< products.Length; i++)
            {
                result.TrySetProduct(i, products[i]);
            }
        }

        return result;
    }

    public CashDeskModel CreateCashDesk(int numericId, Vector2Int coords, int side = 3)
    {
        var config = GameConfigManager.Instance.MainConfig.GetCashDeskConfigByNumericId(numericId);
        var result = new CashDeskModel(numericId, config.ConfigDto, coords, side);
        return result;
    }
}
