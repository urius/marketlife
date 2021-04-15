using UnityEngine;

public class ShelfModel : ShopObjectBase
{
    public ProductModel[] Products;

    public ShelfModel(Vector2Int coords, int level, int angle, ProductModel[] products)
        : base(coords, level, angle)
    {
        Products = products;
    }

    public override ShopObjectType Type => ShopObjectType.Shelf;
}
