public struct RequestRemovePopupCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var highlightedShopObject = gameStateModel.HighlightState.HighlightedShopObject;
        if (highlightedShopObject != null)
        {
            gameStateModel.ResetHighlightedState();
            gameStateModel.ShowPopup(new RemoveShopObjectPopupViewModel(highlightedShopObject));
        }
    }
}
