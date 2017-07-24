using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Qoden.UI
{
    public class QEditText : BaseView<UITextField>
    {
        public QEditText()
        {
        }

        public QEditText(UITextField target) : base(target)
        {
        }
    }

    public static class EditTextExtensions
    {
        public static void SetTextColor(this UITextField field, RGB color)
        {
            field.TextColor = color.ToColor();
        }

        public static void SetTextColor(this IQView<UITextField> field, RGB color)
        {
            field.PlatformView.SetTextColor(color);
        }
    }
}
