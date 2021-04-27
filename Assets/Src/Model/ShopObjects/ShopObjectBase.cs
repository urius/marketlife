using System;
using UnityEngine;

public abstract class ShopObjectBase
{
    public event Action<int, int> SideChanged = delegate { };

    public readonly int Level;
    public readonly Price Price;
    public readonly int UnlockLevel;
    public readonly bool TwoSidesMode;

    public Vector2Int Coords;

    private int _side;

    public ShopObjectBase(int level, ShopObjectConfigDto config, Vector2Int coords, int side)
    {
        Coords = coords;
        Level = level;
        _side = side;

        Price = Price.FromString(config.price);
        BuildMatrix = config.build_matrix;
        UnlockLevel = config.unlock_level;
        TwoSidesMode = config.two_sides_mode;
    }

    public int Angle => SideHelper.ConvertSideToAngle(_side);
    public int Side
    {
        get { return _side; }
        set
        {
            var sideBefore = _side;
            _side = value; SideChanged(sideBefore, value);
        }
    }

    public int[][] BuildMatrix { get; private set; }

    public abstract ShopObjectType Type { get; }
}

public enum ShopObjectType
{
    Undefined,
    CashDesk,
    Shelf,
}
