using System;
using System.Collections.Generic;
using UnityEngine;

namespace Src.Model.Common
{
    public static class MatrixExtensions
    {
        public static int[][] Rotate(this int[][] originalMarix, int deltaSide)
        {
            if (deltaSide == 0 || (originalMarix.Length == 1 && originalMarix[0].Length == 1))
            {
                return originalMarix;
            }

            var deltaSideRest = deltaSide;//todo: clamp by %4
            var tempMtx = originalMarix;
            var deltaSideSubstractValue = deltaSide > 0 ? 1 : -1;
            while (deltaSideRest != 0)
            {
                tempMtx = deltaSide > 0 ? RotateRight(tempMtx) : RotateLeft(tempMtx);
                deltaSideRest -= deltaSideSubstractValue;
            }

            return tempMtx;
        }

        public static IList<T> FlatMap<T>(this int[][] originalMarix, Func<Vector2Int, int, int, T> mapFunc)
        {
            var result = new T[originalMarix[0].Length * originalMarix.Length];
            var flatIndex = 0;
            var width = originalMarix[0].Length;
            var pivot = new Vector2Int(width / 2, originalMarix.Length / 2);
            for (var y = 0; y < originalMarix.Length; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    result[flatIndex] = mapFunc(new Vector2Int(x, y) - pivot, flatIndex, originalMarix[y][x]);
                    flatIndex++;
                }
            }

            return result;
        }

        public static void ForEachElement(this int[][] originalMarix, Action<Vector2Int, int, int> callback)
        {
            var flatIndex = 0;
            var width = originalMarix[0].Length;
            var pivot = new Vector2Int(width / 2, originalMarix.Length / 2);
            for (var y = 0; y < originalMarix.Length; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    callback(new Vector2Int(x, y) - pivot, flatIndex, originalMarix[y][x]);
                    flatIndex++;
                }
            }
        }

        private static int[][] RotateRight(int[][] originalMarix)
        {
            var result = GetZeroMatrix(originalMarix[0].Length, originalMarix.Length);

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
            var result = GetZeroMatrix(originalMarix[0].Length, originalMarix.Length);

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
}
