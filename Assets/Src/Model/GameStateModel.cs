using System;
using System.Threading.Tasks;

public class GameStateModel
{
    private static Lazy<GameStateModel> _instance = new Lazy<GameStateModel>();
    public static GameStateModel Instance => _instance.Value;

    public event Action<GameStateName, GameStateName> GameStateChanged = delegate { };
    public event Action<PlacingStateName, PlacingStateName> PlacingStateChanged = delegate { };
    public event Action<ShopModel> ViewingShopModelChanged = delegate { };
    public event Action PlayerShopModelWasSet = delegate { };
    public event Action HighlightShopObjectChanged = delegate { };

    private TaskCompletionSource<bool> _dataLoadedTcs = new TaskCompletionSource<bool>();
    public Task GameDataLoadedTask => _dataLoadedTcs.Task;
    public GameStateName GameState { get; private set; } = GameStateName.Initializing;
    public PlacingStateName PlacingState { get; private set; } = PlacingStateName.None;
    public int PlacingDecorationNumericId { get; private set; }
    public ShopObjectModelBase PlacingShopObjectModel { get; private set; }
    public ShopModel ViewingShopModel { get; private set; }
    public ShopModel PlayerShopModel { get; private set; }
    public ShopObjectModelBase HighlightedShopObject { get; private set; }

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

    public void SetHighlightedShopObject(ShopObjectModelBase shopObjectModel)
    {
        if (HighlightedShopObject != null)
        {
            HighlightedShopObject.TriggerHighlighted(false);
        }

        HighlightedShopObject = shopObjectModel;

        if (HighlightedShopObject != null)
        {
            HighlightedShopObject.TriggerHighlighted(true);
        }
        HighlightShopObjectChanged();
    }

    private void SetPlacingState(PlacingStateName newState)
    {
        var previousState = PlacingState;
        PlacingState = newState;
        PlacingStateChanged(previousState, newState);
    }
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
