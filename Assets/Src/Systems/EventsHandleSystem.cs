using UnityEngine;

public class EventsHandleSystem : MonoBehaviour
{
    private Dispatcher _dispatcher;
    private GameStateModel _gameStateModel;

    public void Initialize()
    {
        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;

        Activate();
    }

    private void Activate()
    {
        _dispatcher.UIGameViewMouseClick += OnUIGameViewMouseClicked;
        _dispatcher.UIBottomPanelPlaceShelfClicked += OnUIBottomPanelPlaceShelfClicked;
        _dispatcher.UIBottomPanelPlaceFloorClicked += OnUIBottomPanelPlaceFloorClicked;
        _dispatcher.UIBottomPanelPlaceWallClicked += OnUIBottomPanelPlaceWallClicked;
        _dispatcher.UIBottomPanelPlaceWindowClicked += OnUIBottomPanelPlaceWindowClicked;
        _dispatcher.UIBottomPanelPlaceDoorClicked += OnUIBottomPanelPlaceDoorClicked;
        _dispatcher.MouseCellCoordsUpdated += OnMouseCellCoordsUpdated;
        _dispatcher.BottomPanelInteriorClicked += BottomPanelInteriorClicked;
        _dispatcher.BottomPanelInteriorCloseClicked += BottomPanelInteriorCloseClicked;
        _dispatcher.BottomPanlelFinishPlacingClicked += BottomPanelFinishPlacingClicked;
        _dispatcher.BottomPanelRotateRightClicked += BottomPanelRotateRightClicked;
        _dispatcher.BottomPanelRotateLeftClicked += BottomPanelRotateLeftClicked;
    }

    private void OnUIGameViewMouseClicked()
    {
        if (_gameStateModel.PlacingState != PlacingStateName.None)
        {
            new PlaceObjectCommand().Execute();
        }
    }

    private void BottomPanelRotateLeftClicked()
    {
        new RotatePlacingObjectCommand().Execute(false);
    }

    private void BottomPanelRotateRightClicked()
    {
        new RotatePlacingObjectCommand().Execute(true);
    }

    private void BottomPanelFinishPlacingClicked()
    {
        _gameStateModel.ResetPlacingState();
    }

    private void BottomPanelInteriorClicked()
    {
        _gameStateModel.SetGameState(GameStateName.ShopInterior);
    }

    private void BottomPanelInteriorCloseClicked()
    {
        _gameStateModel.SetGameState(GameStateName.ShopSimulation);
    }

    private void OnMouseCellCoordsUpdated(Vector2Int newCoords)
    {
        if (_gameStateModel.PlacingShopObjectModel != null)
        {
            _gameStateModel.PlacingShopObjectModel.Coords = newCoords;
        }
    }

    private void OnUIBottomPanelPlaceShelfClicked(int shelfNumericId)
    {
        new UIRequestPlaceShelfCommand().Execute(shelfNumericId);
    }

    private void OnUIBottomPanelPlaceFloorClicked(int floorNumericId)
    {
        new UIRequestPlacingDecorationCommand().Execute(ShopDecorationObjectType.Floor, floorNumericId);
    }

    private void OnUIBottomPanelPlaceWallClicked(int wallNumericId)
    {
        new UIRequestPlacingDecorationCommand().Execute(ShopDecorationObjectType.Wall, wallNumericId);
    }

    private void OnUIBottomPanelPlaceWindowClicked(int windowNumericId)
    {
        new UIRequestPlacingDecorationCommand().Execute(ShopDecorationObjectType.Window, windowNumericId);
    }

    private void OnUIBottomPanelPlaceDoorClicked(int doorNumericId)
    {
        new UIRequestPlacingDecorationCommand().Execute(ShopDecorationObjectType.Door, doorNumericId);
    }
}
