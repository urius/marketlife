using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopObjectsMediator : MonoBehaviour
{
    private PlayerModel _playerModel;

    private readonly Dictionary<Vector2Int, ShopObjectMediatorBase> _shopObjectMediators = new Dictionary<Vector2Int, ShopObjectMediatorBase>();

    private void Awake()
    {
        _playerModel = PlayerModel.Instance;
    }

    private void Start()
    {
        Activate();

        if (_playerModel.ViewingShopModel != null)
        {
            DisplayShopObjects(_playerModel.ViewingShopModel.ShopObjects);
        }
    }

    private void Activate()
    {
        _playerModel.ViewingShopModelChanged += OnViewingShopModelChanged;
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
            _shopObjectMediators[kvp.Key] = CreateMediator(kvp.Value);
            _shopObjectMediators[kvp.Key].Mediate();
        }
    }

    private ShopObjectMediatorBase CreateMediator(ShopObjectBase shopObjectModel)
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
            kvp.Value.Destroy();
        }
        _shopObjectMediators.Clear();
    }
}
