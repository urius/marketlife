using Src.Common;
using Src.Model;
using Src.Model.ShopObjects;
using UnityEngine;

namespace Src.View.UI.GameView_Panel
{
    public class ShopObjectActionsPanelMediator : IMediator
    {
        private readonly RectTransform _contentTransform;
        private readonly Dispatcher _dispatcher;
        private readonly GameStateModel _gameStateModel;
        private readonly MouseDataProvider _mouseCellCoordsProvider;
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
            _mouseCellCoordsProvider = MouseDataProvider.Instance;
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
            _gameStateModel.HighlightStateChanged += OnHighlightStateChanged;
            _gameStateModel.ActionStateChanged += OnActionStateChanged;
        }

        private void Deactivate()
        {
            _gameStateModel.HighlightStateChanged -= OnHighlightStateChanged;
            _gameStateModel.ActionStateChanged -= OnActionStateChanged;
        }

        private void OnActionStateChanged(ActionStateName prevState, ActionStateName currentState)
        {
            Update();
        }

        private void OnHighlightStateChanged()
        {
            Update();
        }

        private async void Update()
        {
            if (_gameStateModel.GameState != GameStateName.PlayerShopInterior || _gameStateModel.ActionState != ActionStateName.None)
            {
                HideActionsView();
                return;
            }

            if (_gameStateModel.HighlightState.IsHighlighted)
            {
                ShowActionsView();
                SetupActionsView();

                UnsubscribeFromActionsView(_actionsView);
                await _actionsView.AppearAsync();
                if (_gameStateModel.HighlightState.IsHighlighted)
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

        private void SetupActionsView()
        {
            var highlightedShopObject = _gameStateModel.HighlightState.HighlightedShopObject;
            if (highlightedShopObject != null)
            {
                switch (highlightedShopObject.Type)
                {
                    case ShopObjectType.Shelf:
                        _actionsView.SetupButtons();
                        break;
                    case ShopObjectType.CashDesk:
                        _actionsView.SetupButtons(showRemoveButton: false);
                        break;
                }
            }
            else
            {
                _actionsView.SetupButtons(showRotateButtons: false);
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
            var screenPoint = _screenCalculator.CellToScreenPoint(_mouseCellCoordsProvider.MouseCellCoords);
            if (_screenCalculator.ScreenPointToWorldPointInRectangle(_contentTransform, screenPoint, out var position))
            {
                _actionsView.transform.position = position;
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
}
