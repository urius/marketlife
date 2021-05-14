using System;
using System.Threading.Tasks;

public class GameStateModel
{
    private static Lazy<GameStateModel> _instance = new Lazy<GameStateModel>();
    public static GameStateModel Instance => _instance.Value;

    public event Action<GameStateName, GameStateName> GameStateChanged = delegate { };
    public event Action<PlacingStateName, PlacingStateName> PlacingStateChanged = delegate { };
    public event Action<ShopModel> ViewingShopModelChanged = delegate { };

    private TaskCompletionSource<bool> _dataLoadedTcs = new TaskCompletionSource<bool>();
    public Task GameDataLoadedTask => _dataLoadedTcs.Task;

    public GameStateName GameState { get; private set; } = GameStateName.Initializing;
    public PlacingStateName PlacingState { get; private set; } = PlacingStateName.None;
    public ShopObjectBase PlacingShopObjectModel { get; private set; }
    public ShopModel ViewingShopModel { get; private set; }

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
        SetPlacingState(PlacingStateName.None);
    }

    public void SetPlacingObject(ShopObjectBase placingObjectModel)
    {
        PlacingShopObjectModel = placingObjectModel;
        SetPlacingState(PlacingStateName.PlacingShopObject);
    }

    public void SetViewingShopModel(ShopModel shopModel)
    {
        ViewingShopModel = shopModel;
        ViewingShopModelChanged(ViewingShopModel);
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
    PlacingShopObject,
    PlacingFloor,
    PlacingWall,
    PlacingWindow,
    PlacingDoor,
}
