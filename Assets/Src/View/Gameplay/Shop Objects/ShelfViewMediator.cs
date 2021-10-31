using UnityEngine;

public class ShelfViewMediator : ShopObjectMediatorBase
{
    private readonly Dispatcher _dispatcher;
    private readonly SpritesProvider _spritesProvider;
    private readonly ScreenCalculator _screenCalculator;
    private readonly GameStateModel _gameStateModel;
    //
    private SpriteRenderer _fullnessIndicatorView;
    private ShelfModel _shelfModel;

    public ShelfViewMediator(Transform parentTransform, ShelfModel shelfModel)
        : base(parentTransform, shelfModel)
    {
        _dispatcher = Dispatcher.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _screenCalculator = ScreenCalculator.Instance;
        _gameStateModel = GameStateModel.Instance;

        _shelfModel = shelfModel;
    }

    private ShelfView CurrentShelfView => CurrentView as ShelfView;

    public override void Mediate()
    {
        base.Mediate();

        Activate();
    }

    public override void Unmediate()
    {
        if (_fullnessIndicatorView != null)
        {
            GameObject.Destroy(_fullnessIndicatorView.gameObject);
            _fullnessIndicatorView = null;
        }

        Deactivate();

        base.Unmediate();
    }

    protected override void UpdateView()
    {
        base.UpdateView();

        CreateFullnessIndicatorIfNeeded();
        UpdateProductViews();
        UpdateFullnessIndicator();
    }

    private void CreateFullnessIndicatorIfNeeded()
    {
        if (_fullnessIndicatorView == null)
        {
            _fullnessIndicatorView = new ViewsFactory().CreateSpriteRenderer(ParentTransform, _spritesProvider.GetExclamationMarkSprite(), "fullness_indicator");
            var pos = CurrentView.transform.position;
            pos.z = -0.5f;
            _fullnessIndicatorView.sortingLayerName = SortingLayers.GUI;
            _fullnessIndicatorView.transform.position = pos;
            //WallsHelper.PlaceVertical(_fullnessIndicatorView.transform);
            //_fullnessIndicatorView.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            _fullnessIndicatorView.color = _fullnessIndicatorView.color.SetAlpha(0.5f);
        }
    }

    private void Activate()
    {
        _gameStateModel.GameStateChanged += OnGameStateChanged;
        _shelfModel.ProductIsSetOnSlot += OnProductIsSetOnSlot;
        _shelfModel.ProductRemovedFromSlot += OnProductRemovedFromSlot;
        _shelfModel.ProductAmountChangedOnSlot += OnProductAmountChangedOnSlot;
    }

    private void Deactivate()
    {
        _gameStateModel.GameStateChanged -= OnGameStateChanged;
        _shelfModel.ProductIsSetOnSlot -= OnProductIsSetOnSlot;
        _shelfModel.ProductRemovedFromSlot -= OnProductRemovedFromSlot;
        _shelfModel.ProductAmountChangedOnSlot -= OnProductAmountChangedOnSlot;
    }

    private void OnGameStateChanged(GameStateName prev, GameStateName current)
    {
        UpdateFullnessIndicator();
    }

    private void OnProductIsSetOnSlot(ShelfModel shelfModel, int slotIndex)
    {
        UpdateProductViewOnSlot(slotIndex);
        UpdateFullnessIndicator();
        var product = _shelfModel.Slots[slotIndex].Product;
        RequestFlyingProduct(product.Config.Key, product.Amount);
    }

    private void OnProductRemovedFromSlot(ShelfModel shelfModel, int slotIndex, ProductModel removedProduct)
    {
        UpdateProductViewOnSlot(slotIndex);
        UpdateFullnessIndicator();
        RequestFlyingProduct(removedProduct.Config.Key, -removedProduct.Amount);
    }

    private void OnProductAmountChangedOnSlot(ShelfModel shelfModel, int slotIndex, int deltaAmount)
    {
        UpdateProductViewOnSlot(slotIndex);
        UpdateFullnessIndicator();
        var product = _shelfModel.Slots[slotIndex].Product;
        RequestFlyingProduct(product.Config.Key, deltaAmount);
    }

    private void RequestFlyingProduct(string key, int amount)
    {
        var screenPoint = _screenCalculator.CellToScreenPoint(_shelfModel.Coords);
        _dispatcher.UIRequestFlyingProduct(screenPoint, key, amount);
    }

    private void UpdateProductViews()
    {
        for (var i = 0; i < _shelfModel.Slots.Length; i++)
        {
            UpdateProductViewOnSlot(i);
        }
    }

    private void UpdateProductViewOnSlot(int slotIndex)
    {
        var product = _shelfModel.Slots[slotIndex].Product;
        if (product == null)
        {
            CurrentShelfView.EmptyFloor(slotIndex);
        }
        else
        {
            var sprite = _spritesProvider.GetProductSprite(product.Config.Key);
            var fullness = _shelfModel.GetFullnessOnFloor(slotIndex);
            CurrentShelfView.SetProductSpriteOnFloor(slotIndex, sprite, fullness);
        }
    }

    private void UpdateFullnessIndicator()
    {
        if (_gameStateModel.GameState == GameStateName.ShopSimulation)
        {
            var totalFullness = 0f;
            var haveEmptySlots = false;
            for (var i = 0; i < _shelfModel.Slots.Length; i++)
            {
                var fullnessOnFloor = _shelfModel.GetFullnessOnFloor(i);
                totalFullness += fullnessOnFloor;
                if (fullnessOnFloor <= 0) haveEmptySlots = true;
            }
            var totalFullnessFactor = totalFullness / _shelfModel.Slots.Length;

            var needToShowFullnessIndicator = false;
            if (totalFullnessFactor <= 0)
            {
                needToShowFullnessIndicator = true;
                _fullnessIndicatorView.color = _fullnessIndicatorView.color.SetRGBFromColor(Color.red);
            }
            else if (haveEmptySlots == true)
            {
                needToShowFullnessIndicator = true;
                _fullnessIndicatorView.color = _fullnessIndicatorView.color.SetRGBFromColor(Color.magenta);
            }
            _fullnessIndicatorView.gameObject.SetActive(needToShowFullnessIndicator);
        }
        else
        {
            _fullnessIndicatorView.gameObject.SetActive(false);
        }
    }
}
