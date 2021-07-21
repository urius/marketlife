public struct UIProcessWarehouseSlotClickCommand
{
    public void Execute(int slotIndex)
    {
        var gameStateModel = GameStateModel.Instance;
        var warehouseModel = gameStateModel.PlayerShopModel.WarehouseModel;
        var audioManager = AudioManager.Instance;

        var slotModel = warehouseModel.Slots[slotIndex];
        if (slotModel.Product != null)
        {
            if (slotModel.Product.DeliverTime <= gameStateModel.ServerTime)
            {
                gameStateModel.SetPlacingProductSlotIndex(slotIndex);
            }
        }
        else
        {
            gameStateModel.ShowPopup(new OrderProductPopupViewModel(slotIndex));
        }
    }
}
