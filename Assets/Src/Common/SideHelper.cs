using System;

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
}
