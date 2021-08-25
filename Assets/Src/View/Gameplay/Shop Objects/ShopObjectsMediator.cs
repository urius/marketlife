using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopObjectsMediator : MonoBehaviour
{
    private GameStateModel _gameStateModel;

    private readonly Dictionary<Vector2Int, ShopObjectMediatorBase> _shopObjectMediators = new Dictionary<Vector2Int, ShopObjectMediatorBase>();

    private PlacingShopObjectMediator _currentPlacingShopObjectMediator;
    private ShopModel _currentShopModel;

    private readonly List<GameObject> _debugSquaresList = new List<GameObject>();

    private void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
    }

    private void Start()
    {
        Activate();

        if (_gameStateModel.ViewingShopModel != null)
        {
            HandleNewShopModel(_gameStateModel.ViewingShopModel);
        }
    }

    private void HandleNewShopModel(ShopModel viewingShopModel)
    {
        ForgetCurrentShopModel();

        _currentShopModel = viewingShopModel;
        _currentShopModel.ShopObjectPlaced += OnShopObjectPlaced;
        _currentShopModel.ShopObjectRemoved += OnShopObjectRemoved;
        _currentShopModel.ShopObjectsChanged += OnShopObjectsChanged;

        DisplayShopObjects(viewingShopModel.ShopObjects);
    }

    private void ForgetCurrentShopModel()
    {
        if (_currentShopModel != null)
        {
            _currentShopModel.ShopObjectPlaced -= OnShopObjectPlaced;
            _currentShopModel.ShopObjectRemoved -= OnShopObjectRemoved;
            _currentShopModel.ShopObjectsChanged -= OnShopObjectsChanged;
        }
    }

    private void OnShopObjectRemoved(ShopObjectModelBase shopObjectModel)
    {
        _shopObjectMediators[shopObjectModel.Coords].Unmediate();
        _shopObjectMediators.Remove(shopObjectModel.Coords);
    }

    private void OnShopObjectPlaced(ShopObjectModelBase shopObject)
    {
        MediateNewShopObject(shopObject);
    }

    private void Activate()
    {
        _gameStateModel.ViewingUserModelChanged += OnViewingUserModelChanged;
        _gameStateModel.ActionStateChanged += OnActionStateChanged;
    }

    private void OnActionStateChanged(ActionStateName previousState, ActionStateName newState)
    {
        switch (newState)
        {
            case ActionStateName.None:
                if (_currentPlacingShopObjectMediator != null)
                {
                    _currentPlacingShopObjectMediator.Unmediate();
                    _currentPlacingShopObjectMediator = null;
                }
                break;
            case ActionStateName.PlacingNewShopObject:
            case ActionStateName.MovingShopObject:
                _currentPlacingShopObjectMediator = new PlacingShopObjectMediator(transform, _gameStateModel.PlacingShopObjectModel);
                _currentPlacingShopObjectMediator.Mediate();
                break;
        }
    }

    private void OnViewingUserModelChanged(UserModel userModel)
    {
        HandleNewShopModel(userModel.ShopModel);
    }

    private void DisplayShopObjects(Dictionary<Vector2Int, ShopObjectModelBase> newShopObjectsData)
    {
        ClearCurrentObjects();

        foreach (var kvp in newShopObjectsData)
        {
            MediateNewShopObject(kvp.Value);
        }

        DebugDisplayBuildSquares();
    }

    private void MediateNewShopObject(ShopObjectModelBase shopObjectModel)
    {
        var coords = shopObjectModel.Coords;
        _shopObjectMediators[coords] = CreateShopObjectMediator(shopObjectModel);
        _shopObjectMediators[coords].Mediate();
    }

    private void OnShopObjectsChanged()
    {

#if UNITY_EDITOR
        DebugDisplayBuildSquares();
#endif
    }

    private void DebugDisplayBuildSquares()
    {
#if UNITY_EDITOR
        return;

        foreach (var squareGo in _debugSquaresList)
        {
            GameObject.Destroy(squareGo);
        }
        _debugSquaresList.Clear();

        var viewingShopModel = GameStateModel.Instance.ViewingShopModel;
        foreach (var kvp in viewingShopModel.Grid)
        {
            var squareGo = GameObject.Instantiate(PrefabsHolder.Instance.WhiteSquarePrefab, transform);
            _debugSquaresList.Add(squareGo);
            squareGo.transform.position = GridCalculator.Instance.CellToWorld(kvp.Key);
            var sr = squareGo.GetComponent<SpriteRenderer>();
            var buildState = kvp.Value.buildState;
            sr.color = sr.color.SetRGBA((buildState == 1) ? 1 : 0, (buildState == 0) ? 1 : 0, (buildState == -1) ? 1 : 0, 0.5f);
        }
#endif
    }

    private ShopObjectMediatorBase CreateShopObjectMediator(ShopObjectModelBase shopObjectModel)
    {
        ShopObjectMediatorBase result;
        switch (shopObjectModel.Type)
        {
            case ShopObjectType.Shelf:
                result = new ShelfViewMediator(transform, (ShelfModel)shopObjectModel);
                break;
            case ShopObjectType.CashDesk:
                result = new CashDeskViewMediator(transform, (CashDeskModel)shopObjectModel);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(shopObjectModel.Type), $"CreateMediator: Unsupported ShopObjectType: {shopObjectModel.Type}");
        }

        return result;
    }

    private void ClearCurrentObjects()
    {
        foreach (var kvp in _shopObjectMediators)
        {
            kvp.Value.Unmediate();
        }
        _shopObjectMediators.Clear();
    }
}
