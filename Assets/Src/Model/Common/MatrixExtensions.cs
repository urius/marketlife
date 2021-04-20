public static class MatrixExtensions
{
    public static int[][] Rotate(this int[][] originalMarix, int deltaSide)
    {
        if (deltaSide == 0)
        {
            return originalMarix;
        }

        var deltaSideRest = deltaSide;
        var tempMtx = originalMarix;
        var deltaSideSubstractValue = deltaSide > 0 ? 1 : -1;
        while (deltaSideRest != 0)
        {
            tempMtx = deltaSide > 0 ? RotateRight(tempMtx) : RotateLeft(tempMtx);
            deltaSideRest -= deltaSideSubstractValue;
        }

        return tempMtx;
    }

    private static int[][] RotateRight(int[][] originalMarix)
    {
        var result = GetZeroMatrix(originalMarix.Length, originalMarix[0].Length);

        var width = result[0].Length;
        var maxX = width - 1;
        for (var y = 0; y < result.Length; y++)
        {
            for (var x = 0; x < width; x++)
            {
                result[y][x] = originalMarix[maxX - x][y];
            }
        }

        return result;
    }

    private static int[][] RotateLeft(int[][] originalMarix)
    {
        var result = GetZeroMatrix(originalMarix.Length, originalMarix[0].Length);

        var width = result[0].Length;
        var maxY = result.Length - 1;
        for (var y = 0; y < result.Length; y++)
        {
            for (var x = 0; x < width; x++)
            {
                result[y][x] = originalMarix[x][maxY - y];
            }
        }

        return result;
    }

    private static int[][] GetZeroMatrix(int lengthX, int lengthY)
    {
        var result = new int[lengthY][];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = new int[lengthX];
        }

        return result;
    }
}
