using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Qoden.UI
{
    public class QTextView : BaseView<UILabel>
    {
        public QTextView()
        {
        }

        public QTextView(UILabel target) : base(target)
        {
        }
    }

    public static class TextViewExtnesions
    {
        public static void SetTextColor(this IQView<UILabel> view, RGB color)
        { 
            view.PlatformView.SetTextColor(color); 
        }

        public static void SetTextColor(this UILabel view, RGB color)
        {
            view.TextColor = color.ToColor();
        }

        public static void SetText(this IQView<UILabel> view, string text)
        { 
            view.PlatformView.SetText(text);  
        }

        public static void SetText(this UILabel view, string text)
        {
            view.Text = text;
        }

        public static void SetText(this IQView<UILabel> view, NSAttributedString text)
        {
            view.PlatformView.SetText(text);
        }

        public static void SetText(this UILabel view, NSAttributedString text)
        {
            view.AttributedText = text;
        }

        public static void SetFont(this IQView<UILabel> view, Font font)
        {
            view.PlatformView.SetFont(font);
        }

        public static void SetFont(this UILabel view, Font font)
        {
            view.Font = font.ToFont();
        }
    }
}
