using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BottomPanelMediator : UINotMonoMediatorBase
{
    private readonly BottomPanelView _view;
    private readonly Dispatcher _dispatcher;
    private readonly GameStateModel _gameStateModel;
    private readonly BottomPanelViewModel _viewModel;
    private readonly GameConfigManager _configManager;
    private readonly AudioManager _audioManager;
    private readonly Dictionary<GameStateName, IMediator> _lastTabMediatorForState = new Dictionary<GameStateName, IMediator>();

    private IMediator _currentTabMediator;

    public BottomPanelMediator(BottomPanelView view)
        : base(view.transform as RectTransform)
    {
        _view = view;
        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;
        _viewModel = _gameStateModel.BottomPanelViewModel;
        _configManager = GameConfigManager.Instance;
        _audioManager = AudioManager.Instance;
    }

    public override async void Mediate()
    {
        base.Mediate();

        Activate();
        UpdateTabMediator();
        _view.SetAutoPlacePriceGold(_configManager.MainConfig.AutoPlacePriceGold);

        await ShowBgAndTopButtonsForStateAsync(_gameStateModel.GameState);
    }

    private void PlayButtonSound()
    {
        _audioManager.PlaySound(SoundNames.Button5);
    }

    private IMediator GetTabMediatorForGameState(GameStateName gameState)
    {
        return gameState switch
        {
            GameStateName.ShopSimulation => GetSimulationModeTabMediator(),
            GameStateName.ShopInterior => GetInteriorModeTabMediator(),
            _ => null,
        };
    }

    private IMediator GetSimulationModeTabMediator()
    {
        return _viewModel.SimulationModeTab switch
        {
            BottomPanelSimulationModeTab.Friends => null, //todo friends mediator
            BottomPanelSimulationModeTab.Warehouse => new UIBottomPanelWarehouseTabMediator(_view),
            _ => throw new InvalidOperationException($"{nameof(GetSimulationModeTabMediator)}i: interior tab {_viewModel.InteriorModeTab} is not supported"),
        };
    }

    private IMediator GetInteriorModeTabMediator()
    {
        return _viewModel.InteriorModeTab switch
        {
            BottomPanelInteriorModeTab.Shelfs => new UIBottomPanelShelfsTabMediator(_view),
            BottomPanelInteriorModeTab.Floors => new UIBottomPanelFloorsTabMediator(_view),
            BottomPanelInteriorModeTab.Walls => new UIBottomPanelWallsTabMediator(_view),
            BottomPanelInteriorModeTab.Windows => new UIBottomPanelWindowsTabMediator(_view),
            BottomPanelInteriorModeTab.Doors => new UIBottomPanelDoorsTabMediator(_view),
            _ => throw new InvalidOperationException($"{nameof(GetInteriorModeTabMediator)}: interior tab {_viewModel.InteriorModeTab} is not supported"),
        };
    }

    private void Activate()
    {
        _view.PointerEnter += OnBottomPanelPointerEnter;
        _view.PointerExit += OnBottomPanelPointerExit;
        _view.FriendsButtonClicked += OnFriendsButtonClicked;
        _view.WarehouseButtonClicked += OnWarehouseButtonClicked;
        _view.InteriorButtonClicked += OnInteriorButtonClicked;
        _view.ManageButtonClicked += OnManageButtonClicked;
        _view.InteriorCloseButtonClicked += OnInteriorCloseButtonClicked;
        _view.InteriorObjectsButtonClicked += OnInteriorObjectsButtonClicked;
        _view.InteriorFloorsButtonClicked += OnInteriorFloorsButtonClicked;
        _view.InteriorWallsButtonClicked += OnInteriorWallsButtonClicked;
        _view.InteriorWindowsButtonClicked += OnInteriorWindowsButtonClicked;
        _view.InteriorDoorsButtonClicked += OnInteriorDoorsButtonClicked;
        _view.FinishPlacingClicked += OnFinishPlacingButtonClicked;
        _view.RotateRightClicked += OnRotateRightButtonClicked;
        _view.RotateLeftClicked += OnRotateLeftButtonClicked;
        _view.AutoPlaceClicked += OnAutoPlaceClicked;

        _viewModel.SimulationTabChanged += OnSimulationTabChanged;
        _viewModel.InteriorTabChanged += OnInteriorTabChanged;

        _gameStateModel.GameStateChanged += OnGameStateChanged;
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
    }

    private void OnSimulationTabChanged()
    {
        UpdateTabMediator();
    }

    private void OnInteriorTabChanged()
    {
        UpdateTabMediator();
    }

    private void OnAutoPlaceClicked()
    {
        _dispatcher.UIBottomPanelAutoPlaceClicked();
        PlayButtonSound();
    }

    private void OnManageButtonClicked()
    {
        _dispatcher.BottomPanelManageButtonClicked();
        PlayButtonSound();
    }

    private void OnBottomPanelPointerEnter()
    {
        _dispatcher.UIBottomPanelPointerEnter();
    }

    private void OnBottomPanelPointerExit()
    {
        _dispatcher.UIBottomPanelPointerExit();
    }

    private async void OnPlacingStateChanged(PlacingStateName previousState, PlacingStateName newState)
    {
        using (_view.SetBlockedDisposable())
        {
            //placing
            if (newState != PlacingStateName.None && previousState == PlacingStateName.None)
            {
                var minimizeTask = _view.MinimizePanelAsync();
                await HideTopButtonsForStateAsync(_gameStateModel.GameState);
                await minimizeTask;
                switch (newState)
                {
                    case PlacingStateName.PlacingNewShopObject:
                        await _view.ShowPlacingButtonsAsync(PlacingModeType.NewShopObject);
                        break;
                    case PlacingStateName.MovingShopObject:
                        await _view.ShowPlacingButtonsAsync(PlacingModeType.MovingShopObject);
                        break;
                    case PlacingStateName.PlacingNewFloor:
                    case PlacingStateName.PlacingNewWall:
                    case PlacingStateName.PlacingNewWindow:
                    case PlacingStateName.PlacingNewDoor:
                        await _view.ShowPlacingButtonsAsync(PlacingModeType.ShopDecoration);
                        break;
                    case PlacingStateName.PlacingProduct:
                        await _view.ShowPlacingButtonsAsync(PlacingModeType.Product);
                        break;
                }
            }
            //no placing
            else if (newState == PlacingStateName.None && previousState != PlacingStateName.None)
            {
                var maximizeTask = _view.MaximizePanelAsync();
                await _view.HidePlacingButtonsAsync();
                await maximizeTask;
                await ShowBgAndTopButtonsForStateAsync(_gameStateModel.GameState);
            }
        }
    }

    private async void OnGameStateChanged(GameStateName previousState, GameStateName newState)
    {
        if (_gameStateModel.IsPlayingState)
        {
            UpdateTabMediator();
        }

        if (newState == GameStateName.ShopSimulation || newState == GameStateName.ShopInterior)
        {
            using (_view.SetBlockedDisposable())
            {
                await HideTopButtonsForStateAsync(previousState);
                await ShowBgAndTopButtonsForStateAsync(newState);
            }
        }
    }

    private void UpdateTabMediator()
    {
        _currentTabMediator?.Unmediate();
        _currentTabMediator = GetTabMediatorForGameState(_gameStateModel.GameState);
        _currentTabMediator?.Mediate();
    }

    private UniTask ShowBgAndTopButtonsForStateAsync(GameStateName newState)
    {
        switch (newState)
        {
            case GameStateName.ShopInterior:
                _view.ShowInteriorModeBG();
                return _view.ShowInteriorModeButtonsAsync();
            case GameStateName.ShopSimulation:
                _view.ShowSimulationModeBG();
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

    private void SwowTab(IMediator tabMediator)
    {
        _currentTabMediator.Unmediate();
        _currentTabMediator = tabMediator;
        _lastTabMediatorForState[_gameStateModel.GameState] = _currentTabMediator;
        _currentTabMediator.Mediate();
    }    

    private void OnFriendsButtonClicked()
    {
        _dispatcher.BottomPanelFriendsClicked();
    }

    private void OnWarehouseButtonClicked()
    {
        _dispatcher.BottomPanelWarehouseClicked();
    }

    private void OnInteriorButtonClicked()
    {
        _dispatcher.BottomPanelInteriorClicked();
        PlayButtonSound();
    }

    private void OnInteriorObjectsButtonClicked()
    {
        _dispatcher.BottomPanelInteriorShelfsClicked();
        PlayButtonSound();
    }

    private void OnInteriorFloorsButtonClicked()
    {
        _dispatcher.BottomPanelInteriorFloorsClicked();
        PlayButtonSound();
    }

    private void OnInteriorWallsButtonClicked()
    {
        _dispatcher.BottomPanelInteriorWallsClicked();
        PlayButtonSound();
    }

    private void OnInteriorWindowsButtonClicked()
    {
        _dispatcher.BottomPanelInteriorWindowsClicked();
        PlayButtonSound();
    }

    private void OnInteriorDoorsButtonClicked()
    {
        _dispatcher.BottomPanelInteriorDoorsClicked();
        PlayButtonSound();
    }

    private void OnInteriorCloseButtonClicked()
    {
        _dispatcher.BottomPanelInteriorCloseClicked();
        PlayButtonSound();
    }

    private void OnRotateRightButtonClicked()
    {
        _dispatcher.BottomPanelRotateRightClicked();
        PlayButtonSound();
    }

    private void OnRotateLeftButtonClicked()
    {
        _dispatcher.BottomPanelRotateLeftClicked();
        PlayButtonSound();
    }

    private void OnFinishPlacingButtonClicked()
    {
        _dispatcher.BottomPanlelFinishPlacingClicked();
        PlayButtonSound();
    }
}
