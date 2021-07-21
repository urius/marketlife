using System;

public struct RemoveProductFromShelfCommand
{
    public void Execute(ShelfContentPopupViewModel popupModel, int shelfSlotIndex)
    {
        var shelfModel = popupModel.ShelfModel;
        var targetShelfSlot = shelfModel.Slots[shelfSlotIndex];
        var targetProduct = targetShelfSlot.Product;
        var warehouseModel = GameStateModel.Instance.PlayerShopModel.WarehouseModel;
        var audioManager = AudioManager.Instance;

        var removedProductAmount = 0;
        foreach (var slot in warehouseModel.Slots)
        {
            if (slot.HasProduct
                && slot.Product.NumericId == targetProduct.NumericId)
            {
                var restAmount = slot.GetRestAmount();
                if (restAmount > 0)
                {
                    var amountToRemove = Math.Min(targetProduct.Amount - removedProductAmount, restAmount);
                    slot.Product.Amount += amountToRemove;
                    removedProductAmount += amountToRemove;
                }
            }
            if (removedProductAmount >= targetProduct.Amount) break;
        }

        foreach (var slot in warehouseModel.Slots)
        {
            if (!slot.HasProduct)
            {
                var maxAmount = CalculationHelper.GetAmountForProductInVolume(targetProduct.Config, slot.Volume);
                if (maxAmount > 0)
                {
                    var amountToRemove = Math.Min(targetProduct.Amount - removedProductAmount, maxAmount);
                    slot.SetProduct(new ProductModel(targetProduct.Config, amountToRemove));
                    removedProductAmount += amountToRemove;
                }
            }
            if (removedProductAmount >= targetProduct.Amount) break;
        }

        if (removedProductAmount > 0)
        {
            targetShelfSlot.ChangeProductAmount(-removedProductAmount);
            audioManager.PlaySound(SoundNames.Remove2);
        }
    }
}
