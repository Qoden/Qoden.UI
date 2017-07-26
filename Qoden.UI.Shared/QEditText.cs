using System;
namespace Qoden.UI
{
#if __IOS__
    using EditText = UIKit.UITextField;
#endif
#if __ANDROID__
    using EditText = Android.Widget.EditText;
#endif
    public partial class QEditText
    {
        public QEditText()
        {
        }

        public QEditText(EditText target) : base(target)
        {
        }
    }

    public static partial class QEditTextExtensions
    {
        public static void SetFont(this IQView<EditText> view, Font font)
        {
            view.PlatformView.SetFont(font);
        }

        public static void SetHintText(this IQView<EditText> field, string text)
        {
            field.PlatformView.SetHintText(text);
        }

        public static void SetTextColor(this IQView<EditText> field, RGB color)
        {
            field.PlatformView.SetTextColor(color);
        }

        public static void SetText(this IQView<EditText> view, string text)
        {
            view.PlatformView.SetText(text);
        }
    }
}
