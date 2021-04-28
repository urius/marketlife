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
