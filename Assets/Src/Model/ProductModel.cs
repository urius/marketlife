using System;

public class ProductModel
{
    public event Action<int, int> AmountChanged = delegate { };
    public event Action<int, int> DeliveryTimeChanged = delegate { };

    public readonly int NumericId;

    private int _amount = 1;
    private int _deliverTime = 0;

    public ProductModel(int numericId)
    {
        NumericId = numericId;
    }

    public ProductModel(int numericId, int amount) : this(numericId)
    {
        _amount = amount;
    }

    public ProductModel(int numericId, int amount, int deliverTime) : this(numericId, amount)
    {
        _deliverTime = deliverTime;
    }

    public int Amount
    {
        get => _amount;
        set
        {
            var previousValue = _amount;
            _amount = value;
            AmountChanged(previousValue, value);
        }
    }

    public int DeliverTime
    {
        get => _deliverTime;
        set
        {
            var previousValue = _deliverTime;
            _deliverTime = value;
            DeliveryTimeChanged(previousValue, value);
        }
    }
}
