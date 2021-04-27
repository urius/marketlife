using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameStateModel : MonoBehaviour
{
    private static Lazy<GameStateModel> _instance = new Lazy<GameStateModel>();
    public static GameStateModel Instance => _instance.Value;

    public event Action GameStateChanged = delegate { };
    public event Action<ShopObjectBase> PlacingObjectStateChanged = delegate { };
    public event Action<ShopModel> ViewingShopModelChanged = delegate { };

    private TaskCompletionSource<bool> _dataLoadedTcs = new TaskCompletionSource<bool>();
    public Task GameDataLoadedTask => _dataLoadedTcs.Task;

    public GameStateName GameState { get; private set; }
    public ShopObjectBase PlacingShopObjectModel { get; private set; }
    public ShopModel ViewingShopModel { get; private set; }

    public void SetGameState(GameStateName newState)
    {
        if (newState == GameStateName.Loaded)
        {
            _dataLoadedTcs.TrySetResult(true);
        }

        GameState = newState;
        GameStateChanged();
    }

    public void SetPlacingObject(ShopObjectBase placingObjectModel)
    {
        PlacingShopObjectModel = placingObjectModel;

        PlacingObjectStateChanged(PlacingShopObjectModel);
    }

    public void SetViewingShopModel(ShopModel shopModel)
    {
        ViewingShopModel = shopModel;

        ViewingShopModelChanged(ViewingShopModel);
    }
}

public enum GameStateName
{
    Initializing,
    Loading,
    Loaded,
    ShopSimulation,
    ShopBuilding,
}
