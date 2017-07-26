using Android.Widget;

namespace Qoden.UI
{
    public partial class QTextView : BaseView<TextView>
    {
    }

    public static partial class TextViewExtensions
    {
        public static void SetFont(this TextView view, Font font)
        {
            view.Typeface = TypefaceCollection.Get(font.Name, font.Style);
            view.TextSize = font.Size;
        }

        public static void SetTextColor(this TextView view, RGB color)
        {
            view.SetTextColor(color.ToColor());
        }

        public static void SetText(this TextView view, string text)
        {
            view.Text = text;
        }
    }
}
