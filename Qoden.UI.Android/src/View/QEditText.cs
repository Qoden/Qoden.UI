using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace Qoden.UI
{
    public class QEditText : BaseView<EditText>
    {
        public QEditText()
        {
        }

        public QEditText(EditText target) : base(target)
        {
        }
    }

    public static class EditTextExtensions
    {
        public static void SetFont(this IQView<EditText> view, Font font)
        {
            view.PlatformView.SetFont(font);
        }

        public static void SetFont(this EditText view, Font font)
        {
            view.Typeface = TypefaceCollection.Get(font.Name, font.Style);
            view.TextSize = font.Size;
        }
    }
}
