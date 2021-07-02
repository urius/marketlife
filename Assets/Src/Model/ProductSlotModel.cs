using System;

public class ProductSlotModel
{
    public event Action<int> ProductIsSet = delegate { };
    public event Action<int, ProductModel> ProductRemoved = delegate { };
    public event Action<int, int> ProductAmountChanged = delegate { };
    public event Action<int, int> ProductDeliveryTimeChanged = delegate { };

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
        RemoveProduct();
        if (product == null) return;
        Product = product;
        SubscribeToProduct(Product);
        ProductIsSet(Index);
    }

    public void ChangeProductAmount(int deltaAmount)
    {
        if (HasProduct)
        {
            if (Product.Amount + deltaAmount <= 0)
            {
                RemoveProduct();
            }
            else
            {
                Product.Amount += deltaAmount;
            }
        }
    }

    public bool RemoveProduct()
    {
        if (HasProduct)
        {
            UnsubscribeFromProduct(Product);
            var removedProduct = Product;
            Product = null;
            ProductRemoved(Index, removedProduct);
            return true;
        }
        return false;
    }

    public (int Index, int RemovedAmount) RemoveIfContains(ProductModel[] productModels)
    {
        for (var i = 0; i < productModels.Length; i++)
        {
            var productModel = productModels[i];
            if (productModel.Amount > 0
                && HasProduct
                && Product.Config.NumericId == productModel.Config.NumericId)
            {
                var amountToRemove = Math.Min(Product.Amount, productModel.Amount);
                ChangeProductAmount(-amountToRemove);
                return (i, amountToRemove);
            }
        }
        return (-1, 0);
    }

    public float GetFullness()
    {
        if (HasProduct)
        {
            var maxAmount = Volume / Product.Config.Volume;
            return (float)Product.Amount / maxAmount;
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

    public int GetMaxAmount()
    {
        if (HasProduct)
        {
            return Volume / Product.Config.Volume;
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
        ProductDeliveryTimeChanged(Index, deltaTimeSeconds);
    }
}
