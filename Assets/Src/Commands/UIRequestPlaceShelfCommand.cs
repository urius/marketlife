public struct UIRequestPlaceShelfCommand
{
    public void Execute(int shelfNumericId)
    {
        var shelfConfig = GameConfigManager.Instance.MainConfig.GetShelfConfigByNumericId(shelfNumericId);
        var gameStateModel = GameStateModel.Instance;
        Dispatcher.Instance.RequestForceMouseCellPositionUpdate();

        if (shelfConfig.price != null)// TODO check price properly
        {
            var mouseCellCoords = MouseCellCoordsProvider.Instance.MouseCellCoords;
            var model = new ShopObjectModelFactory().CreateShelf(shelfNumericId, mouseCellCoords);
            gameStateModel.SetPlacingObject(model);
        }
    }
}
