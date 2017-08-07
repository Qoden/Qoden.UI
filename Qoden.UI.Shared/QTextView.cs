using System;
namespace Qoden.UI
{
#if __IOS__
    using TextView = UIKit.UILabel;
#endif
#if __ANDROID__
    using TextView = Android.Widget.TextView;
#endif

    public partial class QTextView
    {
        public QTextView()
        {
        }

        public QTextView(TextView target) : base(target)
        {
        }
    }

    public enum TextAlignment
    {
        Left, Center, Right
    }

    public static partial class TextViewExtensions
    {
        public static void SetFont(this IQView<TextView> view, Font font)
        {
            view.PlatformView.SetFont(font);
        }

        public static void SetTextColor(this IQView<TextView> view, RGB color)
        {
            view.PlatformView.SetTextColor(color);
        }

        public static void SetText(this IQView<TextView> view, string text)
        {
            view.PlatformView.SetText(text);
        }

        public static string GetText(this IQView<TextView> view)
        {
            return view.PlatformView.GetText();
        }

        public static void SetTextAlignment(this IQView<TextView> view, TextAlignment alignment)
        {
            view.PlatformView.SetTextAlignment(alignment);
        }
    }
}
