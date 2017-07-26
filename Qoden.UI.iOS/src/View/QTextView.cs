using Foundation;
using UIKit;

namespace Qoden.UI
{
    public partial class QTextView : BaseView<UILabel>
    {
    }

    public static partial class TextViewExtensions
    {
        public static void SetTextColor(this UILabel view, RGB color)
        {
            view.TextColor = color.ToColor();
        }

        public static void SetText(this UILabel view, string text)
        {
            view.Text = text;
        }

        public static void SetText(this UILabel view, NSAttributedString text)
        {
            view.AttributedText = text;
        }

        public static void SetText(this IQView<UILabel> view, NSAttributedString text)
        {
            view.PlatformView.SetText(text);
        }

        public static void SetFont(this UILabel view, Font font)
        {
            view.Font = font.ToFont();
        }

        public static void SetEnabled(this UILabel view, bool enabled)
        {
            view.Enabled = enabled;
        }

        public static void SetEnabled(this IQView<UILabel> view, bool enabled)
        {
            view.PlatformView.SetEnabled(enabled);
        }
    }
}
