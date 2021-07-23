public struct UIProcessWarehouseSlotClickCommand
{
    public void Execute(int slotIndex)
    {
        var gameStateModel = GameStateModel.Instance;
        var playerModelHolder = PlayerModelHolder.Instance;
        var warehouseModel = playerModelHolder.ShopModel.WarehouseModel;

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
