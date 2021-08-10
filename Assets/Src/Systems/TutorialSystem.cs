using System;
using System.Collections.Generic;
using System.Linq;

public class TutorialSystem
{
    private const int LastTutorialStepIndex = 5;

    private readonly GameStateModel _gameStateModel;
    private readonly Dispatcher _dispatcher;
    private readonly PlayerModelHolder _playerModelHolder;
    private readonly List<int> _openTutorialSteps;
    private readonly Dictionary<int, int[]> _tutorialSpecialOpenStepsLogic = new Dictionary<int, int[]>();

    private UserModel _playerModel;

    public TutorialSystem()
    {
        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;

        _openTutorialSteps = new List<int>(5) { 0 };
    }

    public async void Start()
    {
        await _gameStateModel.GameDataLoadedTask;

        _playerModel = _playerModelHolder.UserModel;
        UpdateOpenedSteps();
        Activate();
    }

    private void Activate()
    {
        _gameStateModel.GameStateChanged += OnGameStateChanged;
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
        _gameStateModel.PopupShown += OnPopupShown;
        _gameStateModel.PopupRemoved += OnPopupRemoved;
        _gameStateModel.TutorialStepRemoved += OnTutorialStepRemoved;
        _dispatcher.TutorialActionPerformed += OnUITutorialCloseClicked;
    }

    private void OnUITutorialCloseClicked()
    {
        _playerModel.AddPassedTutorialStep(_gameStateModel.ShowingTutorialModel.StepIndex);
        UpdateOpenedSteps();

        _gameStateModel.RemoveCurrentTutorialStepIfNeeded();
    }

    private void UpdateOpenedSteps()
    {
        var i = 0;
        while (i < _openTutorialSteps.Count)
        {
            var stepIndex = _openTutorialSteps[i];
            if (stepIndex < LastTutorialStepIndex
                && _playerModel.IsTutorialStepPassed(stepIndex))
            {
                if (_tutorialSpecialOpenStepsLogic.ContainsKey(stepIndex) == false)
                {
                    _openTutorialSteps.RemoveAt(i);
                    _openTutorialSteps.Add(stepIndex + 1);
                    continue;
                }
                else
                {
                    //todo: use _tutorialSpecialOpenStepsLogic to add few steps
                }
            }
            i++;
        }
    }

    private bool ShowTutorialIfNeeded(bool immediateMode = false)
    {
        if (_gameStateModel.ShowingTutorialModel != null) return false;
        foreach (var tutorialStepIndex in _openTutorialSteps)
        {
            if (CheckTutorialConditions(tutorialStepIndex))
            {
                _gameStateModel.ShowTutorialStep(new TutorialStepViewModel(tutorialStepIndex, immediateMode));
                return true;
            }
        }

        return false;
    }

    private bool CheckTutorialConditions(int tutorialStepIndex)
    {
        if (_gameStateModel.ShowingTutorialModel != null) return false;
        return tutorialStepIndex switch
        {
            0 => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.ShopSimulation),
            1 => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.ShopSimulation)
                && _gameStateModel.BottomPanelViewModel.SimulationModeTab != BottomPanelSimulationModeTab.Warehouse,
            2 => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.ShopSimulation)
                && _gameStateModel.BottomPanelViewModel.SimulationModeTab == BottomPanelSimulationModeTab.Warehouse
                && _playerModel.ShopModel.WarehouseModel.Slots.Any(s => !s.HasProduct),
            _ => false//throw new ArgumentException($"CheckTutorialConditions: {nameof(tutorialStepIndex)} {tutorialStepIndex} is not supported"),
        };
    }

    private bool HasNoOpenedPopups()
    {
        return _gameStateModel.ShowingPopupModel == null;
    }

    private bool HasNoPlacingMode()
    {
        return _gameStateModel.PlacingState == PlacingStateName.None;
    }

    private bool CheckGameState(GameStateName stateName)
    {
        return _gameStateModel.GameState == stateName;
    }

    private void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        ShowTutorialIfNeeded();
    }

    private void OnPlacingStateChanged(PlacingStateName prevState, PlacingStateName currentState)
    {
        ShowTutorialIfNeeded();
    }

    private void OnPopupShown()
    {
        ShowTutorialIfNeeded();
    }

    private void OnPopupRemoved()
    {
        ShowTutorialIfNeeded();
    }

    private void OnTutorialStepRemoved()
    {
        ShowTutorialIfNeeded(immediateMode: true);
    }
}
