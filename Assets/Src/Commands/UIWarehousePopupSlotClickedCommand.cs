using System;

public struct UIWarehousePopupSlotClickedCommand
{
    public void Execute(WarehousePopupViewModel popupModel, int warehouseSlotIndex)
    {
        var targetShelfSlot = popupModel.TargetShelfSlot;

        var gameStateModel = GameStateModel.Instance;
        var warehouseTargetSlot = gameStateModel.PlayerShopModel.WarehouseModel.Slots[warehouseSlotIndex];
        if (warehouseTargetSlot.HasProduct)
        {
            var targetProduct = warehouseTargetSlot.Product;
            var amountMax = CalculationHelper.GetAmountForProductInVolume(targetProduct.Config, targetShelfSlot.Volume);
            var amountToAddOnShelf = Math.Min(amountMax, targetProduct.Amount);
            var productToAdd = new ProductModel(targetProduct.Config, amountToAddOnShelf);
            targetShelfSlot.SetProduct(productToAdd);
            warehouseTargetSlot.ChangeProductAmount(-amountToAddOnShelf);

            gameStateModel.RemoveCurrentPopupIfNeeded();
        }
    }
}
