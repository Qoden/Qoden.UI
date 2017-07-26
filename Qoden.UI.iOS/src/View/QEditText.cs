using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Qoden.UI
{
    public partial class QEditText : QControl<UITextField>
    {
    }

    public static partial class QEditTextExtensions
    {
        public static void SetTextColor(this UITextField field, RGB color)
        {
            field.TextColor = color.ToColor();
        }

        public static void SetHintText(this UITextField field, string text)
        {
            field.Placeholder = text;
        }

        public static void SetHintText(this UITextField field, NSAttributedString text)
        {
            field.AttributedPlaceholder = text;
        }

        public static void SetHintText(this IQView<UITextField> field, NSAttributedString text)
        {
            field.PlatformView.SetHintText(text);
        }

        public static void SetFont(this UITextField view, Font font)
        {
            view.Font = font.ToFont();
        }

        public static void SetText(this UITextField view, string text)
        {
            view.Text = text;
        }

        public static void SetText(this UITextField view, NSAttributedString text)
        {
            view.AttributedText = text;
        }

        public static void SetText(this IQView<UITextField> field, NSAttributedString text)
        {
            field.PlatformView.SetText(text);
        }
    }
}
