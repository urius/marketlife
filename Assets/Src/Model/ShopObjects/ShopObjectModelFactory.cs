using UnityEngine;

public struct ShopObjectModelFactory
{
    public ShelfModel CreateShelf(int numericId, Vector2Int coords, int side = 3, ProductModel[] products = null)
    {
        var config = GameConfigManager.Instance.MainConfig.GetShelfConfigByNumericId(numericId);
        var result = new ShelfModel(numericId, config.ConfigDto, coords, side);
        if (products != null)
        {
            for (var i = 0; i < products.Length; i++)
            {
                result.TrySetProductOn(i, products[i]);
            }
        }

        return result;
    }

    public CashDeskModel CreateCashDesk(int numericId, Vector2Int coords, int side, string paramsShort = null)
    {
        var config = GameConfigManager.Instance.MainConfig.GetCashDeskConfigByNumericId(numericId);
        var hairId = 1;
        var glassesId = 1;
        var dressId = 1;
        if (paramsShort != null)
        {
            var splitted = paramsShort.Split(',');
            if (splitted.Length > 2)
            {
                int.TryParse(splitted[0], out hairId);
                int.TryParse(splitted[1], out glassesId);
                int.TryParse(splitted[2], out dressId);
            }
        }

        var result = new CashDeskModel(numericId, config.ConfigDto, coords, side, hairId, glassesId, dressId);
        return result;
    }
}
