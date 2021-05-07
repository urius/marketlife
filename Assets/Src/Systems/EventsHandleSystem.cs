using System;
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
        _dispatcher.UIBottomPanelPlaceShelfClicked += OnUIBottomPanelPlaceShelfClicked;
        _dispatcher.MouseCellCoordsUpdated += OnMouseCellCoordsUpdated;
        _dispatcher.BottomPanelInteriorClicked += BottomPanelInteriorClicked;
        _dispatcher.BottomPanelInteriorCloseClicked += BottomPanelInteriorCloseClicked;
        _dispatcher.BottomPanlelFinishPlacingClicked += BottomPanelFinishPlacingClicked;
        _dispatcher.BottomPanelRotateRightClicked += BottomPanelRotateRightClicked;
        _dispatcher.BottomPanelRotateLeftClicked += BottomPanelRotateLeftClicked;
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

    private void OnUIBottomPanelPlaceShelfClicked(int shelfId)
    {
        new UIRequestPlaceShelfCommand().Execute(shelfId);
    }
}
