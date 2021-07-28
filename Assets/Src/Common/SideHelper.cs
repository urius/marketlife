using System;
using UnityEngine;

public class SideHelper
{
    public static bool IsProfileSide(int side)
    {
        switch (side)
        {
            case 1:
            case 4:
                return true;
        }
        return false;
    }

    public static bool IsFlippedSide(int side)
    {
        switch (side)
        {
            case 1:
            case 2:
                return true;
        }
        return false;
    }

    public static int GetScaleXMultiplier(int side)
    {
        return IsFlippedSide(side) ? -1 : 1;
    }

    public static int GetSideFromAngle(int angle)
    {
        return angle switch
        {
            45 => 1,
            135 => 2,
            225 => 3,
            315 => 4,
            _ => 3,
        };
    }

    public static int ConvertSideToAngle(int side)
    {
        return side switch
        {
            1 => 45,
            2 => 135,
            3 => 225,
            4 => 315,
            _ => 225,
        };
    }

    public static int ClampSide(int side, bool twoSidesMode = false)
    {
        var result = side;
        if (result > 4)
        {
            result %= 4;
        }
        if (side <= 0)
        {
            result = result % 4 + 4;
        }

        if (twoSidesMode)
        {
            switch (result)
            {
                case 1:
                    result = 3;
                    break;
                case 4:
                    result = 2;
                    break;
            }
        }

        return result;
    }

    public static Vector2Int SideToVector(int side)
    {
        return side switch
        {
            1 => new Vector2Int(0, -1),
            2 => new Vector2Int(1, 0),
            3 => new Vector2Int(0, 1),
            4 => new Vector2Int(-1, 0),
            _ => Vector2Int.zero,
        };
    }

    public static int VectorToSide(Vector2Int vector)
    {
        if (vector.y < 0)
        {
            return 1;
        }
        if (vector.x > 0)
        {
            return 2;
        }
        if (vector.y > 0)
        {
            return 3;
        }
        if (vector.x < 0)
        {
            return 4;
        }

        return 1;
    }
}
