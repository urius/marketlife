using System.Collections.Generic;
using UnityEngine;

public class PerimeterMediator : MonoBehaviour
{
    private GameStateModel _gameStateModel;
    private SpritesProvider _spritesProvider;
    private IMediator _currentPlacingWallMediator;
    private ShopModel _activeShopModel;
    private SpriteRenderer _highlightedSpriteRenderer;
    private readonly Dictionary<Vector2Int, SpriteRenderer> _wallViews = new Dictionary<Vector2Int, SpriteRenderer>();
    private readonly Dictionary<Vector2Int, (Transform transform, SpriteRenderer spriteRenderer)> _windowViews = new Dictionary<Vector2Int, (Transform transform, SpriteRenderer sprite)>();
    private readonly Dictionary<Vector2Int, DoorView> _doorViews = new Dictionary<Vector2Int, DoorView>();

    private void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
        _spritesProvider = SpritesProvider.Instance;
    }

    private void Start()
    {
        Activate();

        if (_gameStateModel.ViewingShopModel != null)
        {
            ActivateForShopModel(_gameStateModel.ViewingShopModel);
            ShowPerimeterDesign(_gameStateModel.ViewingShopModel.ShopDesign);
        }
    }

    private void Activate()
    {
        _gameStateModel.ViewingShopModelChanged += OnViewingShopModelChanged;
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
        _gameStateModel.HighlightStateChanged += OnHighlightStateChanged;
    }

    private void OnHighlightStateChanged()
    {
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        if (_gameStateModel.HighlightState.IsHighlightedDecoration)
        {
            HighlightDecorationOn(_gameStateModel.HighlightState.HighlightedDecorationCoords);
        }
        else
        {
            DisableCurrentHighlight();
        }
    }

    private void HighlightDecorationOn(Vector2Int coords)
    {
        DisableCurrentHighlight();

        if (_windowViews.TryGetValue(coords, out var windowView))
        {
            _highlightedSpriteRenderer = windowView.spriteRenderer;
        }
        else if (_doorViews.TryGetValue(coords, out var doorView))
        {
            _highlightedSpriteRenderer = doorView.SpriteRenderer;
        }

        _highlightedSpriteRenderer.color = Color.green;
    }

    private void DisableCurrentHighlight()
    {
        if (_highlightedSpriteRenderer != null)
        {
            _highlightedSpriteRenderer.color = Color.white;
            _highlightedSpriteRenderer = null;
        }
    }

    private void OnPlacingStateChanged(PlacingStateName previousState, PlacingStateName newState)
    {
        switch (newState)
        {
            case PlacingStateName.None:
                if (_currentPlacingWallMediator != null)
                {
                    _currentPlacingWallMediator.Unmediate();
                    _currentPlacingWallMediator = null;
                }
                UpdateHighlight();
                break;
            case PlacingStateName.PlacingNewWall:
                _currentPlacingWallMediator = new PlacingWallMediator(transform);
                _currentPlacingWallMediator.Mediate();
                break;
            case PlacingStateName.PlacingNewWindow:
            case PlacingStateName.MovingWindow:
                _currentPlacingWallMediator = new PlacingWindowMediator(transform);
                _currentPlacingWallMediator.Mediate();
                break;
            case PlacingStateName.PlacingNewDoor:
            case PlacingStateName.MovingDoor:
                _currentPlacingWallMediator = new PlacingDoorMediator(transform);
                _currentPlacingWallMediator.Mediate();
                break;
        }
    }

    private void OnViewingShopModelChanged(ShopModel newShopModel)
    {
        DeactivateCurrentShopModel();
        ActivateForShopModel(newShopModel);
        ShowPerimeterDesign(newShopModel.ShopDesign);
    }

    private void ActivateForShopModel(ShopModel shopModel)
    {
        _activeShopModel = shopModel;

        _activeShopModel.ShopDesign.WallChanged += OnWallChanged;
        _activeShopModel.ShopDesign.WindowChanged += OnWindowChanged;
        _activeShopModel.ShopDesign.DoorChanged += OnDoorChanged;
        _activeShopModel.ShopDesign.SizeXChanged += OnSizeChanged;
        _activeShopModel.ShopDesign.SizeYChanged += OnSizeChanged;
    }

    private void DeactivateCurrentShopModel()
    {
        if (_activeShopModel == null) return;
        _activeShopModel.ShopDesign.WallChanged -= OnWallChanged;
        _activeShopModel.ShopDesign.WindowChanged -= OnWindowChanged;
        _activeShopModel.ShopDesign.DoorChanged -= OnDoorChanged;
        _activeShopModel.ShopDesign.SizeXChanged -= OnSizeChanged;
        _activeShopModel.ShopDesign.SizeYChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(int previousSize, int currentSize)
    {
        ShowPerimeterDesign(_activeShopModel.ShopDesign);
    }

    private void OnWallChanged(Vector2Int cellCords, int numericId)
    {
        _wallViews[cellCords].sprite = _spritesProvider.GetWallSprite(numericId);
    }

    private void OnWindowChanged(Vector2Int cellCords, int numericId)
    {
        if (numericId > 0)
        {
            if (_windowViews.ContainsKey(cellCords))
            {
                _windowViews[cellCords].spriteRenderer.sprite = _spritesProvider.GetWindowSprite(numericId);
            }
            else
            {
                var windowView = new ViewsFactory().CreateWindow(transform, numericId);
                WallsHelper.PlaceAsWallLike(windowView.transform, cellCords);
                _windowViews[cellCords] = windowView;
            }
        }
        else if (_windowViews.TryGetValue(cellCords, out var windowView))
        {
            Destroy(windowView.transform.gameObject);
            _windowViews.Remove(cellCords);
        }
    }

    private void OnDoorChanged(Vector2Int cellCords, int numericId)
    {
        if (numericId > 0)
        {
            if (_doorViews.ContainsKey(cellCords))
            {
                _doorViews[cellCords].SetDoorId(numericId);
            }
            else
            {
                var doorView = new ViewsFactory().CreateDoor(transform, numericId);
                WallsHelper.PlaceAsWallLike(doorView.transform, cellCords);
                _doorViews[cellCords] = doorView;
            }
        }
        else if (_doorViews.TryGetValue(cellCords, out var doorView))
        {
            Destroy(doorView.transform.gameObject);
            _doorViews.Remove(cellCords);
            if (_wallViews.TryGetValue(cellCords, out var wallView))
            {
                wallView.gameObject.SetActive(true);
            }
        }
    }

    private void ShowPerimeterDesign(ShoDesignModel shopDesign)
    {
        ShowWalls(shopDesign.Walls);
        ShowWindows(shopDesign.Windows);
        ShowDoors(shopDesign.Doors);
    }

    private void ShowWalls(Dictionary<Vector2Int, int> wallsDataNew)
    {
        var viewsFactory = new ViewsFactory();
        foreach (var kvp in wallsDataNew)
        {
            SpriteRenderer wallSprite;
            if (!_wallViews.ContainsKey(kvp.Key))
            {
                wallSprite = viewsFactory.CreateWall(transform, kvp.Value);
                WallsHelper.PlaceAsWallLike(wallSprite.transform, kvp.Key);
                _wallViews[kvp.Key] = wallSprite;
            }
            else
            {
                wallSprite = _wallViews[kvp.Key];
                wallSprite.sprite = _spritesProvider.GetWallSprite(kvp.Value);
                wallSprite.gameObject.SetActive(true);
            }
        }

        var keysToRemove = new List<Vector2Int>();
        foreach (var kvp in _wallViews)
        {
            if (!wallsDataNew.ContainsKey(kvp.Key))
            {
                Destroy(_wallViews[kvp.Key].gameObject);
                keysToRemove.Add(kvp.Key);
            }
        }

        keysToRemove.ForEach(k => _wallViews.Remove(k));
    }

    private void ShowWindows(Dictionary<Vector2Int, int> windowsDataNew)
    {
        RemoveAllWindows();

        var viewsFactory = new ViewsFactory();
        foreach (var kvp in windowsDataNew)
        {
            var windowView = viewsFactory.CreateWindow(transform, kvp.Value);
            WallsHelper.PlaceAsWallLike(windowView.transform, kvp.Key);
            _windowViews[kvp.Key] = windowView;
        }
    }

    private void ShowDoors(Dictionary<Vector2Int, int> doorsDataNew)
    {
        RemoveAllDoors();

        var viewsFactory = new ViewsFactory();
        foreach (var kvp in doorsDataNew)
        {
            if (_wallViews.TryGetValue(kvp.Key, out var wallView))
            {
                wallView.gameObject.SetActive(false);
            }

            var doorlView = viewsFactory.CreateDoor(transform, kvp.Value);
            WallsHelper.PlaceAsWallLike(doorlView.transform, kvp.Key);
            _doorViews[kvp.Key] = doorlView;
        }
    }

    private void RemoveAllWindows()
    {
        foreach (var windowView in _windowViews.Values)
        {
            Destroy(windowView.transform.gameObject);
        }
        _windowViews.Clear();
    }

    private void RemoveAllDoors()
    {
        foreach (var doorView in _doorViews.Values)
        {
            Destroy(doorView.gameObject);
        }
        _doorViews.Clear();
    }
}
