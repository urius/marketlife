using System;
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

    private TaskCompletionSource<bool> _dataLoadedTcs = new TaskCompletionSource<bool>();
    public Task GameDataLoadedTask => _dataLoadedTcs.Task;
    public GameStateName GameState { get; private set; } = GameStateName.Initializing;
    public PlacingStateName PlacingState { get; private set; } = PlacingStateName.None;
    public int PlacingDecorationNumericId { get; private set; }
    public ShopObjectModelBase PlacingShopObjectModel { get; private set; }
    public ShopModel ViewingShopModel { get; private set; }
    public ShopModel PlayerShopModel { get; private set; }
    public HighlightState HighlightState { get; private set; } = HighlightState.Default;

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

    public void ResetPlacingState()
    {
        PlacingShopObjectModel = null;
        PlacingDecorationNumericId = 0;
        SetPlacingState(PlacingStateName.None);
    }

    public void SetPlacingObject(ShopObjectModelBase placingObjectModel, bool isNew = true)
    {
        PlacingShopObjectModel = placingObjectModel;
        SetPlacingState(isNew ? PlacingStateName.PlacingNewShopObject : PlacingStateName.MovingShopObject);
    }

    public void SetPlacingFloor(int numericId)
    {
        PlacingDecorationNumericId = numericId;
        SetPlacingState(PlacingStateName.PlacingNewFloor);
    }

    public void SetPlacingWall(int numericId)
    {
        PlacingDecorationNumericId = numericId;
        SetPlacingState(PlacingStateName.PlacingNewWall);
    }

    public void SetPlacingWindow(int numericId, bool isNew = true)
    {
        PlacingDecorationNumericId = numericId;
        SetPlacingState(isNew ? PlacingStateName.PlacingNewWindow : PlacingStateName.MovingWindow);
    }

    public void SetPlacingDoor(int numericId, bool isNew = true)
    {
        PlacingDecorationNumericId = numericId;
        SetPlacingState(isNew ? PlacingStateName.PlacingNewDoor : PlacingStateName.MovingDoor);
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
            IsHighlighted = true,
            HighlightedDecorationCoords = Vector2Int.zero,
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
            IsHighlighted = true,
            HighlightedDecorationCoords = coords,
            HighlightedShopObject = null,
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
    public bool IsHighlighted;
    public Vector2Int HighlightedDecorationCoords;
    public ShopObjectModelBase HighlightedShopObject;

    public static HighlightState Default => new HighlightState()
    {
        IsHighlighted = false,
        HighlightedDecorationCoords = Vector2Int.zero,
        HighlightedShopObject = null
    };

    public bool IsHighlightedDecoration => IsHighlighted == true && HighlightedDecorationCoords != Vector2Int.zero;
}

public enum GameStateName
{
    Undefined,
    Initializing,
    Loading,
    Loaded,
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
}
