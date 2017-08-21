using System;
using System.Drawing;
#if __IOS__
using UIKit;
#endif

namespace Qoden.UI
{
    public struct EdgeInsets
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public EdgeInsets(float left = 0, float top = 0, float right = 0, float bottom = 0, IUnit unit = null)
        {
            unit = unit ?? Units.PlatformDefault;

            Left = unit.ToPixels(left).Value;
            Top = unit.ToPixels(top).Value;
            Right = unit.ToPixels(right).Value;
            Bottom = unit.ToPixels(bottom).Value;
        }

        public static readonly EdgeInsets Zero = new EdgeInsets(0, 0, 0, 0);

        public RectangleF Substract(RectangleF rect)
        {
            return new RectangleF(rect.Left - Left, rect.Top - Top, rect.Width - Left - Right, rect.Height - Top - Bottom);
        }
    }

    public static class PaddingExtensions
    {
#if __IOS__
        public static EdgeInsets ToEdgeInsets(this UIEdgeInsets p)
        {
            return new EdgeInsets((float)p.Top, (float)p.Left, (float)p.Bottom, (float)p.Right);
        }

        public static UIEdgeInsets ToUIEdgeInsets(this EdgeInsets p)
        {
            return new UIEdgeInsets(p.Top, p.Left, p.Bottom, p.Right);
        }
#endif
    }
}

