using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductSlotModel
{
    public event Action<int> ProductIsSet = delegate { };
    public event Action<int> ProductRemoved = delegate { };
    public event Action<int, int> ProductAmountChanged = delegate { };
    public event Action<int> ProductDeliveryTimeChanged = delegate { };

    public readonly int Index;

    public ProductSlotModel(int index, int volume)
    {
        Index = index;
        Volume = volume;
    }

    public ProductModel Product { get; private set; }
    public bool HasProduct => Product != null;
    public int Volume { get; private set; }

    public void AddVolume(int addedVolume)
    {
        Volume += addedVolume;
    }

    public void SetProduct(ProductModel product)
    {
        Product = product;
        SubscribeToProduct(Product);
        ProductIsSet(Index);
    }

    public bool RemoveProduct()
    {
        if (HasProduct)
        {
            UnsubscribeFromProduct(Product);
            Product = null;
            ProductRemoved(Index);
            return true;
        }
        return false;
    }

    public float GetFullnes()
    {
        if (HasProduct)
        {
            var productsTotalVolume = Product.Config.Volume * Product.Amount;
            return (float)productsTotalVolume / Volume;
        }

        return 0;
    }

    public int GetRestAmount()
    {
        if (HasProduct)
        {
            var maxAmount = Volume / Product.Config.Volume;
            return Math.Max(maxAmount - Product.Amount, 0);
        }

        return -1;
    }

    private void SubscribeToProduct(ProductModel productModel)
    {
        productModel.AmountChanged += OnProductAmountChanged;
        productModel.DeliveryTimeChanged += OnProductDeliveryTimeChanged;
    }

    private void UnsubscribeFromProduct(ProductModel productModel)
    {
        productModel.AmountChanged -= OnProductAmountChanged;
        productModel.DeliveryTimeChanged -= OnProductDeliveryTimeChanged;
    }

    private void OnProductAmountChanged(int deltaAmount)
    {
        if (Product.Amount <= 0)
        {
            RemoveProduct();
        }
        else
        {
            ProductAmountChanged(Index, deltaAmount);
        }
    }

    private void OnProductDeliveryTimeChanged(int deltaTimeSeconds)
    {
        ProductDeliveryTimeChanged(deltaTimeSeconds);
    }
}
