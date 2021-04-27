using System;
using UnityEngine;

public class ShelfModel : ShopObjectBase
{
    public event Action<int, ProductModel> ProductUpdated = delegate { };

    public readonly int PartVolume;
    public readonly int PartsCount;

    public ProductModel[] Products;

    public ShelfModel(int level, ShelfConfigDto shelfConfig, Vector2Int coords, int side = 3)
        : base(level, shelfConfig, coords, side)
    {
        PartVolume = shelfConfig.part_volume;
        PartsCount = shelfConfig.parts_num;

        Products = new ProductModel[PartsCount];
    }

    public bool TrySetProduct(int partIndex, ProductModel productModel)
    {
        if (partIndex < Products.Length)
        {
            Products[partIndex] = productModel;

            ProductUpdated(partIndex, productModel);
            return true;
        }

        return false;
    }

    public override ShopObjectType Type => ShopObjectType.Shelf;
}
