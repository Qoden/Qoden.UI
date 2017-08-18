using System;
using System.Drawing;
#if __ANDROID__
using Android.Content.Res;
#endif

namespace Qoden.UI
{
    public class Units
    {
#if __IOS__
        public static readonly IUnit Dp = IdentityUnit.Instance;
#elif __ANDROID__
        public static readonly IUnit Dp = new Dpi();
#else
        public static readonly IUnit Dp = IdentityUnit.Instance;
#endif
        public static readonly IUnit PlatformDefault = Dp;
        public static readonly IUnit Px = IdentityUnit.Instance;
    }

    public struct Pixel
    {
        public float Value;

        public Pixel(float value)
        {
            Value = value;
        }

        public static Pixel Val(float v) { return new Pixel(v); }

        public int IntValue => (int)Math.Round(Value);
    }

    public interface IUnit
    {
        Pixel ToPixels(float unit);
    }

    public class IdentityUnit : IUnit
    {
        public static readonly IUnit Instance = new IdentityUnit();

        public Pixel ToPixels(float unit)
        {
            return Pixel.Val(unit);
        }
    }

    public static class UnitExtensions
    {
        public static int ToIntPixels(this IUnit unit, float x)
        {
            return (int)Math.Round(unit.ToPixels(x).Value);
        }

        public static float ToFloatPixels(this IUnit unit, float x)
        {
            return unit.ToPixels(x).Value;
        }

        public static RectangleF ToPixels(this RectangleF rect, IUnit unit)
        {
            return new RectangleF(unit.ToPixels(rect.Left).Value,
                                  unit.ToPixels(rect.Top).Value,
                                  unit.ToPixels(rect.Width).Value,
                                  unit.ToPixels(rect.Height).Value);
        }

        public static SizeF ToPixels(this SizeF rect, IUnit unit)
        {
            return new SizeF(unit.ToPixels(rect.Width).Value, unit.ToPixels(rect.Height).Value);
        }
    }

#if __ANDROID__
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
#endif
}
