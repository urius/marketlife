using System;

public class ProductModel
{
    public event Action<int> AmountChanged = delegate { };
    public event Action<int> DeliveryTimeChanged = delegate { };

    public readonly ProductConfig Config;

    private int _amount = 1;
    private int _deliverTime = 0;

    public ProductModel(ProductConfig config)
    {
        Config = config;
    }

    public ProductModel(ProductConfig config, int amount) : this(config)
    {
        _amount = amount;
    }

    public ProductModel(ProductConfig config, int amount, int deliverTime) : this(config, amount)
    {
        _deliverTime = deliverTime;
    }

    public int NumericId => Config.NumericId;
    public int Amount
    {
        get => _amount;
        set
        {
            var previousValue = _amount;
            _amount = value;
            AmountChanged(value - previousValue);
        }
    }

    public int DeliverTime
    {
        get => _deliverTime;
        set
        {
            var previousValue = _deliverTime;
            _deliverTime = value;
            DeliveryTimeChanged(value - previousValue);
        }
    }
}
