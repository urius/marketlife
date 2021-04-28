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
        _gameStateModel.PlacingObjectStateChanged += OnPlacingObjectStateChanged;
    }

    private void OnPlacingObjectStateChanged(ShopObjectBase shopObjectModel)
    {
        switch (shopObjectModel.Type)
        {
            case ShopObjectType.Shelf:
                _currentPlacingShopObjectMediator = new PlacingShopObjectMediator(transform, shopObjectModel as ShelfModel);
                _currentPlacingShopObjectMediator.Mediate();
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
