public struct RemoveProductFromShelfCommand
{
    public void Execute(ShelfContentPopupViewModel popupModel, int shelfSlotIndex)
    {
        var shelfModel = popupModel.ShelfModel;
        var targetShelfSlot = shelfModel.Slots[shelfSlotIndex];
        var targetProduct = targetShelfSlot.Product;
        var warehouseModel = PlayerModelHolder.Instance.ShopModel.WarehouseModel;
        var audioManager = AudioManager.Instance;

        var amountToRemoveFromShelf = warehouseModel.AddProduct(targetProduct.Config, targetProduct.Amount);
        if (amountToRemoveFromShelf > 0)
        {
            targetShelfSlot.ChangeProductAmount(-amountToRemoveFromShelf);
            audioManager.PlaySound(SoundNames.Remove2);
        } else
        {
            audioManager.PlaySound(SoundNames.Negative1);
        }
    }
}
