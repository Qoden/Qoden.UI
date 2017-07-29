using Android.Widget;

namespace Qoden.UI
{
    public partial class QEditText : BaseView<EditText>
    {
    }

    public static partial class QEditTextExtensions
    {
        public static void SetFont(this EditText view, Font font)
        {
            view.Typeface = TypefaceCollection.Get(font.Name, font.Style);
            view.TextSize = font.Size;
        }

        public static void SetHintText(this EditText field, string text)
        {
            field.Hint = text;
        }

        public static void SetTextColor(this EditText field, RGB color)
        {
            field.SetTextColor(color.ToColor());
        }

        public static string GetText(this EditText field)
        {
            return field.Text;
        }
    }
}
