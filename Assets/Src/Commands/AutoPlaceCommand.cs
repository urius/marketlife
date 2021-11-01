using System;
using System.Collections.Generic;

public struct AutoPlaceCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var viewingShopModel = gameStateModel.ViewingShopModel;
        var playerShopModel = PlayerModelHolder.Instance.UserModel.ShopModel;
        var audioManager = AudioManager.Instance;
        var analyticsManager = AnalyticsManager.Instance;
        var isSuccess = false;

        var warehouseSlot = playerShopModel.WarehouseModel.Slots[gameStateModel.PlacingProductWarehouseSlotIndex];

        var productsAddedCount = 0;
        var notEmptyShelfSlots = GetFilteredShelfSlots(FilterNotEmpty);
        foreach (var shelfSlot in notEmptyShelfSlots)
        {
            if (shelfSlot.Product.NumericId == warehouseSlot.Product.NumericId)
            {
                productsAddedCount += new PutWarehouseProductOnShelfCommand().Execute(warehouseSlot.Index, shelfSlot);
                if (warehouseSlot.HasProduct == false) break;
            }
        }
        var emptyShelfSlots = GetFilteredShelfSlots(FilterEmpty);
        foreach (var shelfSlot in emptyShelfSlots)
        {
            productsAddedCount += new PutWarehouseProductOnShelfCommand().Execute(warehouseSlot.Index, shelfSlot);
            if (warehouseSlot.HasProduct == false) break;
        }

        if (productsAddedCount > 0)
        {
            audioManager.PlaySound(SoundNames.ProductPut);
            isSuccess = true;
        }

        gameStateModel.ResetActionState();

        analyticsManager.SendCustom(AnalyticsManager.EventAutoPlaceClick, ("is_success", isSuccess));
    }

    private IEnumerable<ProductSlotModel> GetFilteredShelfSlots(Func<ProductSlotModel, bool> filterFunc)
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var shopModel = playerModel.ShopModel;

        foreach (var kvp in shopModel.ShopObjects)
        {
            if (kvp.Value.Type == ShopObjectType.Shelf)
            {
                var shelfModel = kvp.Value as ShelfModel;
                foreach (var shelfSlot in shelfModel.Slots)
                {
                    if (filterFunc(shelfSlot) == true) yield return shelfSlot;
                }
            }
        }
    }

    private bool FilterNotEmpty(ProductSlotModel slotModel)
    {
        return slotModel.HasProduct;
    }

    private bool FilterEmpty(ProductSlotModel slotModel)
    {
        return !slotModel.HasProduct;
    }
}
