using System;
using Foundation;
using Qoden.UI;
using UIKit;

namespace Example
{
    public partial class Theme
    {
        internal static NSAttributedString HeadlineText(string v)
        {
            var attributedString = new NSMutableAttributedString(v);
            var val = new NSNumber(0.2);
            attributedString.AddAttribute(UIStringAttributeKey.KerningAdjustment, val, new NSRange(0, v.Length));
            return attributedString;
        }

        public static void Tabs(UITabBar view)
        {
            view.Translucent = false;
            view.BarTintColor = RGB.RGBA(0x0A, 0x0A, 0x0A, 0.76f).ToColor();
            view.TintColor = RGB.RGBA(48, 112, 169, 1).ToColor();
        }
    }
}
