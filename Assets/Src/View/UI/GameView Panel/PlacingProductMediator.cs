using UnityEngine;
using UnityEngine.UI;

public class PlacingProductMediator : IMediator
{
    private readonly RectTransform _contentTransform;
    private readonly Dispatcher _dispatcher;
    private readonly GameStateModel _gameStateModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly SpritesProvider _spritesProvider;
    private readonly ScreenCalculator _screenCalculator;

    private GameObject _currentPlacingProductGo;

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
            _currentPlacingProductGo = GameObject.Instantiate(_prefabsHolder.UIPlacingProductPrefab, _contentTransform);
            SetupView();
            _dispatcher.MouseMoved -= OnMouseMoved;
            _dispatcher.MouseMoved += OnMouseMoved;
        }
        else if (previousState == PlacingStateName.PlacingProduct)
        {
            _dispatcher.MouseMoved -= OnMouseMoved;
            if (_currentPlacingProductGo != null)
            {
                GameObject.Destroy(_currentPlacingProductGo);
                _currentPlacingProductGo = null;
            }
        }
    }

    private void OnMouseMoved()
    {
        var screenPoint = Input.mousePosition;
        if (_screenCalculator.ScreenPointToWorldPointInRectangle(_contentTransform, screenPoint, out var position))
        {
            _currentPlacingProductGo.transform.position = position;
        }
    }

    private void SetupView()
    {
        var warehouseModel = _gameStateModel.PlayerShopModel.WarehouseModel;
        var image = _currentPlacingProductGo.GetComponentInChildren<Image>();
        var productKey = warehouseModel.Slots[_gameStateModel.PlacingProductWarehouseSlotIndex].Product.Config.Key;
        image.sprite = _spritesProvider.GetProductIcon(productKey);
    }
}
