using System;
using UnityEngine;

public abstract class ShopObjectBase
{
    public event Action<int, int> SideChanged = delegate { };
    public event Action<Vector2Int, Vector2Int> CoordsChanged = delegate { };

    public readonly int NumericId;
    public readonly Price Price;
    public readonly int UnlockLevel;
    public readonly bool TwoSidesMode;

    private Vector2Int _coords;
    private int _side = -1;
    private readonly int[][] _defaultBuildMatrix;

    public ShopObjectBase(int numericId, ShopObjectConfigDto config, Vector2Int coords, int side)
    {
        NumericId = numericId;

        Price = Price.FromString(config.price);
        _defaultBuildMatrix = config.build_matrix;
        UnlockLevel = config.unlock_level;
        TwoSidesMode = config.two_sides_mode;

        Side = side;
        Coords = coords;
    }

    public int Angle => SideHelper.ConvertSideToAngle(_side);
    public int Side
    {
        get { return _side; }
        set
        {
            if (value == _side) return;
            var sideBefore = _side;
            _side = SideHelper.ClampSide(value, TwoSidesMode);
            RotatedBuildMatrix = _defaultBuildMatrix.Rotate(_side - 3);
            SideChanged(sideBefore, _side);
        }
    }
    public Vector2Int Coords
    {
        get { return _coords; }
        set
        {
            if (value == _coords) return;
            var coordsBefore = _coords;
            _coords = value;
            CoordsChanged(coordsBefore, value);
        }
    }
    public int[][] RotatedBuildMatrix { get; private set; }

    public abstract ShopObjectType Type { get; }
    public abstract ShopObjectBase Clone();
}

public enum ShopObjectType
{
    Undefined,
    CashDesk,
    Shelf,
}
