using System;
using UIKit;

namespace Qoden.UI
{
    public static class RGBExtensions
    {
        public static UIColor ToColor(this RGB bgColor)
        {
            return UIColor.FromRGBA(bgColor.Red / 255f, bgColor.Green / 255f, bgColor.Blue / 255f, bgColor.Alpha / 255f);
        }

        public static RGB ToRGB(this UIColor color)
        {
            nfloat r, g, b, a;
            color.GetRGBA(out r, out g, out b, out a);
            return new RGB((byte)Math.Round(r * 255),
                           (byte)Math.Round(g * 255),
                           (byte)Math.Round(b * 255),
                           (byte)Math.Round(a * 255));
        }
    }
}
