using System;
using System.Drawing;
#if __ANDROID__
using Android.Content.Res;
#endif

namespace Qoden.UI
{
    public static class Units
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
        public static readonly IUnit Id = IdentityUnit.Instance;
    }

    public static class UnitsExtensions
    {
        public static float Dp(this float f)
        {
            return Units.Dp.ToPixels(f);
        }
        public static float Dp(this int f)
        {
            return Units.Dp.ToPixels(f);
        }
        public static float Dp(this double f)
        {
            return Units.Dp.ToPixels((float)f);
        }
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
        float ToPixels(float unit);
    }

    public class IdentityUnit : IUnit
    {
        public static readonly IUnit Instance = new IdentityUnit();

        public float ToPixels(float unit)
        {
            return unit;
        }
    }

    public static class UnitExtensions
    {
        public static int ToIntPixels(this IUnit unit, float x)
        {
            return (int)Math.Round(unit.ToPixels(x));
        }

        public static RectangleF ToPixels(this RectangleF rect, IUnit unit)
        {
            return new RectangleF(unit.ToPixels(rect.Left),
                                  unit.ToPixels(rect.Top),
                                  unit.ToPixels(rect.Width),
                                  unit.ToPixels(rect.Height));
        }

        public static SizeF ToPixels(this SizeF rect, IUnit unit)
        {
            return new SizeF(unit.ToPixels(rect.Width), unit.ToPixels(rect.Height));
        }
    }

#if __ANDROID__
    public class Dpi : IUnit
    {
        public float ToPixels(float dp)
        {
            return Dpi.ToPixles(dp);
        }

        public static float ToPixles(float dp)
        {
            var metrics = Resources.System.DisplayMetrics;
            return dp * metrics.Density;
        }
    }
#endif
}
