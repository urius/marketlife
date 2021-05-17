using UnityEngine;

public struct ShopObjectModelFactory
{
    public ShelfModel CreateShelf(int levelId, Vector2Int coords, int side = 3, ProductModel[] products = null)
    {
        var config = GameConfigManager.Instance.MainConfig.GetShelfConfigByNumericId(levelId);
        var result = new ShelfModel(levelId, config, coords, side);
        if(products != null)
        {
            for(var i = 0; i< products.Length; i++)
            {
                result.TrySetProduct(i, products[i]);
            }
        }

        return result;
    }

    public CashDeskModel CreateCashDesk(int levelId, Vector2Int coords, int side = 3)
    {
        var config = GameConfigManager.Instance.MainConfig.GetCashDeskConfigByNumericId(levelId);
        var result = new CashDeskModel(levelId, config, coords, side);
        return result;
    }
}
