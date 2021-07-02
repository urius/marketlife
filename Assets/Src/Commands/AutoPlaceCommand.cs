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

        if (playerModel.ProgressModel.TrySpendGold(mainConfig.AutoPlacePriceGold))
        {
            ForEveryShelfSlot(AddOnExisting);
            ForEveryShelfSlot(AddOnNew);

            var placingProductSlot = warehouseModel.Slots[gameStateModel.PlacingProductWarehouseSlotIndex];
            if (false == placingProductSlot.HasProduct)
            {
                gameStateModel.ResetPlacingState();
            }
        }
        else
        {
            dispatcher.UIRequestBlinkMoney(true);
        }
    }

    private void ForEveryShelfSlot(Action<ProductSlotModel, ProductSlotModel> action)
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var gameStateModel = GameStateModel.Instance;
        var shopModel = playerModel.ShopModel;
        var placingProductSlot = shopModel.WarehouseModel.Slots[gameStateModel.PlacingProductWarehouseSlotIndex];

        if (false == placingProductSlot.HasProduct) return;
        foreach (var kvp in shopModel.ShopObjects)
        {
            if (kvp.Value.Type == ShopObjectType.Shelf)
            {
                var shelfModel = kvp.Value as ShelfModel;
                foreach (var shelfSlot in shelfModel.Slots)
                {
                    action(placingProductSlot, shelfSlot);
                    if (placingProductSlot.HasProduct == false) return;
                }
            }
        }
    }


    private void AddOnExisting(ProductSlotModel slotFrom, ProductSlotModel slotTo)
    {
        var placingProduct = slotFrom.Product;
        if (slotTo.HasProduct && slotTo.Product.Config.NumericId == placingProduct.Config.NumericId)
        {
            var maxSlotAmount = CalculationHelper.GetAmountForProductInVolume(placingProduct.Config, slotTo.Volume);
            var amountToAdd = Math.Min(maxSlotAmount - slotTo.Product.Amount, placingProduct.Amount);
            slotTo.ChangeProductAmount(amountToAdd);
            slotFrom.ChangeProductAmount(-amountToAdd);
        }
    }

    private void AddOnNew(ProductSlotModel slotFrom, ProductSlotModel slotTo)
    {
        var placingProduct = slotFrom.Product;
        if (slotTo.HasProduct == false)
        {
            var maxSlotAmount = CalculationHelper.GetAmountForProductInVolume(placingProduct.Config, slotTo.Volume);
            var amountToAdd = Math.Min(maxSlotAmount, placingProduct.Amount);
            slotTo.SetProduct(new ProductModel(placingProduct.Config, amountToAdd));
            slotFrom.ChangeProductAmount(-amountToAdd);
        }
    }
}
