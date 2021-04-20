using UnityEngine;

public class ShelfModel : ShopObjectBase
{
    public readonly int PartVolume;
    public readonly int PartsCount;

    public ProductModel[] Products;

    public ShelfModel(ShelfConfigDto shelfConfig, Vector2Int coords, int level, int angle, ProductModel[] products)
        : base(coords, level, angle, shelfConfig)
    {
        Products = products;

        PartVolume = shelfConfig.part_volume;
        PartsCount = shelfConfig.parts_num;
    }

    public override ShopObjectType Type => ShopObjectType.Shelf;
}
