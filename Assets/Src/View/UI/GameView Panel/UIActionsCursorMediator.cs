using Src.Common;
using Src.Model;
using UnityEngine;

namespace Src.View.UI.GameView_Panel
{
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
        private AllFriendsShopsActionsModel _friendsActionsDataModel;

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
            _friendsActionsDataModel = _playerModelHolder.UserModel.FriendsActionsDataModels;
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
                    case ActionStateName.PlacingProductPlayer:
                    case ActionStateName.PlacingProductFriend:
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
                    _dispatcher.UIMouseMoved -= OnMouseMoved;
                    _dispatcher.UIMouseMoved += OnMouseMoved;
                }
            }
            else if (previousState != ActionStateName.None)
            {
                _dispatcher.UIMouseMoved -= OnMouseMoved;
                switch (previousState)
                {
                    case ActionStateName.PlacingProductPlayer:
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
            _friendsActionsDataModel.ActionDataAmountChanged += OnActionAmountChanged;
        }

        private void UnsubscribeFromActionAmountChanged()
        {
            _friendsActionsDataModel.ActionDataAmountChanged -= OnActionAmountChanged;
        }

        private void OnActionAmountChanged(string friendId, FriendShopActionData friendShopActionData)
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
                case ActionStateName.PlacingProductPlayer:
                case ActionStateName.PlacingProductFriend:
                    _actionCursorView.SetText($"{_placingProductModel.Amount}x");
                    break;
                case ActionStateName.FriendShopTakeProduct:
                    UpdateActionAmount(FriendShopActionId.TakeProduct);
                    break;
                case ActionStateName.FriendShopAddUnwash:
                    UpdateActionAmount(FriendShopActionId.AddUnwash);
                    break;
            }
        }

        private void UpdateActionAmount(FriendShopActionId actionId)
        {
            var friendUid = _gameStateModel.ViewingUserModel.Uid;
            var actionData = _friendsActionsDataModel.GetFriendShopActionsModel(friendUid).ActionsById[actionId];
            _actionCursorView.SetText($"{actionData.RestAmount}x");
        }
    }
}
