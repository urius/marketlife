using System;

public class TutorialSystem
{
    private readonly GameStateModel _gameStateModel;
    private readonly Dispatcher _dispatcher;

    public TutorialSystem()
    {
        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public async void Start()
    {
        await _gameStateModel.GameDataLoadedTask;

        Activate();
    }

    private void Activate()
    {
        _gameStateModel.GameStateChanged += OnGameStateChanged;
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
        _gameStateModel.PopupShown += OnPopupShown;
        _gameStateModel.PopupRemoved += OnPopupRemoved;
        _gameStateModel.TutorialStepRemoved += OnTutorialStepRemoved;
    }

    private void CheckTutorial()
    {
        //check for show tutorial steps
    }

    private void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        CheckTutorial();
    }

    private void OnPlacingStateChanged(PlacingStateName prevState, PlacingStateName currentState)
    {
        CheckTutorial();
    }

    private void OnPopupShown()
    {
        CheckTutorial();
    }

    private void OnPopupRemoved()
    {
        CheckTutorial();
    }

    private void OnTutorialStepRemoved()
    {
        CheckTutorial();
    }
}
