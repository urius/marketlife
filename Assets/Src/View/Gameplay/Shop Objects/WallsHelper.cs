using UnityEngine;

public class WallsHelper
{
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
