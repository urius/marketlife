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
        var audioManager = AudioManager.Instance;
        var analyticsManager = AnalyticsManager.Instance;
        var isSuccess = false;

        var productsAddedCount = ForEveryShelfSlot(AddOnExisting);
        productsAddedCount += ForEveryShelfSlot(AddOnNew);

        if (productsAddedCount > 0)
        {
            audioManager.PlaySound(SoundNames.ProductPut);
            isSuccess = true;
        }

        gameStateModel.ResetActionState();

        analyticsManager.SendCustom(AnalyticsManager.EventAutoPlaceClick, ("is_success", isSuccess));
    }

    private int ForEveryShelfSlot(Func<ProductSlotModel, ProductSlotModel, int> action)
    {
        var addedProductsCount = 0;
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
                        addedProductsCount += action(placingProductSlot, shelfSlot);
                        if (placingProductSlot.HasProduct == false) break;
                    }
                }
            }
        }

        return addedProductsCount;
    }

    private int AddOnExisting(ProductSlotModel slotFrom, ProductSlotModel slotTo)
    {
        if (slotTo.HasProduct && slotTo.Product.NumericId == slotFrom.Product.NumericId)
        {
            return new PutWarehouseProductOnShelfCommand().Execute(slotFrom.Index, slotTo);
        }

        return 0;
    }

    private int AddOnNew(ProductSlotModel slotFrom, ProductSlotModel slotTo)
    {
        if (slotTo.HasProduct == false)
        {
            return new PutWarehouseProductOnShelfCommand().Execute(slotFrom.Index, slotTo);
        }

        return 0;
    }
}
