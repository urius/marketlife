using System.Collections.Generic;
using UnityEngine;

public class WallsMediator : MonoBehaviour
{
    private PlayerModel _playerModel;
    private GridCalculator _gridCalculator;

    private readonly Dictionary<Vector2Int, WallView> _wallViews = new Dictionary<Vector2Int, WallView>();

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
            ShowWalls(_playerModel.ViewingShopModel.ShopDesign.Walls);
        }
    }

    private void Activate()
    {
        _playerModel.ViewingShopModelChanged += OnViewingShopModelChanged;
    }

    private void OnViewingShopModelChanged(ShopModel newShopModel)
    {
        ShowWalls(newShopModel.ShopDesign.Walls);
    }

    private void ShowWalls(Dictionary<Vector2Int, int> wallsDataNew)
    {
        foreach (var kvp in wallsDataNew)
        {
            if (!_wallViews.ContainsKey(kvp.Key))
            {
                var wallGo = Instantiate(PrefabsHolder.Instance.WallPrefab, transform);
                var wallView = wallGo.GetComponent<WallView>();
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

            _wallViews[kvp.Key].SetWallId(kvp.Value);
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
}