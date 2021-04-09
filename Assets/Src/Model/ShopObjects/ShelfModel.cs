public class ShelfModel : ShopObjectBase
{
    public ProductModel[] Products;

    public ShelfModel(int level, int angle, ProductModel[] products)
        : base(level, angle)
    {
        Products = products;
    }

    public override ShopObjectType Type => ShopObjectType.Shelf;
}
