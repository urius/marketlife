using System;
using UnityEngine;

public class ShopObjectActionsPanelMediator : IMediator
{
    private readonly RectTransform _contentTransform;
    private readonly Dispatcher _dispatcher;
    private readonly GameStateModel _gameStateModel;
    private readonly MouseCellCoordsProvider _mouseCellCoordsProvider;
    private readonly GridCalculator _gridCalculator;
    private readonly ScreenCalculator _screenCalculator;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly PoolCanvasProvider _poolCanvasProvider;

    private ShopObjectActionsPanelView _actionsView;
    private bool _isShown;

    public ShopObjectActionsPanelMediator(RectTransform contentTransform)
    {
        _contentTransform = contentTransform;

        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;
        _mouseCellCoordsProvider = MouseCellCoordsProvider.Instance;
        _gridCalculator = GridCalculator.Instance;
        _screenCalculator = ScreenCalculator.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _poolCanvasProvider = PoolCanvasProvider.Instance;
    }

    public void Mediate()
    {
        Activate();
    }

    public void Unmediate()
    {
        Deactivate();
    }

    private void Activate()
    {
        _gameStateModel.HighlightShopObjectChanged += OnHighlightShopObjectChanged;
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
    }

    private void Deactivate()
    {
        _gameStateModel.HighlightShopObjectChanged -= OnHighlightShopObjectChanged;
        _gameStateModel.PlacingStateChanged -= OnPlacingStateChanged;
    }

    private void OnPlacingStateChanged(PlacingStateName prevState, PlacingStateName currentState)
    {
        Update();
    }

    private void OnHighlightShopObjectChanged()
    {
        Update();
    }

    private async void Update()
    {
        if (_gameStateModel.GameState != GameStateName.ShopInterior || _gameStateModel.PlacingState != PlacingStateName.None)
        {
            HideActionsView();
            return;
        }

        if (_gameStateModel.HighlightedShopObject != null)
        {
            ShowActionsView();

            var worldPoint = _gridCalculator.CellToWord(_mouseCellCoordsProvider.MouseCellCoords);
            var screenPoint = _screenCalculator.WorldToScreenPoint(worldPoint);
            if (_screenCalculator.ScreenPointToWroldPointInRectangle(_contentTransform, screenPoint, out var position))
            {
                _actionsView.transform.position = position;
            }

            switch (_gameStateModel.HighlightedShopObject.Type)
            {
                case ShopObjectType.Shelf:
                    _actionsView.SetupButtons();
                    break;
                case ShopObjectType.CashDesk:
                    _actionsView.SetupButtons(showRemoveButton: false);
                    break;
            }

            UnsubscribeFromActionsView(_actionsView);
            await _actionsView.AppearAsync();
            if (_gameStateModel.HighlightedShopObject != null)
            {
                SubscribeForActionsView(_actionsView);
            }
            else
            {
                HideActionsView();
            }
        }
        else
        {
            HideActionsView();
        }
    }

    private void ShowActionsView()
    {
        if (_actionsView == null)
        {
            var actionsViewGo = GameObject.Instantiate(_prefabsHolder.UIShelfActionsPrefab, _contentTransform);
            _actionsView = actionsViewGo.GetComponent<ShopObjectActionsPanelView>();
        }
        else
        {
            _actionsView.transform.SetParent(_contentTransform);
        }
        _isShown = true;
    }

    private void HideActionsView()
    {
        if (_isShown && _actionsView != null)
        {
            UnsubscribeFromActionsView(_actionsView);
            _actionsView.transform.SetParent(_poolCanvasProvider.PoolCanvasTransform);
            _isShown = false;
        }
    }

    private void SubscribeForActionsView(ShopObjectActionsPanelView actionsView)
    {
        actionsView.RotateRightClicked += OnRotateRightClicked;
        actionsView.RotateLeftClicked += OnRotateLeftClicked;
        actionsView.MoveClicked += OnMoveClicked;
        actionsView.RemoveClicked += OnRemoveClicked;
    }

    private void UnsubscribeFromActionsView(ShopObjectActionsPanelView actionsView)
    {
        actionsView.RotateRightClicked -= OnRotateRightClicked;
        actionsView.RotateLeftClicked -= OnRotateLeftClicked;
        actionsView.MoveClicked -= OnMoveClicked;
        actionsView.RemoveClicked -= OnRemoveClicked;
    }

    private void OnRotateRightClicked()
    {
        _dispatcher.UIActionsRotateRightClicked();
    }

    private void OnRotateLeftClicked()
    {
        _dispatcher.UIActionsRotateLeftClicked();
    }

    private void OnMoveClicked()
    {
        _dispatcher.UIActionsMoveClicked();
    }

    private void OnRemoveClicked()
    {
        _dispatcher.UIActionsRemoveClicked();
    }
}
