using UnityEngine;

public class ShelfViewMediator : ShopObjectMediatorBase
{
    private readonly SpritesProvider _spritesProvider;

    private ShelfModel _shelfModel;

    public ShelfViewMediator(Transform parentTransform, ShelfModel shelfModel)
        : base(parentTransform, shelfModel)
    {
        _spritesProvider = SpritesProvider.Instance;

        _shelfModel = shelfModel;
    }

    private ShelfView CurrentShelfView => CurrentView as ShelfView;

    protected override void UpdateView()
    {
        base.UpdateView();

        UpdateProductViews();
    }

    private void UpdateProductViews()
    {
        for (var i = 0; i < _shelfModel.Products.Length; i++)
        {
            var product = _shelfModel.Products[i];
            if (product == null)
            {
                CurrentShelfView.EmptyFloor(i);
            }
            else
            {
                var sprite = _spritesProvider.GetProductSprite(product.Config.Key);
                var fullnes = _shelfModel.GetFullnesOnFloor(i);
                CurrentShelfView.SetProductSpriteOnFloor(i, sprite, fullnes);
            }
        }
    }
}
