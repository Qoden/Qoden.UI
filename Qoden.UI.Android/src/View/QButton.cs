using Android.Widget;

namespace Qoden.UI
{
    public partial class QButton : BaseView<Button>
    {
    }

    public static partial class QButtonExtensions
    {
        public static void SetText(this Button view, string text)
        {
            view.Text = text;
        }

        public static void SetFont(this Button view, Font font)
        {
            view.Typeface = TypefaceCollection.Get(font.Name, font.Style);
            view.TextSize = font.Size;
        }

        public static void SetTextColor(this Button view, RGB color)
        {
            view.SetTextColor(color.ToColor());
        }
    }
}
