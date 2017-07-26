using System;
using Foundation;
using Qoden.UI;
using UIKit;

namespace Example
{
    public partial class Theme
    {
        public static NSAttributedString HeadlineText(string v)
        {
            var attributedString = new NSMutableAttributedString(v);
            var val = new NSNumber(0.2);
            attributedString.AddAttribute(UIStringAttributeKey.KerningAdjustment, val, new NSRange(0, v.Length));
            return attributedString;
        }

        public static NSAttributedString TableTextFieldPlaceholderText(string v)
        {
            var attributedString = new NSMutableAttributedString(v);
            attributedString.AddAttribute(UIStringAttributeKey.ForegroundColor, Colors.TableFieldHintText.ToColor(), new NSRange(0, v.Length));
            return attributedString;
        }

        public static void Tabs(UITabBar view)
        {
            view.Translucent = false;
            view.BarTintColor = RGB.RGBA(0x0A, 0x0A, 0x0A, 0.76f).ToColor();
            view.TintColor = RGB.RGBA(48, 112, 169, 1).ToColor();
        }

        public static void FormList(QGroupedListView table)
        {
            table.SetBackgroundColor(Theme.Colors.BG);
            table.PlatformView.SeparatorStyle = UIKit.UITableViewCellSeparatorStyle.SingleLine;
            table.PlatformView.SeparatorColor = RGB.RGBA(74, 82, 89, 0.2f).ToColor();
            table.PlatformView.AllowsSelection = false;
        }

        private static void TextFieldDefaults(QEditText text)
        {
            text.PlatformView.KeyboardAppearance = UIKeyboardAppearance.Dark;
        }

    }
}
