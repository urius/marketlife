using System;
using System.Linq;
using UnityEngine;

public class ShelfModel : ShopObjectModelBase
{
    public event Action<int> ProductIsSetOnSlot = delegate { };
    public event Action<int> ProductRemovedFromSlot = delegate { };
    public event Action<int, int> ProductAmountChangedOnSlot = delegate { };

    public readonly int PartVolume;
    public readonly int PartsCount;
    public readonly ProductSlotModel[] Slots;

    public ShelfModel(int numericId, ShelfConfigDto shelfConfig, Vector2Int coords, int side = 3)
        : base(numericId, shelfConfig, coords, side)
    {
        PartVolume = shelfConfig.part_volume;
        PartsCount = shelfConfig.parts_num;

        Slots = new ProductSlotModel[PartsCount];

        for (var i = 0; i < Slots.Length; i++)
        {
            var slot = new ProductSlotModel(i, PartVolume);
            slot.ProductIsSet += OnSlotProductSet;
            slot.ProductRemoved += OnSlotProductRemoved;
            slot.ProductAmountChanged += OnSlotProductAmountChanged;
            Slots[i] = slot;
        }
    }

    private void OnSlotProductSet(int slotIndex)
    {
        ProductIsSetOnSlot(slotIndex);
    }

    private void OnSlotProductRemoved(int slotIndex)
    {
        ProductRemovedFromSlot(slotIndex);
    }

    private void OnSlotProductAmountChanged(int slotIndex, int amountDelta)
    {
        ProductAmountChangedOnSlot(slotIndex, amountDelta);
    }

    public float GetFullnesOnFloor(int partIndex)
    {
        return Slots[partIndex].GetFullnes();
    }

    public bool RemoveProductAt(int partIndex)
    {
        if (partIndex < Slots.Length)
        {
            return Slots[partIndex].RemoveProduct();
        }

        return false;
    }

    public bool TrySetProductOn(int partIndex, ProductModel productModel)
    {
        if (partIndex < Slots.Length)
        {
            Slots[partIndex].SetProduct(productModel);
            return true;
        }

        return false;
    }

    public bool TryGetProductAt(int partIndex, out ProductModel productModel)
    {
        productModel = null;
        if (partIndex < Slots.Length
           && Slots[partIndex].HasProduct)
        {
            productModel = Slots[partIndex].Product;
            return true;
        }

        return false;
    }

    public int GetRestAmountOn(int partIndex)
    {
        if (partIndex < Slots.Length)
        {
            Slots[partIndex].GetRestAmount();
        }
        return -1;
    }

    public override ShopObjectType Type => ShopObjectType.Shelf;

    public override ShopObjectModelBase Clone()
    {
        var products = Slots.Select(s => s.Product).ToArray();
        return new ShopObjectModelFactory().CreateShelf(NumericId, Coords, Side, products);
    }
}

