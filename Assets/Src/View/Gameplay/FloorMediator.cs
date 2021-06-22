using System.Collections.Generic;
using UnityEngine;

public class FloorMediator : MonoBehaviour
{
    private GameStateModel _gameStateModel;
    private GridCalculator _gridCalculator;
    private SpritesProvider _spritesProvider;
    private IMediator _currentPlacingFloorMediator;
    private ShopModel _activeShopModel;

    private readonly Dictionary<Vector2Int, SpriteRenderer> _floorSprites = new Dictionary<Vector2Int, SpriteRenderer>();

    private void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
        _gridCalculator = GridCalculator.Instance;
        _spritesProvider = SpritesProvider.Instance;
    }

    private void Start()
    {
        Activate();

        if (_gameStateModel.ViewingShopModel != null)
        {
            ActivateForShopModel(_gameStateModel.ViewingShopModel);
            ShowFloors(_gameStateModel.ViewingShopModel.ShopDesign.Floors);
        }
    }

    private void Activate()
    {
        _gameStateModel.ViewingShopModelChanged += OnViewingShopModelChanged;
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
    }

    private void ActivateForShopModel(ShopModel shopModel)
    {
        _activeShopModel = shopModel;

        _activeShopModel.ShopDesign.FloorChanged += OnFloorChanged;
        _activeShopModel.ShopDesign.SizeXChanged += OnSizeChanged;
        _activeShopModel.ShopDesign.SizeYChanged += OnSizeChanged;
    }

    private void DeactivateCurrentShopModel()
    {
        if (_activeShopModel == null) return;
        _activeShopModel.ShopDesign.FloorChanged -= OnFloorChanged;
        _activeShopModel.ShopDesign.SizeXChanged -= OnSizeChanged;
        _activeShopModel.ShopDesign.SizeYChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(int previousSize, int currentSize)
    {
        ShowFloors(_gameStateModel.ViewingShopModel.ShopDesign.Floors);
    }

    private void OnFloorChanged(Vector2Int cellCoords, int numericId)
    {
        _floorSprites[cellCoords].sprite = _spritesProvider.GetFloorSprite(numericId);
    }

    private void OnPlacingStateChanged(PlacingStateName previousState, PlacingStateName newState)
    {
        switch (newState)
        {
            case PlacingStateName.None:
                if (_currentPlacingFloorMediator != null)
                {
                    _currentPlacingFloorMediator.Unmediate();
                    _currentPlacingFloorMediator = null;
                }
                break;
            case PlacingStateName.PlacingNewFloor:
                _currentPlacingFloorMediator = new PlacingFloorMediator(transform);
                _currentPlacingFloorMediator.Mediate();
                break;
        }
    }

    private void OnViewingShopModelChanged(ShopModel newShopModel)
    {
        DeactivateCurrentShopModel();
        ActivateForShopModel(newShopModel);
        ShowFloors(newShopModel.ShopDesign.Floors);
    }

    private void ShowFloors(Dictionary<Vector2Int, int> floorsDataNew)
    {
        var viewsFactory = new ViewsFactory();
        foreach (var kvp in floorsDataNew)
        {
            if (!_floorSprites.ContainsKey(kvp.Key))
            {
                var floorSprite = viewsFactory.CreateFloor(transform, kvp.Value);
                floorSprite.transform.position = _gridCalculator.CellToWord(kvp.Key);
                _floorSprites[kvp.Key] = floorSprite;
            }
            else
            {
                _floorSprites[kvp.Key].sprite = _spritesProvider.GetFloorSprite(kvp.Value);
            }
        }

        var keysToRemove = new List<Vector2Int>();
        foreach (var kvp in _floorSprites)
        {
            if (!floorsDataNew.ContainsKey(kvp.Key))
            {
                Destroy(_floorSprites[kvp.Key].gameObject);
                keysToRemove.Add(kvp.Key);
            }
        }

        keysToRemove.ForEach(k => _floorSprites.Remove(k));
    }
}
