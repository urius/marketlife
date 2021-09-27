using System;
using UnityEngine;

public class UIActionsCursorMediator : IMediator
{
    private readonly RectTransform _contentTransform;
    private readonly Dispatcher _dispatcher;
    private readonly GameStateModel _gameStateModel;
    private readonly PlayerModelHolder _playerModelHolder;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly SpritesProvider _spritesProvider;
    private readonly ScreenCalculator _screenCalculator;

    private PlacingProductView _actionCursorView;
    private ProductModel _placingProductModel;
    private AvailableFriendShopActionsDataModel _playerActionsDataModel;

    public UIActionsCursorMediator(RectTransform contentTransform)
    {
        _contentTransform = contentTransform;

        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _screenCalculator = ScreenCalculator.Instance;
    }

    public void Mediate()
    {
        _playerActionsDataModel = _playerModelHolder.UserModel.ActionsDataModel;
        _gameStateModel.ActionStateChanged += OnActionStateChanged;
    }

    public void Unmediate()
    {
        _gameStateModel.ActionStateChanged -= OnActionStateChanged;
    }

    private void OnActionStateChanged(ActionStateName previousState, ActionStateName currentState)
    {
        if (currentState != ActionStateName.None)
        {
            switch (currentState)
            {
                case ActionStateName.PlacingProduct:
                    _placingProductModel = _playerModelHolder.ShopModel.WarehouseModel.Slots[_gameStateModel.PlacingProductWarehouseSlotIndex].Product;
                    CreateCursorWithIcon(_spritesProvider.GetProductIcon(_placingProductModel.Config.Key));
                    SubscribeOnPlacingProduct();
                    break;
                case ActionStateName.FriendShopTakeProduct:
                    CreateCursorWithIcon(_spritesProvider.GetTakeActionIcon());
                    SubscribeOnActionAmountChanged();
                    break;
                case ActionStateName.FriendShopAddUnwash:
                    CreateCursorWithIcon(_spritesProvider.GetAddUnwashActionIcon());
                    SubscribeOnActionAmountChanged();
                    break;
            }

            if (_actionCursorView != null)
            {
                UpdateAmount();
                UpdateViewPosition();
                _dispatcher.MouseMoved -= OnMouseMoved;
                _dispatcher.MouseMoved += OnMouseMoved;
            }
        }
        else if (previousState != ActionStateName.None)
        {
            _dispatcher.MouseMoved -= OnMouseMoved;
            switch (previousState)
            {
                case ActionStateName.PlacingProduct:
                    UnsubscribeFromPlacingProduct();
                    _placingProductModel = null;
                    break;
                case ActionStateName.FriendShopTakeProduct:
                    UnsubscribeFromActionAmountChanged();
                    break;
                case ActionStateName.FriendShopAddUnwash:
                    UnsubscribeFromActionAmountChanged();
                    break;
            }

            if (_actionCursorView != null)
            {
                GameObject.Destroy(_actionCursorView.gameObject);
                _actionCursorView = null;
            }
        }
    }

    private void SubscribeOnActionAmountChanged()
    {
        _playerActionsDataModel.ActionDataAmountChanged += OnActionAmountChanged;
    }

    private void UnsubscribeFromActionAmountChanged()
    {
        _playerActionsDataModel.ActionDataAmountChanged -= OnActionAmountChanged;
    }

    private void OnActionAmountChanged(AvailableFriendShopActionData friendShopActionData)
    {
        UpdateAmount();
    }

    private void CreateCursorWithIcon(Sprite iconSprite)
    {
        var go = GameObject.Instantiate(_prefabsHolder.UIFlyingActionPrefab, _contentTransform);
        _actionCursorView = go.GetComponent<PlacingProductView>();
        _actionCursorView.SetImageSprite(iconSprite);
    }

    private void SubscribeOnPlacingProduct()
    {
        _placingProductModel.AmountChanged += OnProductAmountChanged;
    }

    private void UnsubscribeFromPlacingProduct()
    {
        _placingProductModel.AmountChanged -= OnProductAmountChanged;
    }

    private void OnProductAmountChanged(int deltaAmount)
    {
        UpdateAmount();
    }

    private void OnMouseMoved()
    {
        UpdateViewPosition();
    }

    private void UpdateViewPosition()
    {
        var screenPoint = Input.mousePosition;
        if (_screenCalculator.ScreenPointToWorldPointInRectangle(_contentTransform, screenPoint, out var position))
        {
            _actionCursorView.transform.position = position;
        }
    }

    private void UpdateAmount()
    {
        switch (_gameStateModel.ActionState)
        {
            case ActionStateName.PlacingProduct:
                _actionCursorView.SetText($"{_placingProductModel.Amount}x");
                break;
            case ActionStateName.FriendShopTakeProduct:
                UpdateActionAmount(FriendShopActionId.Take);
                break;
            case ActionStateName.FriendShopAddUnwash:
                UpdateActionAmount(FriendShopActionId.AddUnwash);
                break;
        }
    }

    private void UpdateActionAmount(FriendShopActionId actionId)
    {
        var actionData = _playerActionsDataModel.ActionsById[actionId];
        _actionCursorView.SetText($"{actionData.RestAmount}x");
    }
}
