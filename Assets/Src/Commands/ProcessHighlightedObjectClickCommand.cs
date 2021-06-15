public struct ProcessHighlightedObjectClickCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var highlightState = gameStateModel.HighlightState;

        if (gameStateModel.GameState == GameStateName.ShopSimulation
               && gameStateModel.ShowingPopupModel == null)
        {
            if (highlightState.HighlightedShopObject is ShelfModel highlightedShelf)
            {
                gameStateModel.ShowPopup(new ShelfContentPopupViewModel(highlightedShelf));
            }
        }
    }
}
