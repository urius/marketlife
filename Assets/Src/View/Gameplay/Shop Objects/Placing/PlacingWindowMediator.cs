using UnityEngine;

public class PlacingWindowMediator : IMediator
{
    protected readonly Transform ParentTransform;

    private readonly GameStateModel _gameStateModel;
    private readonly Dispatcher _dispatcher;
    private readonly MouseCellCoordsProvider _mouseCellCoordsProvider;

    private (Transform transform, SpriteRenderer sprite) _currentPlacingObjectContext;
    private ShopModel _currentShopModel;

    public PlacingWindowMediator(Transform parentTransform)
    {
        ParentTransform = parentTransform;

        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
        _mouseCellCoordsProvider = MouseCellCoordsProvider.Instance;
    }

    public void Mediate()
    {
        _currentShopModel = GameStateModel.Instance.ViewingShopModel;

        _dispatcher.MouseCellCoordsUpdated += OnMouseCellCoordsUpdated;
        _currentShopModel.ShopDesign.WindowChanged += OnWindowChanged;

        _currentPlacingObjectContext = new ViewsFactory().CreateWindow(ParentTransform, _gameStateModel.PlacingDecorationNumericId);
        var sprite = _currentPlacingObjectContext.sprite;
        sprite.color = sprite.color.SetAlpha(0.7f);
        sprite.sortingLayerName = SortingLayers.Placing;
        WallsHelper.ToLeftState(_currentPlacingObjectContext.transform);

        UpdatePosition(_mouseCellCoordsProvider.MouseCellCoords);
        UpdateBuildAvailability(_mouseCellCoordsProvider.MouseCellCoords);
    }

    public void Unmediate()
    {
        _dispatcher.MouseCellCoordsUpdated -= OnMouseCellCoordsUpdated;
        _currentShopModel.ShopDesign.WindowChanged -= OnWindowChanged;

        GameObject.Destroy(_currentPlacingObjectContext.transform.gameObject);
        _currentPlacingObjectContext = default;
    }

    private void OnWindowChanged(Vector2Int cellCoords, int numericId)
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
        _currentPlacingObjectContext.sprite.color = _currentShopModel.CanPlaceWindow(mouseCellCoords, _gameStateModel.PlacingDecorationNumericId)
                    ? _currentPlacingObjectContext.sprite.color.SetRGB(0.5f, 1f, 0.5f)
                    : _currentPlacingObjectContext.sprite.color.SetRGB(1f, 0.5f, 0.5f);
    }

}
