using System;
using UnityEngine;

public class ShelfModel : ShopObjectModelBase
{
    public event Action<int, ProductModel> ProductUpdated = delegate { };

    public readonly int PartVolume;
    public readonly int PartsCount;

    public ProductModel[] Products;

    public ShelfModel(int numericId, ShelfConfigDto shelfConfig, Vector2Int coords, int side = 3)
        : base(numericId, shelfConfig, coords, side)
    {
        PartVolume = shelfConfig.part_volume;
        PartsCount = shelfConfig.parts_num;

        Products = new ProductModel[PartsCount];
    }

    public float GetFullnesOnFloor(int floor)
    {
        var productOnFloor = Products[floor];
        if (productOnFloor != null)
        {
            var volume = productOnFloor.Config.Volume * productOnFloor.Amount;
            return (float)volume / PartVolume;
        }

        return 0;
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

    public override ShopObjectModelBase Clone()
    {
        return new ShopObjectModelFactory().CreateShelf(NumericId, Coords, Side, Products);
    }
}
