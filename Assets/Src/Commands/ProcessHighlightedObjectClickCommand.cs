public struct ProcessHighlightedObjectClickCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;

        if (gameStateModel.GameState == GameStateName.PlayerShopSimulation
               && gameStateModel.ShowingPopupModel == null)
        {
            ProcessHighlightedObjectInternal();
        }
    }

    private void ProcessHighlightedObjectInternal()
    {
        var gameStateModel = GameStateModel.Instance;
        var highlightState = gameStateModel.HighlightState;

        if (highlightState.HighlightedShopObject is ShelfModel highlightedShelf)
        {
            gameStateModel.ShowPopup(new ShelfContentPopupViewModel(highlightedShelf));
        }
        else if (highlightState.IsHighlightedUnwash)
        {
            ProcessHighlightedUnwash();
        }
    }

    private void ProcessHighlightedUnwash()
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var gameStateModel = GameStateModel.Instance;
        var highlightState = gameStateModel.HighlightState;
        var shopModel = gameStateModel.ViewingShopModel;
        var mainConfig = GameConfigManager.Instance.MainConfig;
        var dispatcher = Dispatcher.Instance;
        var screenCalculator = ScreenCalculator.Instance;
        var audioManager = AudioManager.Instance;

        var coords = highlightState.HighlightedCoords;
        shopModel.RemoveUnwash(coords);
        playerModel.ProgressModel.AddExp(mainConfig.RemoveUnwashesRewardExp);

        gameStateModel.ResetHighlightedState();

        var screenPoint = screenCalculator.CellToScreenPoint(coords);
        dispatcher.UIRequestFlyingExp(screenPoint, mainConfig.RemoveUnwashesRewardExp);

        audioManager.PlayRandomSound(SoundNames.Clean1, SoundNames.Clean2, SoundNames.Clean3, SoundNames.Clean4);
    }
}
