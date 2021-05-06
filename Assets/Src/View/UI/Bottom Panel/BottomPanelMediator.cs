using Cysharp.Threading.Tasks;
using UnityEngine;

public class BottomPanelMediator : UINotMonoMediatorBase
{
    private readonly BottomPanelView _view;
    private readonly Dispatcher _dispatcher;
    private readonly GameStateModel _gameStateModel;

    private UINotMonoMediatorBase _currentTabMediator;

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

        _currentTabMediator = new UIBottomPanelShelfsTabMediator(_view);
        _currentTabMediator.Mediate();

        Activate();

        await ShowButtonsForStateAsync(_gameStateModel.GameState);
    }

    private void Activate()
    {
        _view.InteriorButtonClicked += OnInteriorButtonClicked;
        _view.InteriorCloseButtonClicked += OnInteriorCloseButtonClicked;
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
                await HideButtonsForStateAsync(_gameStateModel.GameState);
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
                await ShowButtonsForStateAsync(_gameStateModel.GameState);
            }
        }
    }

    private async void OnGameStateChanged(GameStateName previousState, GameStateName newState)
    {
        using (_view.SetBlockedDisposable())
        {
            await HideButtonsForStateAsync(previousState);
            await ShowButtonsForStateAsync(newState);
        }
    }

    private UniTask ShowButtonsForStateAsync(GameStateName newState)
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

    private UniTask HideButtonsForStateAsync(GameStateName previousState)
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

    private void OnInteriorButtonClicked()
    {
        _dispatcher.BottomPanelInteriorClicked();
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

    public override void Unmediate()
    {
        base.Unmediate();
    }
}
