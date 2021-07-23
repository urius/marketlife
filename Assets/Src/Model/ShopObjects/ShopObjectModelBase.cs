using System;
using UnityEngine;

public abstract class ShopObjectModelBase : PositionableObjectModelBase
{
    public event Action<bool> HighlightStateChanged = delegate { };

    public readonly int NumericId;
    public readonly Price Price;
    public readonly int UnlockLevel;

    public ShopObjectModelBase(int numericId, ShopObjectConfigDto configDto, Vector2Int coords, int side)
        : base(configDto.build_matrix, configDto.two_sides_mode, coords, side)
    {
        NumericId = numericId;

        Price = Price.FromString(configDto.price);
        UnlockLevel = configDto.unlock_level;
    }
    public bool IsHighlighted { get; private set; }

    public abstract ShopObjectType Type { get; }
    public abstract ShopObjectModelBase Clone();

    public void TriggerHighlighted(bool isHighlighted)
    {
        if (isHighlighted == IsHighlighted) return;
        IsHighlighted = isHighlighted;
        HighlightStateChanged(isHighlighted);
    }
}

public enum ShopObjectType
{
    Undefined,
    CashDesk,
    Shelf,
}
