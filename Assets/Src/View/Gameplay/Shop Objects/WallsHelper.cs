using UnityEngine;

public class WallsHelper
{
    public static void PlaceAsWallLike(Transform transform, Vector2Int cellCoords)
    {
        var gridCalculator = GridCalculator.Instance;
        var irRight = cellCoords.y < cellCoords.x;
        transform.position = irRight ? gridCalculator.GetCellLeftCorner(cellCoords) : gridCalculator.GetCellRightCorner(cellCoords);
        if (irRight)
        {
            ToRightState(transform);
        }
        else
        {
            ToLeftState(transform);
        }
    }

    public static void ToRightState(Transform transform)
    {
        var euler = transform.eulerAngles;
        euler.x = 225;
        euler.y = 270;
        euler.z = 90;
        transform.eulerAngles = euler;
    }

    public static void ToLeftState(Transform transform)
    {
        var euler = transform.eulerAngles;
        euler.x = 135;
        euler.y = 270;
        euler.z = 90;
        transform.eulerAngles = euler;
    }
}
