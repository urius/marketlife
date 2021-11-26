using System;
using UnityEngine;

public class RootGameViewMediator : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private SpriteRenderer _guiCursorRenderer;

    private UpdatesProvider _updatesProvider;
    private Dispatcher _dispatcher;
    private GameStateModel _gameStateModel;
    private GridCalculator _gridCalculator;
    private TutorialUIElementsProvider _tutorialUIElementsProvider;
    private int NextRealtimeSecondUpdate;
    private int NextGameplaySecondUpdate;

    private void Awake()
    {
        _updatesProvider = UpdatesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;

        _tutorialUIElementsProvider.SetElement(TutorialUIElement.ShopFloorTransform, transform);
        SetupGridCalculator();
        NextRealtimeSecondUpdate = (int)Time.realtimeSinceStartup + 1;
        NextGameplaySecondUpdate = NextRealtimeSecondUpdate;

        Activate();
    }

    private void Activate()
    {
        _dispatcher.MouseCellCoordsUpdated += OnMouseCellCoordsUpdated;
        _gameStateModel.GameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateName previousState, GameStateName currentState)
    {
        switch (currentState)
        {
            case GameStateName.PlayerShopSimulation:
                _guiCursorRenderer.color = Color.green;
                break;
            case GameStateName.PlayerShopInterior:
                _guiCursorRenderer.color = Color.blue;
                break;
        }
    }

    private void OnMouseCellCoordsUpdated(Vector2Int cellCoords)
    {
        _guiCursorRenderer.transform.position = _gridCalculator.CellToWorld(cellCoords);
    }

    private void FixedUpdate()
    {
        if (_gameStateModel.IsGamePaused == false)
        {
            _updatesProvider.CallGameplayUpdate();
            if (Time.realtimeSinceStartup >= NextGameplaySecondUpdate)
            {
                NextGameplaySecondUpdate = (int)Time.realtimeSinceStartup + 1;
                _updatesProvider.CallGameplaySecondUpdate();
            }
        }
        _updatesProvider.CallRealtimeUpdate();
        if (Time.realtimeSinceStartup >= NextRealtimeSecondUpdate)
        {
            NextRealtimeSecondUpdate = (int)Time.realtimeSinceStartup + 1;
            _updatesProvider.CallRealtimeSecondUpdate();
        }
        UpdateCursorAnimation();
    }

    private void UpdateCursorAnimation()
    {
        var alpha = (1 + (float)Math.Sin(Time.frameCount * 0.2f)) * 0.25f + 0.5f;
        _guiCursorRenderer.color = _guiCursorRenderer.color.SetAlpha(alpha);
    }

    private void SetupGridCalculator()
    {
        _gridCalculator = GridCalculator.Instance;
        _gridCalculator.SetGrid(_grid);
    }

    private void OnDrawGizmos()
    {
        if (_gridCalculator == null)
        {
            SetupGridCalculator();
        }

        return;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        if (playerModel != null)
        {
            foreach (var customer in playerModel.SessionDataModel.Customers)
            {
                if (customer.Path != null)
                {
                    foreach (var step in customer.Path)
                    {
                        Gizmos.DrawSphere(_gridCalculator.CellToWorld(step), 0.5f);
                    }
                }
            }
        }
    }
}
