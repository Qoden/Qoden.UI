using System;
using Android.Content.Res;

namespace Qoden.UI
{
    public class Units
    {
        public static readonly IUnit Dp = new Dpi();
        public static readonly IUnit Px = IdentityUnit.Identity;
    }

    public class Dpi : IUnit
    {
        Pixel IUnit.ToPixels(float dp)
        {
            return Pixel.Val(ToPixles(dp));
        }

        public static float ToPixles(float dp)
        {
            var metrics = Resources.System.DisplayMetrics;
            return dp * metrics.Density;
        }
    }
}
