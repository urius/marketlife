using System;

public struct PutWarehouseProductOnShelfCommand
{
    public int Execute(int warehouseSlotIndex, ProductSlotModel shelfSlot)
    {
        var gameStateModel = GameStateModel.Instance;
        var playerShopModel = PlayerModelHolder.Instance.ShopModel;
        var warehouseSlot = playerShopModel.WarehouseModel.Slots[warehouseSlotIndex];
        var placedProductsCount = 0;

        if (warehouseSlot.HasProduct && warehouseSlot.Product.DeliverTime <= gameStateModel.ServerTime)
        {
            var placingProduct = warehouseSlot.Product;
            if (shelfSlot.HasProduct && shelfSlot.Product.NumericId == warehouseSlot.Product.NumericId)
            {
                var maxSlotAmount = CalculationHelper.GetAmountForProductInVolume(placingProduct.Config, shelfSlot.Volume);
                var amountToAdd = Math.Min(maxSlotAmount - shelfSlot.Product.Amount, placingProduct.Amount);
                if (amountToAdd > 0)
                {
                    shelfSlot.ChangeProductAmount(amountToAdd);
                    warehouseSlot.ChangeProductAmount(-amountToAdd);
                    placedProductsCount = amountToAdd;
                }
            }
            else if (shelfSlot.HasProduct == false)
            {
                var maxSlotAmount = CalculationHelper.GetAmountForProductInVolume(placingProduct.Config, shelfSlot.Volume);
                var amountToAdd = Math.Min(maxSlotAmount, placingProduct.Amount);
                if (amountToAdd > 0)
                {
                    shelfSlot.SetProduct(new ProductModel(placingProduct.Config, amountToAdd));
                    warehouseSlot.ChangeProductAmount(-amountToAdd);
                    placedProductsCount = amountToAdd;
                }
            }
        }

        return placedProductsCount;
    }
}
