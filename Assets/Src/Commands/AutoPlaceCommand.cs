using System;

public struct AutoPlaceCommand
{
    public void Execute()
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var warehouseModel = playerModel.ShopModel.WarehouseModel;
        var mainConfig = GameConfigManager.Instance.MainConfig;
        var dispatcher = Dispatcher.Instance;
        var gameStateModel = GameStateModel.Instance;

        if (playerModel.ProgressModel.CanSpendGold(mainConfig.AutoPlacePriceGold))
        {
            var productsAdded = ForEveryShelfSlot(AddOnExisting);
            productsAdded |= ForEveryShelfSlot(AddOnNew);

            if (productsAdded)
            {
                playerModel.ProgressModel.TrySpendGold(mainConfig.AutoPlacePriceGold);
            }
            gameStateModel.ResetPlacingState();
        }
        else
        {
            dispatcher.UIRequestBlinkMoney(true);
        }
    }

    private bool ForEveryShelfSlot(Func<ProductSlotModel, ProductSlotModel, bool> action)
    {
        var isSuccess = false;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var gameStateModel = GameStateModel.Instance;
        var shopModel = playerModel.ShopModel;
        var placingProductSlot = shopModel.WarehouseModel.Slots[gameStateModel.PlacingProductWarehouseSlotIndex];
        if (placingProductSlot.HasProduct)
        {
            foreach (var kvp in shopModel.ShopObjects)
            {
                if (kvp.Value.Type == ShopObjectType.Shelf)
                {
                    var shelfModel = kvp.Value as ShelfModel;
                    foreach (var shelfSlot in shelfModel.Slots)
                    {
                        isSuccess |= action(placingProductSlot, shelfSlot);
                        if (placingProductSlot.HasProduct == false) return true;
                    }
                }
            }
        }
        return isSuccess;
    }


    private bool AddOnExisting(ProductSlotModel slotFrom, ProductSlotModel slotTo)
    {
        var placingProduct = slotFrom.Product;
        if (slotTo.HasProduct && slotTo.Product.Config.NumericId == placingProduct.Config.NumericId)
        {
            var maxSlotAmount = CalculationHelper.GetAmountForProductInVolume(placingProduct.Config, slotTo.Volume);
            var amountToAdd = Math.Min(maxSlotAmount - slotTo.Product.Amount, placingProduct.Amount);
            if (amountToAdd > 0)
            {
                slotTo.ChangeProductAmount(amountToAdd);
                slotFrom.ChangeProductAmount(-amountToAdd);
                return true;
            }
        }

        return false;
    }

    private bool AddOnNew(ProductSlotModel slotFrom, ProductSlotModel slotTo)
    {
        var placingProduct = slotFrom.Product;
        if (slotTo.HasProduct == false)
        {
            var maxSlotAmount = CalculationHelper.GetAmountForProductInVolume(placingProduct.Config, slotTo.Volume);
            var amountToAdd = Math.Min(maxSlotAmount, placingProduct.Amount);
            if (amountToAdd > 0)
            {
                slotTo.SetProduct(new ProductModel(placingProduct.Config, amountToAdd));
                slotFrom.ChangeProductAmount(-amountToAdd);
                return true;
            }
        }
        return false;
    }
}
