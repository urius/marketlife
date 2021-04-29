using System;

public class BuildHelper
{
    public static bool CanBuild(int buildValue1, int buildValue2)
    {
        return !((buildValue1 == buildValue2 && buildValue1 == 1)
            || (buildValue1 == -buildValue2 && Math.Abs(buildValue1) == 1));
    }
}
