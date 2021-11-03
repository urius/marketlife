using UnityEngine;

public class PlacingWallMediator : IMediator
{
    protected readonly Transform ParentTransform;

    private readonly GameStateModel _gameStateModel;
    private readonly Dispatcher _dispatcher;
    private readonly MouseDataProvider _mouseCellCoordsProvider;

    private (SpriteRenderer sprite, Transform transform) _currentPlacingObjectContext;
    private ShopModel _currentShopModel;

    public PlacingWallMediator(Transform parentTransform)
    {
        ParentTransform = parentTransform;

        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
        _mouseCellCoordsProvider = MouseDataProvider.Instance;
    }

    public void Mediate()
    {
        _currentShopModel = GameStateModel.Instance.ViewingShopModel;

        _dispatcher.MouseCellCoordsUpdated += OnMouseCellCoordsUpdated;
        _currentShopModel.ShopDesign.WallChanged += OnWallChanged;

        var sprite = new ViewsFactory().CreateWall(ParentTransform, _gameStateModel.PlacingDecorationNumericId);
        sprite.color = sprite.color.SetAlpha(0.7f);
        sprite.sortingLayerName = SortingLayers.Placing;
        WallsHelper.ToLeftState(sprite.transform);
        _currentPlacingObjectContext = (sprite, sprite.transform);

        UpdatePosition(_mouseCellCoordsProvider.MouseCellCoords);
        UpdateBuildAvailability(_mouseCellCoordsProvider.MouseCellCoords);
    }

    public void Unmediate()
    {
        _dispatcher.MouseCellCoordsUpdated -= OnMouseCellCoordsUpdated;
        _currentShopModel.ShopDesign.WallChanged -= OnWallChanged;

        GameObject.Destroy(_currentPlacingObjectContext.transform.gameObject);
        _currentPlacingObjectContext = default;
    }

    private void OnWallChanged(Vector2Int cellCoords, int numericId)
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
        WallsHelper.PlaceAsWallLike(_currentPlacingObjectContext.transform, cellCoords);
    }

    private void UpdateBuildAvailability(Vector2Int mouseCellCoords)
    {
        _currentPlacingObjectContext.sprite.color = _currentShopModel.CanPlaceWall(mouseCellCoords, _gameStateModel.PlacingDecorationNumericId)
                    ? _currentPlacingObjectContext.sprite.color.SetRGB(0.5f, 1f, 0.5f)
                    : _currentPlacingObjectContext.sprite.color.SetRGB(1f, 0.5f, 0.5f);
    }
}
