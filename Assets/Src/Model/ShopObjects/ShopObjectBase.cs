using System;
using UnityEngine;

public abstract class ShopObjectBase
{
    public event Action<int, int> SideChanged = delegate { };

    public readonly int Level;

    public Vector2Int Coords;

    private int _side;

    public ShopObjectBase(Vector2Int coords, int level, int angle)
    {
        Level = level;

        _side = SideHelper.GetSideFromAngle(angle);

        Coords = coords;
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

    public abstract ShopObjectType Type { get; }
}

public enum ShopObjectType
{
    Undefined,
    CashDesk,
    Shelf,
}
