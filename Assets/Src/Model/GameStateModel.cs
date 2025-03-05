using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Src.Model.Configs;
using Src.Model.Popups;
using Src.Model.ShopObjects;
using Src.Model.Tutorial;
using UnityEngine;

namespace Src.Model
{
    public class GameStateModel
    {
        private static Lazy<GameStateModel> _instance = new Lazy<GameStateModel>();
        public static GameStateModel Instance => _instance.Value;

        public event Action<GameStateName, GameStateName> GameStateChanged = delegate { };
        public event Action PausedStateChanged = delegate { };
        public event Action<ActionStateName, ActionStateName> ActionStateChanged = delegate { };
        public event Action<UserModel> ViewingUserModelChanged = delegate { };
        public event Action HighlightStateChanged = delegate { };
        public event Action PopupShown = delegate { };
        public event Action<PopupViewModelBase> PopupRemoved = delegate { };
        public event Action TutorialStepShown = delegate { };
        public event Action TutorialStepRemoved = delegate { };

        public readonly BottomPanelViewModel BottomPanelViewModel;

        public BankConfigItem ChargedBankItem;

        private readonly Stack<PopupViewModelBase> _showingPopupModelsStack;
        private readonly Dictionary<PopupType, PopupViewModelBase> _popupViewModelsCache = new Dictionary<PopupType, PopupViewModelBase>();

        private TaskCompletionSource<bool> _dataLoadedTcs = new TaskCompletionSource<bool>();
        private int _placingIntParameter = -1;
        private int _lastCheckedServerTime;
        private float _realtimeSinceStartupCheckpoint;

        public GameStateModel()
        {
            BottomPanelViewModel = new BottomPanelViewModel();
            _showingPopupModelsStack = new Stack<PopupViewModelBase>();
        }

        public Task GameDataLoadedTask => _dataLoadedTcs.Task;
        public bool IsGamePaused { get; private set; } = false;
        public GameStateName GameState { get; private set; } = GameStateName.Initializing;
        public bool IsSimulationState => GameState == GameStateName.PlayerShopSimulation || GameState == GameStateName.ShopFriend;
        public bool IsPlayingState => CheckIsPlayingState(GameState);
        public ActionStateName ActionState { get; private set; } = ActionStateName.None;
        public int PlacingDecorationNumericId => _placingIntParameter;
        public int PlacingProductWarehouseSlotIndex => _placingIntParameter;
        public ShopObjectModelBase PlacingShopObjectModel { get; private set; }
        public UserModel ViewingUserModel { get; private set; }
        public ShopModel ViewingShopModel => ViewingUserModel?.ShopModel;
        public HighlightState HighlightState { get; private set; } = HighlightState.Default;
        public PopupViewModelBase ShowingPopupModel => _showingPopupModelsStack.Count > 0 ? _showingPopupModelsStack.Peek() : null;
        public TutorialStepViewModel ShowingTutorialModel { get; private set; }
        public int StartGameServerTime { get; private set; }
        public int ServerTime
        {
            get
            {
                var delta = (int)(Time.realtimeSinceStartup - _realtimeSinceStartupCheckpoint);
                var result = _lastCheckedServerTime + delta;
                return result;
            }
        }

        public bool IsServerTimeValid => _lastCheckedServerTime > 0;

        public bool CheckIsPlayingState(GameStateName state)
        {
            switch (state)
            {
                case GameStateName.PlayerShopSimulation:
                case GameStateName.PlayerShopInterior:
                case GameStateName.ShopFriend:
                    return true;
            }
            return false;
        }

        public void SetServerTime(int serverTime)
        {
            if (StartGameServerTime <= 0) StartGameServerTime = serverTime;
            _lastCheckedServerTime = serverTime;
            _realtimeSinceStartupCheckpoint = Time.realtimeSinceStartup;
        }

        public void SetGameState(GameStateName newState)
        {
            if (newState == GameState) return;

            if (newState == GameStateName.Loaded)
            {
                _dataLoadedTcs.TrySetResult(true);
            }

            var previousState = GameState;
            GameState = newState;
            GameStateChanged(previousState, newState);
        }

        public void ShowPopup(PopupViewModelBase popupModel)
        {
            _showingPopupModelsStack.Push(popupModel);
            UpdatePausedState();
            PopupShown();
        }

        public void CachePopup(PopupViewModelBase popupModel)
        {
            _popupViewModelsCache[popupModel.PopupType] = popupModel;
        }

        public PopupViewModelBase GetPopupFromCache(PopupType popupType)
        {
            if (_popupViewModelsCache.ContainsKey(popupType))
            {
                return _popupViewModelsCache[popupType];
            }
            return null;
        }

        public PopupViewModelBase GetFirstPopupOfTypeOrDefault(PopupType popupType)
        {
            foreach (var popupModel in _showingPopupModelsStack)
            {
                if (popupModel.PopupType == popupType) return popupModel;
            }
            return null;
        }

        public void RemoveCurrentPopupIfNeeded()
        {
            if (ShowingPopupModel != null)
            {
                ShowingPopupModel.Dispose();
                var removedPopupModel = _showingPopupModelsStack.Pop();
                UpdatePausedState();
                PopupRemoved(removedPopupModel);
            }
        }

        public void ShowTutorialStep(TutorialStepViewModel tutorialStepViewModel)
        {
            RemoveCurrentTutorialStepIfNeeded();
            ShowingTutorialModel = tutorialStepViewModel;
            UpdatePausedState();
            TutorialStepShown();
        }

        public void RemoveCurrentTutorialStepIfNeeded()
        {
            if (ShowingTutorialModel != null)
            {
                ShowingTutorialModel = null;
                UpdatePausedState();
                TutorialStepRemoved();
            }
        }

        private void UpdatePausedState()
        {
            var isPausedBefore = IsGamePaused;
            IsGamePaused = _showingPopupModelsStack.Count > 0 || ShowingTutorialModel != null;
            if (IsGamePaused != isPausedBefore)
            {
                PausedStateChanged();
            }
        }

        public void ResetActionState()
        {
            PlacingShopObjectModel = null;
            _placingIntParameter = -1;
            SetActionState(ActionStateName.None);
        }

        public void SetPlacingObject(ShopObjectModelBase placingObjectModel, bool isNew = true)
        {
            PlacingShopObjectModel = placingObjectModel;
            SetActionState(isNew ? ActionStateName.PlacingNewShopObject : ActionStateName.MovingShopObject);
        }

        public void SetPlacingFloor(int numericId)
        {
            _placingIntParameter = numericId;
            SetActionState(ActionStateName.PlacingNewFloor);
        }

        public void SetPlacingWall(int numericId)
        {
            _placingIntParameter = numericId;
            SetActionState(ActionStateName.PlacingNewWall);
        }

        public void SetPlacingWindow(int numericId, bool isNew = true)
        {
            _placingIntParameter = numericId;
            SetActionState(isNew ? ActionStateName.PlacingNewWindow : ActionStateName.MovingWindow);
        }

        public void SetPlacingDoor(int numericId, bool isNew = true)
        {
            _placingIntParameter = numericId;
            SetActionState(isNew ? ActionStateName.PlacingNewDoor : ActionStateName.MovingDoor);
        }

        public void SetPlacingProductOnPlayersShop(int slotIndex)
        {
            _placingIntParameter = slotIndex;
            SetActionState(ActionStateName.PlacingProductPlayer);
        }

        public void SetPlacingProductOnFriendsShopAction(int slotIndex)
        {
            _placingIntParameter = slotIndex;
            SetActionState(ActionStateName.PlacingProductFriend);
        }

        public void SetTakeProductAction()
        {
            SetActionState(ActionStateName.FriendShopTakeProduct);
        }

        public void SetAddUnwashAction()
        {
            SetActionState(ActionStateName.FriendShopAddUnwash);
        }

        public void SetViewingUserModel(UserModel userModel)
        {
            ViewingUserModel = userModel;
            ViewingUserModelChanged(ViewingUserModel);
        }

        public void ResetHighlightedState(bool isSilent = false)
        {
            if (HighlightState.IsHighlighted == false) return;

            if (HighlightState.HighlightedShopObject != null)
            {
                HighlightState.HighlightedShopObject.TriggerHighlighted(false);
            }

            HighlightState = HighlightState.Default;
            if (isSilent == false)
            {
                HighlightStateChanged();
            }
        }

        public void SetHighlightedShopObject(ShopObjectModelBase shopObjectModel)
        {
            if (HighlightState.IsHighlighted && HighlightState.HighlightedShopObject == shopObjectModel)
            {
                return;
            }
            if (HighlightState.HighlightedShopObject != null)
            {
                HighlightState.HighlightedShopObject.TriggerHighlighted(false);
            }

            HighlightState = new HighlightState()
            {
                HighlightedShopObject = shopObjectModel,
            };
            shopObjectModel.TriggerHighlighted(true);

            HighlightStateChanged();
        }

        public void SetHighlightedDecorationOn(Vector2Int coords)
        {
            ResetHighlightedState(isSilent: true);
            HighlightState = new HighlightState()
            {
                IsHighlightedDecoration = true,
                HighlightedCoords = coords,
            };

            HighlightStateChanged();
        }

        public void SetHighlightedUnwashOn(Vector2Int coords)
        {
            ResetHighlightedState(isSilent: true);
            HighlightState = new HighlightState()
            {
                IsHighlightedUnwash = true,
                HighlightedCoords = coords,
            };

            HighlightStateChanged();
        }

        private void SetActionState(ActionStateName newState)
        {
            var previousState = ActionState;
            ActionState = newState;
            ActionStateChanged(previousState, newState);
        }
    }

    public struct HighlightState
    {
        public bool IsHighlightedDecoration;
        public bool IsHighlightedUnwash;
        public ShopObjectModelBase HighlightedShopObject;
        public Vector2Int HighlightedCoords;

        public static HighlightState Default => new HighlightState()
        {
            HighlightedCoords = Vector2Int.zero,
            HighlightedShopObject = null
        };

        public bool IsHighlighted => IsHighlightedDecoration || IsHighlightedUnwash || HighlightedShopObject != null;
    }

    public enum GameStateName
    {
        Undefined,
        Initializing,
        Loading,
        Loaded,
        ReadyForStart,
        PlayerShopSimulation,
        PlayerShopInterior,
        ShopFriend,
    }

    public enum ActionStateName
    {
        None,
        PlacingNewShopObject,
        MovingShopObject,
        PlacingNewFloor,
        PlacingNewWall,
        PlacingNewWindow,
        MovingWindow,
        PlacingNewDoor,
        MovingDoor,
        PlacingProductPlayer,
        PlacingProductFriend,
        FriendShopTakeProduct,
        FriendShopAddUnwash,
    }
}