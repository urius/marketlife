using System;
using UnityEngine;

namespace Src.Common_Utils
{
    public static class Vector2Extensions
    {
        public static bool IsInsidePolygon(this Vector2 p, Vector2[] polygon)
        {
            double minX = polygon[0].x;
            double maxX = polygon[0].x;
            double minY = polygon[0].y;
            double maxY = polygon[0].y;
            for (int i = 1; i < polygon.Length; i++)
            {
                Vector3 q = polygon[i];
                minX = Math.Min(q.x, minX);
                maxX = Math.Max(q.x, maxX);
                minY = Math.Min(q.y, minY);
                maxY = Math.Max(q.y, maxY);
            }

            if (p.x < minX || p.x > maxX || p.y < minY || p.y > maxY)
            {
                return false;
            }

            // https://wrf.ecse.rpi.edu/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((polygon[i].y > p.y) != (polygon[j].y > p.y) &&
                    p.x < (polygon[j].x - polygon[i].x) * (p.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x)
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}
