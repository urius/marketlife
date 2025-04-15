using System.Collections.Generic;
using System.Linq;
using Src.Common;

public class TutorialSystem
{
    private readonly GameStateModel _gameStateModel;
    private readonly Dispatcher _dispatcher;
    private readonly PlayerModelHolder _playerModelHolder;
    private readonly AnalyticsManager _analyticsManager;
    private readonly List<TutorialStep> _openTutorialSteps;
    private readonly TutorialStep[][] _tutorialSequences = new TutorialStep[][] {
        new TutorialStep[]{
            TutorialStep.Welcome,
            TutorialStep.OpenWarehouse,
            TutorialStep.OpenOrderPopup,
            TutorialStep.OrderProduct,
            TutorialStep.Delivering,
            TutorialStep.PlacingProduct,
            TutorialStep.FinishPlacingProduct,
            TutorialStep.ShowMoodInteriorAndFriendsUI,
            TutorialStep.ShowSaveIcon,
            TutorialStep.ReadyToPlay,
            TutorialStep.Billboard,
        },
        new TutorialStep[]{
            TutorialStep.FriendUI
        },
    };
    private readonly int[] _tutorialSequencesIndexes;

    private UserModel _playerModel;

    public TutorialSystem()
    {
        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _analyticsManager = AnalyticsManager.Instance;

        _tutorialSequencesIndexes = new int[_tutorialSequences.Length];
        _openTutorialSteps = new List<TutorialStep>(5);
        _openTutorialSteps.AddRange(_tutorialSequences.Select(s => s[0]));
    }

    public async void Start()
    {
        await _gameStateModel.GameDataLoadedTask;
#if UNITY_EDITOR
        if (DebugDataHolder.Instance.IsTutorialDisabled == true) return;
#endif

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
        var stepIndex = _gameStateModel.ShowingTutorialModel.StepIndex;
        _analyticsManager.SendTutorialStep(stepIndex, GetTutorialIdByStepIndex((TutorialStep)stepIndex));
        _playerModel.AddPassedTutorialStep(stepIndex);
        UpdateOpenedSteps();
        _gameStateModel.RemoveCurrentTutorialStepIfNeeded();
        ShowTutorialIfNeeded(immediateMode: true);
    }

    private void UpdateOpenedSteps()
    {
        for (var i = 0; i < _tutorialSequences.Length; i++)
        {
            while (_tutorialSequencesIndexes[i] < _tutorialSequences[i].Length)
            {
                var stepIndex = _tutorialSequencesIndexes[i];
                var sequence = _tutorialSequences[i];
                var step = sequence[stepIndex];
                if (_playerModel.IsTutorialStepPassed((int)step))
                {
                    _openTutorialSteps.Remove(step);
                    _tutorialSequencesIndexes[i]++;
                    stepIndex = _tutorialSequencesIndexes[i];
                    if (stepIndex < sequence.Length)
                    {
                        step = sequence[stepIndex];
                        _openTutorialSteps.Add(step);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    private bool ShowTutorialIfNeeded(bool immediateMode = false)
    {
        if (_gameStateModel.ShowingTutorialModel != null) return false;
        foreach (var tutorialStep in _openTutorialSteps)
        {
            if (CheckTutorialConditions(tutorialStep))
            {
                _gameStateModel.ShowTutorialStep(new TutorialStepViewModel((int)tutorialStep, immediateMode));
                if (tutorialStep == 0)
                {
                    _analyticsManager.SendTutorialStart(GetTutorialIdByStepIndex(tutorialStep));
                }
                return true;
            }
        }

        return false;
    }

    private bool CheckTutorialConditions(TutorialStep tutorialStep)
    {
        if (_gameStateModel.ShowingTutorialModel != null) return false;
        return tutorialStep switch
        {
            TutorialStep.Welcome => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.PlayerShopSimulation),
            TutorialStep.OpenWarehouse => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.PlayerShopSimulation)
                && _gameStateModel.BottomPanelViewModel.SimulationModeTab != BottomPanelSimulationModeTab.Warehouse,
            TutorialStep.OpenOrderPopup => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.PlayerShopSimulation)
                && _gameStateModel.BottomPanelViewModel.SimulationModeTab == BottomPanelSimulationModeTab.Warehouse
                && _playerModel.ShopModel.WarehouseModel.Slots.Any(s => !s.HasProduct),
            TutorialStep.OrderProduct => HasNoOpenedPopups() == false
                && _gameStateModel.ShowingPopupModel.PopupType == PopupType.OrderProduct,
            TutorialStep.Delivering => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.PlayerShopSimulation)
                && _playerModel.ShopModel.WarehouseModel.Slots.Any(s => s.HasProduct && s.Product.DeliverTime > _gameStateModel.ServerTime),
            TutorialStep.PlacingProduct => HasNoOpenedPopups()
                && _gameStateModel.ActionState == ActionStateName.PlacingProductPlayer,
            TutorialStep.FinishPlacingProduct => HasNoOpenedPopups()
                && HasNoPlacingMode(),
            TutorialStep.ShowMoodInteriorAndFriendsUI => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.PlayerShopSimulation),
            TutorialStep.ShowSaveIcon => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.PlayerShopSimulation),
            TutorialStep.ReadyToPlay => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.PlayerShopSimulation),
            TutorialStep.FriendUI => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.ShopFriend),
            TutorialStep.Billboard => HasNoOpenedPopups()
                && HasNoPlacingMode()
                && CheckGameState(GameStateName.PlayerShopSimulation)
                && _playerModel.ShopModel.BillboardModel.IsAvailable,
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

    private void OnPopupRemoved(PopupViewModelBase popupViewModel)
    {
        ShowTutorialIfNeeded();
    }

    private string GetTutorialIdByStepIndex(TutorialStep step)
    {
        return step switch
        {
            TutorialStep.FriendUI => "TutorialFriend",
            _ => "TutorialMain",
        };
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
    Billboard = 11,
}

