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
    public static class LayoutBuilder_Extensions
    {
        public static PlatformViewLayoutBox View(this LayoutBuilder builder, View v, RectangleF? bounds = null, EdgeInset? padding = null, IUnit units = null)
        {
            return (PlatformViewLayoutBox)builder.View(new QView(v), bounds, padding, units);
        }
    }
}
