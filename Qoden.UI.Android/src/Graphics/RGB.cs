using System;
using Android.Graphics;

namespace Qoden.UI
{
    public static class RGBExtensions
    {
        public static Color ToColor(this RGB bgColor)
        {
            return Color.Argb(bgColor.Alpha, bgColor.Red, bgColor.Green, bgColor.Blue);
        }

        public static RGB ToRGB(this Color bgColor)
        {
            return RGB.RGBA(bgColor.R, bgColor.G, bgColor.B, bgColor.A);
        }
    }
}
