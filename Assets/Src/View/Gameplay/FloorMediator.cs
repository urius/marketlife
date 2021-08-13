using System.Collections.Generic;
using UnityEngine;

public class FloorMediator : MonoBehaviour
{
    private GameStateModel _gameStateModel;
    private GridCalculator _gridCalculator;
    private SpritesProvider _spritesProvider;
    private IMediator _currentPlacingFloorMediator;
    private ShopModel _activeShopModel;
    private SpriteRenderer _highlightedUnwashSpriteRenderer;

    private readonly Dictionary<Vector2Int, SpriteRenderer> _floorSprites = new Dictionary<Vector2Int, SpriteRenderer>();
    private readonly Dictionary<Vector2Int, SpriteRenderer> _unwashesSprites = new Dictionary<Vector2Int, SpriteRenderer>();

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
            ActivateAndDisplayShopModel(_gameStateModel.ViewingShopModel);
        }
    }

    private void ActivateAndDisplayShopModel(ShopModel shopModel)
    {
        DeactivateCurrentShopModel();
        ActivateForShopModel(shopModel);
        ShowFloors(shopModel.ShopDesign.Floors);
        ShowUnwashes(shopModel.Unwashes);
    }

    private void Activate()
    {
        _gameStateModel.ViewingUserModelChanged += OnViewingUserModelChanged;
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
        _gameStateModel.HighlightStateChanged += OnHighlightStateChanged;
    }

    private void ActivateForShopModel(ShopModel shopModel)
    {
        _activeShopModel = shopModel;

        _activeShopModel.UnwashAdded += OnUnwashAdded;
        _activeShopModel.UnwashRemoved += OnUnwashRemoved;
        _activeShopModel.ShopDesign.FloorChanged += OnFloorChanged;
        _activeShopModel.ShopDesign.SizeXChanged += OnSizeChanged;
        _activeShopModel.ShopDesign.SizeYChanged += OnSizeChanged;
    }

    private void DeactivateCurrentShopModel()
    {
        if (_activeShopModel == null) return;
        _activeShopModel.UnwashAdded -= OnUnwashAdded;
        _activeShopModel.UnwashRemoved -= OnUnwashRemoved;
        _activeShopModel.ShopDesign.FloorChanged -= OnFloorChanged;
        _activeShopModel.ShopDesign.SizeXChanged -= OnSizeChanged;
        _activeShopModel.ShopDesign.SizeYChanged -= OnSizeChanged;
    }

    private void OnHighlightStateChanged()
    {
        if (_highlightedUnwashSpriteRenderer != null)
        {
            _highlightedUnwashSpriteRenderer.color = Color.white;
            _highlightedUnwashSpriteRenderer = null;
        }

        if (_gameStateModel.HighlightState.IsHighlightedUnwash)
        {
            if (_unwashesSprites.TryGetValue(_gameStateModel.HighlightState.HighlightedCoords, out var spriteRenderer))
            {
                _highlightedUnwashSpriteRenderer = spriteRenderer;
                _highlightedUnwashSpriteRenderer.color = new Color(1, 0, 0, 0.5f);
            }
        }
    }

    private void OnUnwashRemoved(Vector2Int coords)
    {
        if (_unwashesSprites.ContainsKey(coords))
        {
            GameObject.Destroy(_unwashesSprites[coords].gameObject);
            _unwashesSprites.Remove(coords);
        }
    }

    private void OnUnwashAdded(Vector2Int coords)
    {
        if (_unwashesSprites.ContainsKey(coords))
        {
            _unwashesSprites[coords].sprite = _spritesProvider.GetUnwashSprite(_activeShopModel.Unwashes[coords]);
        }
        else
        {
            CreateUnwash(coords, _activeShopModel.Unwashes[coords]);
        }
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

    private void OnViewingUserModelChanged(UserModel userModel)
    {
        ActivateAndDisplayShopModel(userModel.ShopModel);
    }

    private void ShowFloors(Dictionary<Vector2Int, int> floorsDataNew)
    {
        var viewsFactory = new ViewsFactory();
        foreach (var kvp in floorsDataNew)
        {
            if (!_floorSprites.ContainsKey(kvp.Key))
            {
                var floorSprite = viewsFactory.CreateFloor(transform, kvp.Value);
                floorSprite.transform.position = _gridCalculator.CellToWorld(kvp.Key);
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

    private void ShowUnwashes(Dictionary<Vector2Int, int> unwashesData)
    {
        foreach (var unwashSprite in _unwashesSprites)
        {
            GameObject.Destroy(unwashSprite.Value.gameObject);
        }
        _unwashesSprites.Clear();

        foreach (var kvp in unwashesData)
        {
            CreateUnwash(kvp.Key, kvp.Value);
        }
    }

    private void CreateUnwash(Vector2Int coords, int numericId)
    {
        var go = GameObject.Instantiate(PrefabsHolder.Instance.OnFloorItemPrefab, transform);
        var spriteRenderer = go.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpritesProvider.Instance.GetUnwashSprite(numericId);
        spriteRenderer.transform.position = _gridCalculator.CellToWorld(coords);
        _unwashesSprites[coords] = spriteRenderer;
    }
}
