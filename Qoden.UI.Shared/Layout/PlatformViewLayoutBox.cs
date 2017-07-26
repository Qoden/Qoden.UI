using System;
using System.Drawing;

namespace Qoden.UI
{
#if __IOS__
    using View = UIKit.UIView;
#endif
#if __ANDROID__
    using View = Android.Views.View;
#endif

    public static partial class PlatformViewLayoutBox_Extensions
    {
        public static PlatformViewLayoutBox LayoutInBounds(this View view, RectangleF r, IUnit unit = null)
        {
            return new PlatformViewLayoutBox(new QView(view), r, unit ?? Units.Dp);
        }

        public static PlatformViewLayoutBox LayoutBox(this IPlatformView view, IUnit unit = null)
        {
            var rect = new RectangleF(0, 0, float.MaxValue, float.MaxValue);
            return new PlatformViewLayoutBox(view, rect, unit ?? Units.Dp); 
        }

        public static PlatformViewLayoutBox LayoutBox(this View view, IUnit unit = null)
        {
            return new PlatformViewLayoutBox(new QView(view), new RectangleF(0, 0, float.MaxValue, float.MaxValue), unit ?? Units.Dp);
        }
    }
}
