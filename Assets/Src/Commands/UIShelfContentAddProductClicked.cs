public struct UIShelfContentAddProductClicked
{
    public void Execute(ShelfContentPopupViewModel popupModel, int shelfSlotIndex)
    {
        var shelfSlot = popupModel.ShelfModel.Slots[shelfSlotIndex];

        var gameStateModel = GameStateModel.Instance;
        gameStateModel.ShowPopup(new WarehousePopupViewModel(shelfSlot));
    }
}
