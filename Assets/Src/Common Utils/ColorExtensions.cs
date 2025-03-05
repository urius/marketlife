using UnityEngine;

namespace Src.Common_Utils
{
    public static class ColorExtensions
    {
        public static Color SetAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Color SetRGB(this Color color, float r, float g, float b)
        {
            color.r = r;
            color.g = g;
            color.b = b;
            return color;
        }

        public static Color SetRGBFromColor(this Color color, Color from)
        {
            color.r = from.r;
            color.g = from.g;
            color.b = from.b;
            return color;
        }

        public static Color SetRGBA(this Color color, float r, float g, float b, float alpha)
        {
            color.a = alpha;
            color.r = r;
            color.g = g;
            color.b = b;
            return color;
        }

        public static Color SetR(this Color color, float r)
        {
            color.r = r;
            return color;
        }

        public static Color SetG(this Color color, float g)
        {
            color.g = g;
            return color;
        }

        public static Color SetB(this Color color, float b)
        {
            color.b = b;
            return color;
        }
    }
}
