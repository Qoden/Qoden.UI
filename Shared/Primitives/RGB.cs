using System;

namespace Qoden.UI
{
    public struct RGB
    {
        public RGB(byte red, byte green, byte blue) : this()
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = 0xff;
        }

        public RGB(byte red, byte green, byte blue, byte alpha) : this()
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public RGB(byte red, byte green, byte blue, float alpha) : this()
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = (byte)Math.Round(255 * alpha);
        }

        public static RGB RGBA(byte red, byte green, byte blue, float alpha)
        {
            return new RGB(red, green, blue, (byte)Math.Round(255 * alpha));
        }

        public byte Red { get; private set; }

        public byte Green { get; private set; }

        public byte Blue { get; private set; }

        public byte Alpha { get; private set; }

        public int IntARGB { get => 0xFF << 24 | Red << 16 | Green << 8 | Blue; }

        public static readonly RGB Clear = new RGB(0, 0, 0, 0);

        public RGB WithAlpha(float newAlpha) => new RGB(Red, Green, Blue, (byte) Math.Round(255 * newAlpha));
    }


    public static class RGBExtensions
    {
#if __IOS__
        public static UIKit.UIColor ToColor(this RGB bgColor)
        {
            return UIKit.UIColor.FromRGBA(bgColor.Red / 255f, bgColor.Green / 255f, bgColor.Blue / 255f, bgColor.Alpha / 255f);
        }

        public static RGB ToRGB(this UIKit.UIColor color)
        {
            nfloat r, g, b, a;
            color.GetRGBA(out r, out g, out b, out a);
            return new RGB((byte)Math.Round(r * 255),
                           (byte)Math.Round(g * 255),
                           (byte)Math.Round(b * 255),
                           (byte)Math.Round(a * 255));
        }
#endif

#if __ANDROID__
        public static Android.Graphics.Color ToColor(this RGB bgColor)
        {
            return Android.Graphics.Color.Argb(bgColor.Alpha, bgColor.Red, bgColor.Green, bgColor.Blue);
        }

        public static RGB ToRGB(this Android.Graphics.Color bgColor)
        {
            return RGB.RGBA(bgColor.R, bgColor.G, bgColor.B, bgColor.A);
        }
#endif
    }
}