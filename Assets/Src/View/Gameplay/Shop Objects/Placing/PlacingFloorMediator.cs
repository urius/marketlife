using UnityEngine;

public class PlacingFloorMediator : IMediator
{
    protected readonly Transform ParentTransform;

    private readonly GameStateModel _gameStateModel;
    private readonly Dispatcher _dispatcher;
    private readonly MouseDataProvider _mouseCellCoordsProvider;
    private readonly GridCalculator _gridCalculator;

    private SpriteRenderer _currentPlacingSprite;
    private ShopModel _currentShopModel;

    public PlacingFloorMediator(Transform parentTransform)
    {
        ParentTransform = parentTransform;

        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
        _mouseCellCoordsProvider = MouseDataProvider.Instance;
        _gridCalculator = GridCalculator.Instance;
    }

    public void Mediate()
    {
        _currentShopModel = GameStateModel.Instance.ViewingShopModel;

        _dispatcher.MouseCellCoordsUpdated += OnMouseCellCoordsUpdated;
        _currentShopModel.ShopDesign.FloorChanged += OnFloorChanged;

        var sprite = new ViewsFactory().CreateFloor(ParentTransform, _gameStateModel.PlacingDecorationNumericId);
        sprite.color = sprite.color.SetAlpha(0.7f);
        sprite.sortingLayerName = SortingLayers.Placing;
        _currentPlacingSprite = sprite;

        UpdatePosition(_mouseCellCoordsProvider.MouseCellCoords);
        UpdateBuildAvailability(_mouseCellCoordsProvider.MouseCellCoords);
    }

    public void Unmediate()
    {
        _dispatcher.MouseCellCoordsUpdated -= OnMouseCellCoordsUpdated;
        _currentShopModel.ShopDesign.FloorChanged -= OnFloorChanged;

        GameObject.Destroy(_currentPlacingSprite.gameObject);
        _currentPlacingSprite = default;
    }

    private void OnFloorChanged(Vector2Int cellCoords, int numericId)
    {
        UpdateBuildAvailability(cellCoords);
    }

    private void OnMouseCellCoordsUpdated(Vector2Int cellCoords)
    {
        UpdatePosition(cellCoords);
        UpdateBuildAvailability(cellCoords);
    }

    private void UpdatePosition(Vector2Int cellCoords)
    {
        _currentPlacingSprite.transform.position = _gridCalculator.CellToWorld(cellCoords); ;
    }

    private void UpdateBuildAvailability(Vector2Int mouseCellCoords)
    {
        _currentPlacingSprite.color = _currentShopModel.CanPlaceFloor(mouseCellCoords, _gameStateModel.PlacingDecorationNumericId)
                    ? _currentPlacingSprite.color.SetRGB(0.5f, 1f, 0.5f)
                    : _currentPlacingSprite.color.SetRGB(1f, 0.5f, 0.5f);
    }
}
