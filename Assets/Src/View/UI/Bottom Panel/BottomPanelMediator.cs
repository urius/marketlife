using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BottomPanelMediator : UINotMonoMediatorBase
{
    private readonly BottomPanelView _view;
    private readonly Dispatcher _dispatcher;
    private readonly GameStateModel _gameStateModel;
    private readonly Dictionary<GameStateName, UIBottomPanelSubMediatorBase> _lastTabMediatorForState = new Dictionary<GameStateName, UIBottomPanelSubMediatorBase>();

    private UIBottomPanelSubMediatorBase _currentTabMediator;

    public BottomPanelMediator(BottomPanelView view)
        : base(view.transform as RectTransform)
    {
        _view = view;
        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;
    }

    public override async void Mediate()
    {
        base.Mediate();

        Activate();

        _currentTabMediator = GetTabMediatorForGameState(_gameStateModel.GameState);
        _currentTabMediator?.Mediate();

        await ShowTopButtonsForStateAsync(_gameStateModel.GameState);
    }

    private UIBottomPanelSubMediatorBase GetTabMediatorForGameState(GameStateName gameState)
    {
        UIBottomPanelSubMediatorBase result = null;
        if (_lastTabMediatorForState.ContainsKey(gameState))
        {
            result = _lastTabMediatorForState[gameState];
        }
        else
        {
            switch (gameState)
            {
                case GameStateName.ShopInterior:
                    result = new UIBottomPanelShelfsTabMediator(_view);
                    break;
                case GameStateName.ShopSimulation:
                    //TODO: default simulationMediator (friends?)
                    break;
            }
        }
        _lastTabMediatorForState[gameState] = result;

        return result;
    }

    private void Activate()
    {
        _view.InteriorButtonClicked += OnInteriorButtonClicked;
        _view.InteriorCloseButtonClicked += OnInteriorCloseButtonClicked;
        _view.InteriorObjectsButtonClicked += OnInteriorObjectsButtonClicked;
        _view.InteriorFloorsButtonClicked += OnInteriorFloorsButtonClicked;
        _view.InteriorWallsButtonClicked += OnInteriorWallsButtonClicked;
        _view.InteriorWindowsButtonClicked += OnInteriorWindowsButtonClicked;
        _view.InteriorDoorsButtonClicked += OnInteriorDoorsButtonClicked;
        _view.FinishPlacingClicked += OnFinishPlacingButtonClicked;
        _view.RotateRightClicked += OnRotateRightButtonClicked;
        _view.RotateLeftClicked += OnRotateLeftButtonClicked;

        _gameStateModel.GameStateChanged += OnGameStateChanged;
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
    }

    private async void OnPlacingStateChanged(PlacingStateName previousState, PlacingStateName newState)
    {
        using (_view.SetBlockedDisposable())
        {
            //placing
            if (previousState == PlacingStateName.None && newState != PlacingStateName.None)
            {
                var minimizeTask = _view.MinimizePanelAsync();
                await HideTopButtonsForStateAsync(_gameStateModel.GameState);
                await minimizeTask;
                switch (newState)
                {
                    case PlacingStateName.PlacingShopObject:
                        await _view.ShowPlacingButtonsAsync();
                        break;
                }
            }
            //no placing
            else if (previousState != PlacingStateName.None && newState == PlacingStateName.None)
            {
                var maximizeTask = _view.MaximizePanelAsync();
                await _view.HidePlacingButtonsAsync();
                await maximizeTask;
                await ShowTopButtonsForStateAsync(_gameStateModel.GameState);
            }
        }
    }

    private async void OnGameStateChanged(GameStateName previousState, GameStateName newState)
    {
        _currentTabMediator?.Unmediate();
        _currentTabMediator = GetTabMediatorForGameState(_gameStateModel.GameState);
        _currentTabMediator?.Mediate();
        if (newState == GameStateName.ShopSimulation || newState == GameStateName.ShopInterior)
        {
            using (_view.SetBlockedDisposable())
            {
                await HideTopButtonsForStateAsync(previousState);
                await ShowTopButtonsForStateAsync(newState);
            }
        }
    }

    private UniTask ShowTopButtonsForStateAsync(GameStateName newState)
    {
        switch (newState)
        {
            case GameStateName.ShopInterior:
                return _view.ShowInteriorModeButtonsAsync();
            case GameStateName.ShopSimulation:
                return _view.ShowSimulationModeButtonsAsync();
        }

        return UniTask.CompletedTask;
    }

    private UniTask HideTopButtonsForStateAsync(GameStateName previousState)
    {
        switch (previousState)
        {
            case GameStateName.ShopSimulation:
                return _view.HideSimulationModeButtonsAsync();
            case GameStateName.ShopInterior:
                return _view.HideInteriorModeButtonsAsync();
        }

        return UniTask.CompletedTask;
    }

    private void SwowTab(UIBottomPanelSubMediatorBase tabMediator)
    {
        _currentTabMediator.Unmediate();
        _currentTabMediator = tabMediator;
        _lastTabMediatorForState[_gameStateModel.GameState] = _currentTabMediator;
        _currentTabMediator.Mediate();
    }

    private void OnInteriorButtonClicked()
    {
        _dispatcher.BottomPanelInteriorClicked();
    }

    private void OnInteriorObjectsButtonClicked()
    {
        SwowTab(new UIBottomPanelShelfsTabMediator(_view));
    }

    private void OnInteriorFloorsButtonClicked()
    {
        SwowTab(new UIBottomPanelFloorsTabMediator(_view));
    }

    private void OnInteriorWallsButtonClicked()
    {
        SwowTab(new UIBottomPanelWallsTabMediator(_view));
    }
    
    private void OnInteriorWindowsButtonClicked()
    {
        SwowTab(new UIBottomPanelWindowsTabMediator(_view));
    }

    private void OnInteriorDoorsButtonClicked()
    {
        SwowTab(new UIBottomPanelDoorsTabMediator(_view));
    }    

    private void OnInteriorCloseButtonClicked()
    {
        _dispatcher.BottomPanelInteriorCloseClicked();
    }

    private void OnRotateRightButtonClicked()
    {
        _dispatcher.BottomPanelRotateRightClicked();
    }

    private void OnRotateLeftButtonClicked()
    {
        _dispatcher.BottomPanelRotateLeftClicked();
    }

    private void OnFinishPlacingButtonClicked()
    {
        _dispatcher.BottomPanlelFinishPlacingClicked();
    }
}
