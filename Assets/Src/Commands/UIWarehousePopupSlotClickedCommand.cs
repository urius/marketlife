public struct UIWarehousePopupSlotClickedCommand
{
    public void Execute(int warehouseSlotIndex)
    {
        var gameStateModel = GameStateModel.Instance;
        var viewingUserModel = gameStateModel.ViewingUserModel;

        if (gameStateModel.GameState == GameStateName.PlayerShopSimulation)
        {
            var popupModel = gameStateModel.GetFirstPopupOfTypeOrDefault(PopupType.WarehouseForShelf) as WarehousePopupForShelfViewModel;
            new PutWarehouseProductOnShelfCommand().Execute(warehouseSlotIndex, viewingUserModel, popupModel.TargetShelfModel, popupModel.TargetShelfSlotIndex);
            gameStateModel.RemoveCurrentPopupIfNeeded();
        }
        else if (gameStateModel.GameState == GameStateName.ShopFriend)
        {
            if (gameStateModel.GetFirstPopupOfTypeOrDefault(PopupType.Warehouse) is WarehousePopupViewModel popupModel)
            {
                gameStateModel.SetPlacingProductOnFriendsShopAction(warehouseSlotIndex);
                gameStateModel.RemoveCurrentPopupIfNeeded();
            }
        }
    }
}
