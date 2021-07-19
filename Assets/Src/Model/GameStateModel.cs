using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameStateModel
{
    private static Lazy<GameStateModel> _instance = new Lazy<GameStateModel>();
    public static GameStateModel Instance => _instance.Value;

    public event Action<GameStateName, GameStateName> GameStateChanged = delegate { };
    public event Action<PlacingStateName, PlacingStateName> PlacingStateChanged = delegate { };
    public event Action<ShopModel> ViewingShopModelChanged = delegate { };
    public event Action PlayerShopModelWasSet = delegate { };
    public event Action HighlightStateChanged = delegate { };
    public event Action PopupShown = delegate { };

    public event Action PopupRemoved = delegate { };

    private TaskCompletionSource<bool> _dataLoadedTcs = new TaskCompletionSource<bool>();
    private int _placingIntParameter = -1;
    private readonly Stack<PopupViewModelBase> _showingPopupModelsStack = new Stack<PopupViewModelBase>();

    public Task GameDataLoadedTask => _dataLoadedTcs.Task;
    public GameStateName GameState { get; private set; } = GameStateName.Initializing;
    public bool IsRealtimeState => GameState == GameStateName.ShopSimulation;
    public PlacingStateName PlacingState { get; private set; } = PlacingStateName.None;
    public int PlacingDecorationNumericId => _placingIntParameter;
    public int PlacingProductWarehouseSlotIndex => _placingIntParameter;
    public ShopObjectModelBase PlacingShopObjectModel { get; private set; }
    public ShopModel ViewingShopModel { get; private set; }
    public ShopModel PlayerShopModel { get; private set; }
    public HighlightState HighlightState { get; private set; } = HighlightState.Default;
    public PopupViewModelBase ShowingPopupModel => _showingPopupModelsStack.Count > 0 ? _showingPopupModelsStack.Peek() : null;
    public int ServerTime
    {
        get
        {
            var delta = (int)(Time.realtimeSinceStartup - _realtimeSinceStartupCheckpoint);
            var result = _lastCheckedServerTime + delta;
            return result;
        }
    }

    private int _lastCheckedServerTime;
    private float _realtimeSinceStartupCheckpoint;

    public void SetServerTime(int serverTime)
    {
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
        PopupShown();
    }


    public void RemoveCurrentPopupIfNeeded()
    {
        if (ShowingPopupModel != null)
        {
            _showingPopupModelsStack.Pop();
            PopupRemoved();
        }
    }

    public void ResetPlacingState()
    {
        PlacingShopObjectModel = null;
        _placingIntParameter = -1;
        SetPlacingState(PlacingStateName.None);
    }

    public void SetPlacingObject(ShopObjectModelBase placingObjectModel, bool isNew = true)
    {
        PlacingShopObjectModel = placingObjectModel;
        SetPlacingState(isNew ? PlacingStateName.PlacingNewShopObject : PlacingStateName.MovingShopObject);
    }

    public void SetPlacingFloor(int numericId)
    {
        _placingIntParameter = numericId;
        SetPlacingState(PlacingStateName.PlacingNewFloor);
    }

    public void SetPlacingWall(int numericId)
    {
        _placingIntParameter = numericId;
        SetPlacingState(PlacingStateName.PlacingNewWall);
    }

    public void SetPlacingWindow(int numericId, bool isNew = true)
    {
        _placingIntParameter = numericId;
        SetPlacingState(isNew ? PlacingStateName.PlacingNewWindow : PlacingStateName.MovingWindow);
    }

    public void SetPlacingDoor(int numericId, bool isNew = true)
    {
        _placingIntParameter = numericId;
        SetPlacingState(isNew ? PlacingStateName.PlacingNewDoor : PlacingStateName.MovingDoor);
    }

    public void SetPlacingProductSlotIndex(int slotIndex)
    {
        _placingIntParameter = slotIndex;
        SetPlacingState(PlacingStateName.PlacingProduct);
    }

    public void SetViewingShopModel(ShopModel shopModel)
    {
        ViewingShopModel = shopModel;
        ViewingShopModelChanged(ViewingShopModel);
    }

    public void SetPlayerShopModel(ShopModel shopModel)
    {
        if (PlayerShopModel != null)
        {
            throw new InvalidOperationException("GameStateModel.SetPlayerShopModel(): PlayerShopModel already setup");
        }
        PlayerShopModel = shopModel;
        PlayerShopModelWasSet();
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

    private void SetPlacingState(PlacingStateName newState)
    {
        var previousState = PlacingState;
        PlacingState = newState;
        PlacingStateChanged(previousState, newState);
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
    ShopSimulation,
    ShopInterior,
}

public enum PlacingStateName
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
    PlacingProduct,
}
