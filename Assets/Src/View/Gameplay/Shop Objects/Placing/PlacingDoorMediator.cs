using UnityEngine;

public class PlacingDoorMediator : IMediator
{
    protected readonly Transform ParentTransform;

    private readonly GameStateModel _gameStateModel;
    private readonly Dispatcher _dispatcher;
    private readonly MouseDataProvider _mouseCellCoordsProvider;

    private DoorView _currentPlacingDoor;
    private ShopModel _currentShopModel;

    public PlacingDoorMediator(Transform parentTransform)
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
        _currentShopModel.ShopDesign.DoorChanged += OnDoorChanged;

        _currentPlacingDoor = new ViewsFactory().CreateDoor(ParentTransform, _gameStateModel.PlacingDecorationNumericId);
        _currentPlacingDoor.Color = _currentPlacingDoor.Color.SetAlpha(0.7f);
        _currentPlacingDoor.SetSortingLayerName(SortingLayers.Placing);
        WallsHelper.ToLeftState(_currentPlacingDoor.transform);

        UpdatePosition(_mouseCellCoordsProvider.MouseCellCoords);
        UpdateBuildAvailability(_mouseCellCoordsProvider.MouseCellCoords);
    }

    public void Unmediate()
    {
        _dispatcher.MouseCellCoordsUpdated -= OnMouseCellCoordsUpdated;
        _currentShopModel.ShopDesign.DoorChanged -= OnDoorChanged;

        GameObject.Destroy(_currentPlacingDoor.transform.gameObject);
        _currentPlacingDoor = default;
    }

    private void OnDoorChanged(Vector2Int cellCoords, int numericId)
    {
        if (numericId > 0)
        {
            UpdateBuildAvailability(cellCoords);
        }
    }

    private void OnMouseCellCoordsUpdated(Vector2Int cellCoords)
    {
        UpdatePosition(cellCoords);
        UpdateBuildAvailability(cellCoords);
    }

    private void UpdatePosition(Vector2Int cellCoords)
    {
        WallsHelper.PlaceAsWallLike(_currentPlacingDoor.transform, cellCoords);
    }

    private void UpdateBuildAvailability(Vector2Int mouseCellCoords)
    {
        _currentPlacingDoor.Color = _currentShopModel.CanPlaceDoor(mouseCellCoords, _gameStateModel.PlacingDecorationNumericId)
                    ? _currentPlacingDoor.Color.SetRGB(0.5f, 1f, 0.5f)
                    : _currentPlacingDoor.Color.SetRGB(1f, 0.5f, 0.5f);
    }
}
