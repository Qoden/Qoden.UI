using System;
namespace Qoden.UI
{
#if __IOS__
    using Button = UIKit.UIButton;
#endif
#if __ANDROID__
    using Button = Android.Widget.Button;
#endif
    public partial class QButton
    {
        public QButton()
        {
        }

        public QButton(Button target) : base(target)
        {
        }
    }

    public static partial class QButtonExtensions
    {
        public static void SetText(this IQView<Button> view, string text)
        {
            view.PlatformView.SetText(text);
        }

        public static void SetTextColor(this IQView<Button> view, RGB color)
        {
            view.PlatformView.SetTextColor(color);
        }

        public static void SetFont(this IQView<Button> view, Font font)
        {
            view.PlatformView.SetFont(font);
        }
    }
}
