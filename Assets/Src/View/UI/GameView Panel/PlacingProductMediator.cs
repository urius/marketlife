using UnityEngine;

public class PlacingProductMediator : IMediator
{
    private readonly RectTransform _contentTransform;
    private readonly Dispatcher _dispatcher;
    private readonly GameStateModel _gameStateModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly SpritesProvider _spritesProvider;
    private readonly ScreenCalculator _screenCalculator;

    private PlacingProductView _placingProductView;
    private ProductModel _placingProductModel;

    public PlacingProductMediator(RectTransform contentTransform)
    {
        _contentTransform = contentTransform;

        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _screenCalculator = ScreenCalculator.Instance;
    }

    public void Mediate()
    {
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
    }

    public void Unmediate()
    {
        _gameStateModel.PlacingStateChanged -= OnPlacingStateChanged;
    }

    private void OnPlacingStateChanged(PlacingStateName previousState, PlacingStateName currentState)
    {
        if (currentState == PlacingStateName.PlacingProduct)
        {
            _placingProductModel = _gameStateModel.PlayerShopModel.WarehouseModel.Slots[_gameStateModel.PlacingProductWarehouseSlotIndex].Product;
            var go = GameObject.Instantiate(_prefabsHolder.UIPlacingProductPrefab, _contentTransform);
            _placingProductView = go.GetComponent<PlacingProductView>();
            SetupView();
            UpdateViewPosition();

            _dispatcher.MouseMoved -= OnMouseMoved;
            _dispatcher.MouseMoved += OnMouseMoved;
            SubscribeForPlacingProduct();
        }
        else if (previousState == PlacingStateName.PlacingProduct)
        {
            _dispatcher.MouseMoved -= OnMouseMoved;
            UnsubscribeFromPlacingProduct();

            _placingProductModel = null;
            if (_placingProductView != null)
            {
                GameObject.Destroy(_placingProductView.gameObject);
                _placingProductView = null;
            }
        }
    }

    private void SubscribeForPlacingProduct()
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
            _placingProductView.transform.position = position;
        }
    }

    private void SetupView()
    {
        _placingProductView.SetImageSprite(_spritesProvider.GetProductIcon(_placingProductModel.Config.Key));
        UpdateAmount();
    }

    private void UpdateAmount()
    {
        _placingProductView.SetText($"{_placingProductModel.Amount}x");
    }
}
