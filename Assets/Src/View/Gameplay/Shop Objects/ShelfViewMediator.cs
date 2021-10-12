using UnityEngine;

public class ShelfViewMediator : ShopObjectMediatorBase
{
    private readonly Dispatcher _dispatcher;
    private readonly SpritesProvider _spritesProvider;
    private readonly ScreenCalculator _screenCalculator;

    private SpriteRenderer _fullnessIndicatorView;
    private ShelfModel _shelfModel;

    public ShelfViewMediator(Transform parentTransform, ShelfModel shelfModel)
        : base(parentTransform, shelfModel)
    {
        _dispatcher = Dispatcher.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _screenCalculator = ScreenCalculator.Instance;

        _shelfModel = shelfModel;
    }

    private ShelfView CurrentShelfView => CurrentView as ShelfView;

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

    private void Activate()
    {
        _shelfModel.ProductIsSetOnSlot += OnProductIsSetOnSlot;
        _shelfModel.ProductRemovedFromSlot += OnProductRemovedFromSlot;
        _shelfModel.ProductAmountChangedOnSlot += OnProductAmountChangedOnSlot;
    }

    private void Deactivate()
    {
        _shelfModel.ProductIsSetOnSlot -= OnProductIsSetOnSlot;
        _shelfModel.ProductRemovedFromSlot -= OnProductRemovedFromSlot;
        _shelfModel.ProductAmountChangedOnSlot -= OnProductAmountChangedOnSlot;
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
        var totalFullness = 0f;
        for (var i = 0; i < _shelfModel.Slots.Length; i++)
        {
            totalFullness += _shelfModel.GetFullnessOnFloor(i);
        }
        var totalFullnessFactor = totalFullness / _shelfModel.Slots.Length;

        _fullnessIndicatorView.gameObject.SetActive(totalFullnessFactor <= 0.8f);
        if (totalFullnessFactor > 0.4f)
        {
            _fullnessIndicatorView.color = _fullnessIndicatorView.color.SetRGBFromColor(Color.yellow);
        }
        else if (totalFullnessFactor > 0)
        {
            _fullnessIndicatorView.color = _fullnessIndicatorView.color.SetRGBFromColor(Color.magenta);
        }
        else
        {
            _fullnessIndicatorView.color = _fullnessIndicatorView.color.SetRGBFromColor(Color.red);
        }
    }
}
