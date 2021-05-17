using UnityEngine;

public class PlacingShopDecorationMediator
{
    protected readonly Transform ParentTransform;

    private readonly GameStateModel _gameStateModel;
    private readonly Dispatcher _dispatcher;
    private readonly MouseCellCoordsProvider _mouseCellCoordsProvider;
    private readonly GridCalculator _gridCalculator;

    private SpriteRenderer _currentPlacingSprite;
    private ShopModel _currentShopModel;

    public PlacingShopDecorationMediator(Transform parentTransform)
    {
        ParentTransform = parentTransform;

        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
        _mouseCellCoordsProvider = MouseCellCoordsProvider.Instance;
        _gridCalculator = GridCalculator.Instance;
    }

    public void Mediate()
    {
        _dispatcher.MouseCellCoordsUpdated += OnMouseCellCoordsUpdated;
        _currentShopModel = GameStateModel.Instance.ViewingShopModel;

        switch (_gameStateModel.PlacingState)
        {
            case PlacingStateName.PlacingFloor:
                _currentPlacingSprite = new ViewsFactory().CreateFloor(ParentTransform, _gameStateModel.PlacingDecorationNumericId);
                _currentPlacingSprite.color = _currentPlacingSprite.color.SetAlpha(0.7f);
                _currentPlacingSprite.sortingLayerName = SortingLayers.Placing;
                break;
        }

        UpdatePosition(_mouseCellCoordsProvider.MouseCellCoords);
        UpdateBuildAvailability(_mouseCellCoordsProvider.MouseCellCoords);
    }

    public void Unmediate()
    {
        _dispatcher.MouseCellCoordsUpdated -= OnMouseCellCoordsUpdated;

        GameObject.Destroy(_currentPlacingSprite.gameObject);
        _currentPlacingSprite = null;
    }

    private void OnMouseCellCoordsUpdated(Vector2Int cellCoords)
    {
        UpdatePosition(cellCoords);
        UpdateBuildAvailability(cellCoords);
    }

    private void UpdatePosition(Vector2Int cellCoords)
    {
        _currentPlacingSprite.transform.position = _gridCalculator.CellToWord(cellCoords);
    }

    private void UpdateBuildAvailability(Vector2Int mouseCellCoords)
    {
        if (_currentShopModel.CanPlaceFloor(mouseCellCoords, _gameStateModel.PlacingDecorationNumericId))
        {
            _currentPlacingSprite.color = _currentPlacingSprite.color.SetRGB(0.5f, 1f, 0.5f);
        }
        else
        {
            _currentPlacingSprite.color = _currentPlacingSprite.color.SetRGB(1f, 0.5f, 0.5f);
        }
    }
}
