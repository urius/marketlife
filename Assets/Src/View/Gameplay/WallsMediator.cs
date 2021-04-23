using System.Collections.Generic;
using UnityEngine;

public class WallsMediator : MonoBehaviour
{
    private PlayerModel _playerModel;
    private GridCalculator _gridCalculator;

    private readonly Dictionary<Vector2Int, WallView> _wallViews = new Dictionary<Vector2Int, WallView>();
    private readonly Dictionary<Vector2Int, DoorView> _doorViews = new Dictionary<Vector2Int, DoorView>();

    private void Awake()
    {
        _playerModel = PlayerModel.Instance;
        _gridCalculator = GridCalculator.Instance;
    }

    private void Start()
    {
        Activate();

        if (_playerModel.ViewingShopModel != null)
        {
            ShowPerimeterDesign(_playerModel.ViewingShopModel.ShopDesign);
        }
    }

    private void Activate()
    {
        _playerModel.ViewingShopModelChanged += OnViewingShopModelChanged;
    }

    private void OnViewingShopModelChanged(ShopModel newShopModel)
    {
        ShowPerimeterDesign(newShopModel.ShopDesign);
    }

    private void ShowPerimeterDesign(ShoDesignModel shopDesign)
    {
        ShowWalls(shopDesign.Walls);
        ShowWindows(shopDesign.Windows);
        ShowDoors(shopDesign.Doors);
    }

    private void ShowWalls(Dictionary<Vector2Int, int> wallsDataNew)
    {
        foreach (var kvp in wallsDataNew)
        {
            WallView wallView;
            if (!_wallViews.ContainsKey(kvp.Key))
            {
                var wallGo = Instantiate(PrefabsHolder.Instance.WallPrefab, transform);
                wallView = wallGo.GetComponent<WallView>();
                var irRightWall = kvp.Key.y == -1;
                wallGo.transform.position = irRightWall ? _gridCalculator.GetCellLeftCorner(kvp.Key) : _gridCalculator.GetCellRightCorner(kvp.Key);
                if (irRightWall)
                {
                    wallView.ToRightState();
                }
                else
                {
                    wallView.ToLeftState();
                }

                _wallViews[kvp.Key] = wallView;
            }
            else
            {
                wallView = _wallViews[kvp.Key];
                wallView.gameObject.SetActive(true);
            }

            wallView.SetWallId(kvp.Value);
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
        foreach (var kvp in _wallViews)
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
        }
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
