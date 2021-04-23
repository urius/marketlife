using System.Collections.Generic;
using UnityEngine;

public class FloorMediator : MonoBehaviour
{
    private PlayerModel _playerModel;
    private GridCalculator _gridCalculator;
    private SpritesProvider _spritesProvider;

    private readonly Dictionary<Vector2Int, SpriteRenderer> _floorSprites = new Dictionary<Vector2Int, SpriteRenderer>();

    private void Awake()
    {
        _playerModel = PlayerModel.Instance;
        _gridCalculator = GridCalculator.Instance;
        _spritesProvider = SpritesProvider.Instance;
    }

    private void Start()
    {
        Activate();

        if (_playerModel.ViewingShopModel != null)
        {
            ShowFloors(_playerModel.ViewingShopModel.ShopDesign.Floors);
        }
    }

    private void Activate()
    {
        _playerModel.ViewingShopModelChanged += OnViewingShopModelChanged;
    }

    private void OnViewingShopModelChanged(ShopModel newShopModel)
    {
        ShowFloors(newShopModel.ShopDesign.Floors);
    }

    private void ShowFloors(Dictionary<Vector2Int, int> floorsDataNew)
    {
        foreach (var kvp in floorsDataNew)
        {
            if (!_floorSprites.ContainsKey(kvp.Key))
            {
                var floorGo = Instantiate(PrefabsHolder.Instance.FloorPrefab, transform);
                var spriteRenderer = floorGo.GetComponent<SpriteRenderer>();
                floorGo.transform.position = _gridCalculator.CellToWord(kvp.Key);

                _floorSprites[kvp.Key] = spriteRenderer;
            }

            _floorSprites[kvp.Key].sprite = _spritesProvider.GetFloorSprite(kvp.Value);
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
