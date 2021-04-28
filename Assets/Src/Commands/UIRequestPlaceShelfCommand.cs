public struct UIRequestPlaceShelfCommand
{
    public void Execute(int shelfLevelId)
    {
        var shelfConfig = GameConfigManager.Instance.MainConfig.GetShelfConfigByLevelId(shelfLevelId);
        var gameStateModel = GameStateModel.Instance;

        if (shelfConfig.price != null)// TODO check price properly
        {
            var mouseCellCoords = MouseCellCoordsProvider.Instance.MouseCellCoords;
            var model = new ShopObjectModelFactory().CreateShelf(shelfLevelId, mouseCellCoords);
            gameStateModel.SetPlacingObject(model);
        }
    }
}
