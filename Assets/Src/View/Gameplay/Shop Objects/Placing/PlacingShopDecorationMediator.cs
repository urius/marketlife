using System;
using UnityEngine;

public class PlacingShopDecorationMediator
{
    protected readonly Transform ParentTransform;

    private readonly GameStateModel _gameStateModel;
    private readonly Dispatcher _dispatcher;
    private readonly MouseCellCoordsProvider _mouseCellCoordsProvider;
    private readonly GridCalculator _gridCalculator;

    private (SpriteRenderer sprite, Transform transform) _currentPlacingObjectContext;
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
        _currentShopModel = GameStateModel.Instance.ViewingShopModel;

        _dispatcher.MouseCellCoordsUpdated += OnMouseCellCoordsUpdated;
        SpriteRenderer sprite = null;
        switch (_gameStateModel.PlacingState)
        {
            case PlacingStateName.PlacingFloor:
                sprite = new ViewsFactory().CreateFloor(ParentTransform, _gameStateModel.PlacingDecorationNumericId);
                sprite.color = sprite.color.SetAlpha(0.7f);
                sprite.sortingLayerName = SortingLayers.Placing;
                _currentPlacingObjectContext = (sprite, sprite.transform);

                _currentShopModel.ShopDesign.FloorChanged += OnFloorChanged;
                break;
            case PlacingStateName.PlacingWall:
                sprite = new ViewsFactory().CreateWall(ParentTransform, _gameStateModel.PlacingDecorationNumericId);
                sprite.color = sprite.color.SetAlpha(0.7f);
                sprite.sortingLayerName = SortingLayers.Placing;
                WallsHelper.ToLeftState(sprite.transform);
                _currentPlacingObjectContext = (sprite, sprite.transform);

                _currentShopModel.ShopDesign.WallChanged += OnWallChanged;
                break;
        }

        UpdatePosition(_mouseCellCoordsProvider.MouseCellCoords);
        UpdateOrientation(_mouseCellCoordsProvider.MouseCellCoords);
        UpdateBuildAvailability(_mouseCellCoordsProvider.MouseCellCoords);
    }

    public void Unmediate()
    {
        _dispatcher.MouseCellCoordsUpdated -= OnMouseCellCoordsUpdated;
        _currentShopModel.ShopDesign.FloorChanged -= OnFloorChanged;
        _currentShopModel.ShopDesign.WallChanged -= OnWallChanged;

        GameObject.Destroy(_currentPlacingObjectContext.transform.gameObject);
        _currentPlacingObjectContext = default;
    }

    private void OnFloorChanged(Vector2Int cellCoords, int numericId)
    {
        UpdateBuildAvailability(cellCoords);
    }

    private void OnWallChanged(Vector2Int cellCoords, int numericId)
    {
        UpdateBuildAvailability(cellCoords);
    }

    private void OnMouseCellCoordsUpdated(Vector2Int cellCoords)
    {
        UpdatePosition(cellCoords);
        UpdateOrientation(cellCoords);
        UpdateBuildAvailability(cellCoords);
    }

    private void UpdateOrientation(Vector2Int cellCoords)
    {
        switch (_gameStateModel.PlacingState)
        {
            case PlacingStateName.PlacingWall:
            case PlacingStateName.PlacingWindow:
                var isRight = cellCoords.y < cellCoords.x;
                if (isRight)
                {
                    WallsHelper.ToRightState(_currentPlacingObjectContext.transform);
                }
                else
                {
                    WallsHelper.ToLeftState(_currentPlacingObjectContext.transform);
                }
                break;
        };
    }

    private void UpdatePosition(Vector2Int cellCoords)
    {
        _currentPlacingObjectContext.transform.position = GetPosition(cellCoords);
    }

    private void UpdateBuildAvailability(Vector2Int mouseCellCoords)
    {
        _currentPlacingObjectContext.sprite.color = CanBePlaced(mouseCellCoords)
                    ? _currentPlacingObjectContext.sprite.color.SetRGB(0.5f, 1f, 0.5f)
                    : _currentPlacingObjectContext.sprite.color.SetRGB(1f, 0.5f, 0.5f);
    }

    private Vector3 GetPosition(Vector2Int cellCoords)
    {
        switch (_gameStateModel.PlacingState)
        {

            case PlacingStateName.PlacingFloor:
                return _gridCalculator.CellToWord(cellCoords);
            case PlacingStateName.PlacingWall:
            case PlacingStateName.PlacingWindow:
                var isRightWall = cellCoords.y < cellCoords.x;
                return isRightWall ? _gridCalculator.GetCellLeftCorner(cellCoords) : _gridCalculator.GetCellRightCorner(cellCoords);
            default:
                throw new ArgumentOutOfRangeException($"PlacingShopDecorationMediator: PlacingState {_gameStateModel.PlacingState} is not supported");
        };
    }

    private bool CanBePlaced(Vector2Int mouseCellCoords)
    {
        return _gameStateModel.PlacingState switch
        {
            PlacingStateName.PlacingFloor => _currentShopModel.CanPlaceFloor(mouseCellCoords, _gameStateModel.PlacingDecorationNumericId),
            PlacingStateName.PlacingWall => _currentShopModel.CanPlaceWall(mouseCellCoords, _gameStateModel.PlacingDecorationNumericId),
            _ => throw new ArgumentOutOfRangeException($"PlacingShopDecorationMediator: PlacingState {_gameStateModel.PlacingState} is not supported"),
        };
    }
}
