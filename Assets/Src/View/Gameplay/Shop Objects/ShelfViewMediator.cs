using UnityEngine;

public class ShelfViewMediator : ShopObjectMediatorBase
{
    private readonly SpritesProvider _spritesProvider;

    private ShelfModel _model;

    public ShelfViewMediator(Transform parentTransform, ShelfModel shelfModel)
        : base(parentTransform, shelfModel)
    {
        _spritesProvider = SpritesProvider.Instance;

        _model = shelfModel;
    }

    private ShelfView CurrentShelfView => CurrentView as ShelfView;

    protected override void UpdateView()
    {
        base.UpdateView();

        for (var i = 0; i < _model.Products.Length; i++)
        {
            var product = _model.Products[i];
            if (product == null) continue;
            var sprite = _spritesProvider.GetProductSprite(product.Config.Key);
            var fullnes = _model.GetFullnesOnFloor(i);
            CurrentShelfView.SetProductSpriteOnFloor(i, sprite, fullnes);
        }
    }
}
