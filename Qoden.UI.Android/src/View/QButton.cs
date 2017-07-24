using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace Qoden.UI
{
    public class QButton : BaseView<Button>
    {
    }

    public static class ButtonExtensions
    {
        public static void SetText(this IQView<Button> view, string text)
        {
            view.PlatformView.SetText(text);
        }

        public static void SetText(this Button view, string text)
        {
            view.Text = text;
        }
    }
}
