using System.Collections.Generic;
using UnityEngine;

public class WallsMediator : MonoBehaviour
{
    private GameStateModel _gameStateModel;
    private GridCalculator _gridCalculator;
    private SpritesProvider _spritesProvider;
    private PlacingShopDecorationMediator _currentPlacingShopDecorationMediator;
    private ShopModel _activeShopModel;
    private readonly Dictionary<Vector2Int, SpriteRenderer> _wallViews = new Dictionary<Vector2Int, SpriteRenderer>();
    private readonly Dictionary<Vector2Int, DoorView> _doorViews = new Dictionary<Vector2Int, DoorView>();

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
            ShowPerimeterDesign(_gameStateModel.ViewingShopModel.ShopDesign);
        }
    }

    private void Activate()
    {
        _gameStateModel.ViewingShopModelChanged += OnViewingShopModelChanged;
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
    }

    private void OnPlacingStateChanged(PlacingStateName previousState, PlacingStateName newState)
    {
        switch (newState)
        {
            case PlacingStateName.None:
                if (_currentPlacingShopDecorationMediator != null)
                {
                    _currentPlacingShopDecorationMediator.Unmediate();
                    _currentPlacingShopDecorationMediator = null;
                }
                break;
            case PlacingStateName.PlacingWall:
                _currentPlacingShopDecorationMediator = new PlacingShopDecorationMediator(transform);
                _currentPlacingShopDecorationMediator.Mediate();
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
    }

    private void DeactivateCurrentShopModel()
    {
        if (_activeShopModel == null) return;
        _activeShopModel.ShopDesign.WallChanged -= OnWallChanged;
    }

    private void OnWallChanged(Vector2Int cellCords, int numericId)
    {
        _wallViews[cellCords].sprite = _spritesProvider.GetWallSprite(numericId);
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
                var irRightWall = kvp.Key.y == -1;
                wallSprite.transform.position = irRightWall ? _gridCalculator.GetCellLeftCorner(kvp.Key) : _gridCalculator.GetCellRightCorner(kvp.Key);
                if (irRightWall)
                {
                    WallsHelper.ToRightState(wallSprite.transform);
                }
                else
                {
                    WallsHelper.ToLeftState(wallSprite.transform);
                }

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
        /*foreach (var kvp in _wallViews)
        {
            var wallView = kvp.Value;
            if (windowsDataNew.TryGetValue(kvp.Key, out var windowId))
            {
                wallView.SetWindowId(windowId);
            }
            else
            {
                wallView.RemoveWindow();
            }
        }*/
        //TODO: windows
    }

    private void ShowDoors(Dictionary<Vector2Int, int> doorsDataNew)
    {
        RemoveAllDoors();

        foreach (var kvp in doorsDataNew)
        {
            if (_wallViews.TryGetValue(kvp.Key, out var wallView))
            {
                wallView.gameObject.SetActive(false);
            }

            var doorGo = Instantiate(PrefabsHolder.Instance.DoorPrefab, transform);
            var doorlView = doorGo.GetComponent<DoorView>();
            var irRightSide = kvp.Key.y == -1;
            doorGo.transform.position = irRightSide ? _gridCalculator.GetCellLeftCorner(kvp.Key) : _gridCalculator.GetCellRightCorner(kvp.Key);
            if (irRightSide)
            {
                doorlView.ToRightState();
            }
            else
            {
                doorlView.ToLeftState();
            }

            _doorViews[kvp.Key] = doorlView;
        }
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
