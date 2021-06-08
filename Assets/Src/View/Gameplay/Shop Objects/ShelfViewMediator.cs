using UnityEngine;

public class ShelfViewMediator : ShopObjectMediatorBase
{
    private readonly Dispatcher _dispatcher;
    private readonly SpritesProvider _spritesProvider;
    private readonly ScreenCalculator _screenCalculator;

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

        UpdateProductViews();
    }

    public override void Mediate()
    {
        base.Mediate();

        Activate();
    }

    public override void Unmediate()
    {
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

    private void OnProductIsSetOnSlot(int slotIndex)
    {
        UpdateProductViewOnSlot(slotIndex);
        var product = _shelfModel.Slots[slotIndex].Product;
        RequestFlyingProduct(product.Config.Key, product.Amount);
    }

    private void OnProductRemovedFromSlot(int slotIndex, ProductModel removedProduct)
    {
        UpdateProductViewOnSlot(slotIndex);
        RequestFlyingProduct(removedProduct.Config.Key, -removedProduct.Amount);
    }

    private void OnProductAmountChangedOnSlot(int slotIndex, int deltaAmount)
    {
        UpdateProductViewOnSlot(slotIndex);
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
}
