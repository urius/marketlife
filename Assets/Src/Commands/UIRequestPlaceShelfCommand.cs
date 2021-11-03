public struct UIRequestPlaceShelfCommand
{
    public void Execute(int shelfNumericId)
    {
        var shelfConfig = GameConfigManager.Instance.MainConfig.GetShelfConfigByNumericId(shelfNumericId);
        var gameStateModel = GameStateModel.Instance;
        var audioManager = AudioManager.Instance;

        if (gameStateModel.ActionState != ActionStateName.None) return;

        Dispatcher.Instance.RequestForceMouseCellPositionUpdate();
        var mouseCellCoords = MouseDataProvider.Instance.MouseCellCoords;
        var model = new ShopObjectModelFactory().CreateShelf(shelfNumericId, mouseCellCoords);
        gameStateModel.SetPlacingObject(model);
    }
}
