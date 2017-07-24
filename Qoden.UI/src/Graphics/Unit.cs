using System;
using System.Drawing;

namespace Qoden.UI
{
    public struct Pixel
    {
        public float Value;

        public Pixel(float value)
        {
            Value = value;
        }

        public static Pixel Val(float v) { return new Pixel(v); }
    }

    public interface IUnit
    {
        Pixel ToPixels(float unit);
    }

    public class IdentityUnit : IUnit
    {
        public static readonly IUnit Identity = new IdentityUnit();

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
}
