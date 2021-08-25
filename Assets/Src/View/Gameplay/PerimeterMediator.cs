using System.Collections.Generic;
using UnityEngine;

public class PerimeterMediator : MonoBehaviour
{
    private GameStateModel _gameStateModel;
    private SpritesProvider _spritesProvider;
    private UpdatesProvider _updatesProvider;
    private IMediator _currentPlacingWallMediator;
    private UserModel _activeUserModel;
    private SpriteRenderer _highlightedSpriteRenderer;

    private readonly Dictionary<Vector2Int, SpriteRenderer> _wallViews = new Dictionary<Vector2Int, SpriteRenderer>();
    private readonly Dictionary<Vector2Int, (Transform transform, SpriteRenderer spriteRenderer)> _windowViews = new Dictionary<Vector2Int, (Transform transform, SpriteRenderer sprite)>();
    private readonly Dictionary<Vector2Int, DoorView> _doorViews = new Dictionary<Vector2Int, DoorView>();

    private void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _updatesProvider = UpdatesProvider.Instance;
    }

    private void Start()
    {
        Activate();

        if (_gameStateModel.ViewingUserModel != null)
        {
            ActivateForUserModel(_gameStateModel.ViewingUserModel);
            ShowPerimeterDesign(_gameStateModel.ViewingShopModel.ShopDesign);
        }
    }

    private void Activate()
    {
        _gameStateModel.ViewingUserModelChanged += OnViewingUserModelChanged;
        _gameStateModel.ActionStateChanged += OnActionStateChanged;
        _gameStateModel.HighlightStateChanged += OnHighlightStateChanged;
        _gameStateModel.GameStateChanged += OnGameStateChanged;
        _updatesProvider.GameplaySecondUpdate += OnGameplaySecondUpdate;
    }

    private void OnGameplaySecondUpdate()
    {
        UpdateDoorsState();
    }

    private void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        if (_gameStateModel.IsSimulationState == false)
        {
            foreach (var kvp in _doorViews)
            {
                SetDoorOpenState(kvp.Key, isOpenState: false);
            }
        } else
        {
            UpdateDoorsState();
        }
    }

    private void UpdateDoorsState()
    {
        if (_gameStateModel.IsSimulationState && _activeUserModel != null)
        {
            foreach (var kvp in _doorViews)
            {
                var doorOusidePoint = kvp.Key;
                if (_activeUserModel.SessionDataModel.HaveCustomerAt(doorOusidePoint)
                    || _activeUserModel.SessionDataModel.HaveCustomerAt(GetDoorInsidePoint(doorOusidePoint)))
                {
                    SetDoorOpenState(kvp.Key, isOpenState: true);
                }
                else
                {
                    SetDoorOpenState(kvp.Key, isOpenState: false);
                }
            }
        }
    }

    private Vector2Int GetDoorInsidePoint(Vector2Int doorOusidePoint)
    {
        var result = doorOusidePoint;
        if (result.x == -1) result.x = 0;
        if (result.y == -1) result.y = 0;
        return result;
    }

    private void SetDoorOpenState(Vector2Int doorCoords, bool isOpenState)
    {
        if (_doorViews.ContainsKey(doorCoords))
        {
            var view = _doorViews[doorCoords];
            var isRight = doorCoords.y < doorCoords.x;
            if (isRight)
            {
                if (isOpenState)
                {
                    WallsHelper.ToLeftState(view.transform);
                }
                else
                {
                    WallsHelper.ToRightState(view.transform);
                }
            }
            else
            {
                if (isOpenState)
                {
                    WallsHelper.ToRightState(view.transform);
                }
                else
                {
                    WallsHelper.ToLeftState(view.transform);
                }
            }
        }
    }

    private void OnHighlightStateChanged()
    {
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        if (_gameStateModel.HighlightState.IsHighlightedDecoration)
        {
            HighlightDecorationOn(_gameStateModel.HighlightState.HighlightedCoords);
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

    private void OnActionStateChanged(ActionStateName previousState, ActionStateName newState)
    {
        switch (newState)
        {
            case ActionStateName.None:
                if (_currentPlacingWallMediator != null)
                {
                    _currentPlacingWallMediator.Unmediate();
                    _currentPlacingWallMediator = null;
                }
                UpdateHighlight();
                break;
            case ActionStateName.PlacingNewWall:
                _currentPlacingWallMediator = new PlacingWallMediator(transform);
                _currentPlacingWallMediator.Mediate();
                break;
            case ActionStateName.PlacingNewWindow:
            case ActionStateName.MovingWindow:
                _currentPlacingWallMediator = new PlacingWindowMediator(transform);
                _currentPlacingWallMediator.Mediate();
                break;
            case ActionStateName.PlacingNewDoor:
            case ActionStateName.MovingDoor:
                _currentPlacingWallMediator = new PlacingDoorMediator(transform);
                _currentPlacingWallMediator.Mediate();
                break;
        }
    }

    private void OnViewingUserModelChanged(UserModel userModel)
    {
        DeactivateCurrentUserModel();
        ActivateForUserModel(userModel);
        ShowPerimeterDesign(userModel.ShopModel.ShopDesign);
    }

    private void ActivateForUserModel(UserModel userModel)
    {
        _activeUserModel = userModel;
        var shopModel = userModel.ShopModel;

        shopModel.ShopDesign.WallChanged += OnWallChanged;
        shopModel.ShopDesign.WindowChanged += OnWindowChanged;
        shopModel.ShopDesign.DoorChanged += OnDoorChanged;
        shopModel.ShopDesign.SizeXChanged += OnSizeChanged;
        shopModel.ShopDesign.SizeYChanged += OnSizeChanged;
    }

    private void DeactivateCurrentUserModel()
    {
        if (_activeUserModel == null) return;
        var shopModel = _activeUserModel.ShopModel;

        shopModel.ShopDesign.WallChanged -= OnWallChanged;
        shopModel.ShopDesign.WindowChanged -= OnWindowChanged;
        shopModel.ShopDesign.DoorChanged -= OnDoorChanged;
        shopModel.ShopDesign.SizeXChanged -= OnSizeChanged;
        shopModel.ShopDesign.SizeYChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(int previousSize, int currentSize)
    {
        ShowPerimeterDesign(_activeUserModel.ShopModel.ShopDesign);
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
            if (_wallViews.TryGetValue(cellCords, out var wallView))
            {
                wallView.gameObject.SetActive(false);
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
