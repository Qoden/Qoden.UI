using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace Qoden.UI
{
    public class QTextView : BaseView<TextView>
    {
        public QTextView()
        {
        }

        public QTextView(TextView target) : base(target)
        {
        }
    }

    public static class TextViewExtensions
    {
        public static void SetFont(this IQView<TextView> view, Font font)
        {
            view.PlatformView.SetFont(font);
        }

        public static void SetFont(this TextView view, Font font)
        {
            view.Typeface = TypefaceCollection.Get(font.Name, font.Style);
            view.TextSize = font.Size;
        }

        public static void SetTextColor(this IQView<TextView> view, RGB color)
        {
            view.PlatformView.SetTextColor(color);
        }

        public static void SetTextColor(this TextView view, RGB color)
        {
            view.SetTextColor(color.ToColor());
        }

        public static void SetText(this IQView<TextView> view, string text)
        {
            view.PlatformView.SetText(text);
        }

        public static void SetText(this TextView view, string text)
        {
            view.Text = text;
        }
    }
}
