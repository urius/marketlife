public struct UIWarehousePopupSlotClickedCommand
{
    public void Execute(WarehousePopupViewModel popupModel, int warehouseSlotIndex)
    {
        var targetShelfSlot = popupModel.TargetShelfSlot;
        var gameStateModel = GameStateModel.Instance;

        new PutWarehouseProductOnShelfCommand().Execute(warehouseSlotIndex, targetShelfSlot);

        gameStateModel.RemoveCurrentPopupIfNeeded();
    }
}
