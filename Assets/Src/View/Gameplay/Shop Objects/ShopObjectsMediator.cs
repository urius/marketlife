using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopObjectsMediator : MonoBehaviour
{
    private GameStateModel _gameStateModel;

    private readonly Dictionary<Vector2Int, ShopObjectMediatorBase> _shopObjectMediators = new Dictionary<Vector2Int, ShopObjectMediatorBase>();

    private PlacingShopObjectMediator _currentPlacingShopObjectMediator;

    private void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
    }

    private void Start()
    {
        Activate();

        if (_gameStateModel.ViewingShopModel != null)
        {
            DisplayShopObjects(_gameStateModel.ViewingShopModel.ShopObjects);
        }
    }

    private void Activate()
    {
        _gameStateModel.ViewingShopModelChanged += OnViewingShopModelChanged;
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
    }

    private void OnPlacingStateChanged(PlacingStateName previouseState, PlacingStateName newState)
    {
        switch (newState)
        {
            case PlacingStateName.None:
                if (_currentPlacingShopObjectMediator != null)
                {
                    _currentPlacingShopObjectMediator.Unmediate();
                    _currentPlacingShopObjectMediator = null;
                }
                break;
            case PlacingStateName.PlacingShopObject:
                var shopObjectModel = _gameStateModel.PlacingShopObjectModel;
                switch (shopObjectModel.Type)
                {
                    case ShopObjectType.Shelf:
                        _currentPlacingShopObjectMediator = new PlacingShopObjectMediator(transform, shopObjectModel as ShelfModel);
                        _currentPlacingShopObjectMediator.Mediate();
                        break;
                }
                break;
        }
    }

    private void OnViewingShopModelChanged(ShopModel newShopModel)
    {
        DisplayShopObjects(newShopModel.ShopObjects);
    }

    private void DisplayShopObjects(Dictionary<Vector2Int, ShopObjectBase> newShopObjectsData)
    {
        ClearCurrentObjects();

        foreach (var kvp in newShopObjectsData)
        {
            _shopObjectMediators[kvp.Key] = CreateShopObjectMediator(kvp.Value);
            _shopObjectMediators[kvp.Key].Mediate();
        }

        DebugDisplayBuildSquares();
    }

    private void DebugDisplayBuildSquares()
    {
#if UNITY_EDITOR
        var viewingShopModel = GameStateModel.Instance.ViewingShopModel;
        foreach (var kvp in viewingShopModel.Grid)
        {
            var squareGo = GameObject.Instantiate(PrefabsHolder.Instance.WhiteSquarePrefab, transform);
            squareGo.transform.position = GridCalculator.Instance.CellToWord(kvp.Key);
            var sr = squareGo.GetComponent<SpriteRenderer>();
            var buildState = kvp.Value.buildState;
            sr.color = sr.color.SetRGBA((buildState == 1) ? 1 : 0, (buildState == 0) ? 1 : 0, (buildState == -1) ? 1 : 0, 0.5f);
        }
#endif
    }

    private ShopObjectMediatorBase CreateShopObjectMediator(ShopObjectBase shopObjectModel)
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
