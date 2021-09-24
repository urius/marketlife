using System;
using System.Collections.Generic;
using System.Linq;

public class TutorialSystem
{
    private const int LastTutorialStepIndex = 15;

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
        _gameStateModel.ActionStateChanged += OnActionStateChanged;
        _gameStateModel.PopupShown += OnPopupShown;
        _gameStateModel.PopupRemoved += OnPopupRemoved;
        _dispatcher.TutorialActionPerformed += OnUITutorialActionPerformed;
    }

    private void OnUITutorialActionPerformed()
    {
        _playerModel.AddPassedTutorialStep(_gameStateModel.ShowingTutorialModel.StepIndex);
        UpdateOpenedSteps();

        _gameStateModel.RemoveCurrentTutorialStepIfNeeded();
        ShowTutorialIfNeeded(immediateMode: true);
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
        return (TutorialStep)tutorialStepIndex switch
        {
            TutorialStep.Welcome => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.ShopSimulation),
            TutorialStep.OpenWarehouse => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.ShopSimulation)
                && _gameStateModel.BottomPanelViewModel.SimulationModeTab != BottomPanelSimulationModeTab.Warehouse,
            TutorialStep.OpenOrderPopup => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.ShopSimulation)
                && _gameStateModel.BottomPanelViewModel.SimulationModeTab == BottomPanelSimulationModeTab.Warehouse
                && _playerModel.ShopModel.WarehouseModel.Slots.Any(s => !s.HasProduct),
            TutorialStep.OrderProduct => HasNoOpenedPopups() == false
                && _gameStateModel.ShowingPopupModel.PopupType == PopupType.OrderProduct,
            TutorialStep.Delivering => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.ShopSimulation)
                && _playerModel.ShopModel.WarehouseModel.Slots.Any(s => s.HasProduct && s.Product.DeliverTime > _gameStateModel.ServerTime),
            TutorialStep.PlacingProduct => HasNoOpenedPopups()
                && _gameStateModel.ActionState == ActionStateName.PlacingProduct,
            TutorialStep.FinishPlacingProduct => HasNoOpenedPopups()
                && HasNoPlacingMode(),
            TutorialStep.ShowMoodInteriorAndFriendsUI => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.ShopSimulation),
            TutorialStep.ShowSaveIcon => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.ShopSimulation),
            TutorialStep.ReadyToPlay => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.ShopSimulation),
            TutorialStep.FriendUI => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.ShopFriend),
            _ => false//throw new ArgumentException($"CheckTutorialConditions: {nameof(tutorialStepIndex)} {tutorialStepIndex} is not supported"),
        };
    }

    private bool HasNoOpenedPopups()
    {
        return _gameStateModel.ShowingPopupModel == null;
    }

    private bool HasNoPlacingMode()
    {
        return _gameStateModel.ActionState == ActionStateName.None;
    }

    private bool CheckGameState(GameStateName stateName)
    {
        return _gameStateModel.GameState == stateName;
    }

    private void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        ShowTutorialIfNeeded();
    }

    private void OnActionStateChanged(ActionStateName prevState, ActionStateName currentState)
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
}

public enum TutorialStep
{
    Welcome = 0,
    OpenWarehouse = 1,
    OpenOrderPopup = 2,
    OrderProduct = 3,
    Delivering = 4,
    PlacingProduct = 5,
    FinishPlacingProduct = 6,
    ShowMoodInteriorAndFriendsUI = 7,
    ShowSaveIcon = 8,
    ReadyToPlay = 9,
    FriendUI = 10,
}

