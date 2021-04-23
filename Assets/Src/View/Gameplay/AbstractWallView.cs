using UnityEngine;

public class AbstractWallView : MonoBehaviour
{
    public void ToRightState()
    {
        var euler = transform.eulerAngles;
        euler.x = 225;
        transform.eulerAngles = euler;
    }

    public void ToLeftState()
    {
        var euler = transform.eulerAngles;
        euler.x = 135;
        transform.eulerAngles = euler;
    }
}
