using System;
using Qoden.UI;

#if __IOS__
using View = UIKit.UIView;
using Label = UIKit.UILabel;
#endif
#if __ANDROID__
using View = Android.Views.View;
using Label = Android.Widget.TextView;
#endif

namespace Example
{
    public partial class Theme
    {
        public static void TextField(TextField field)
        {
            field.SetBackgroundColor(new RGB(24, 24, 24));
            field.Label.SetTextColor(new RGB(111, 118, 124));
            field.Label.SetBackgroundColor(RGB.Clear);
            field.Label.SetFont(new Font("Arial", 15));

            field.Text.SetTextColor(new RGB(255, 255, 255));
            field.Text.SetBackgroundColor(RGB.Clear);
            field.Label.SetFont(new Font("Arial", 15));
        }

        public static void ButtonField(TextViewWithIcon field)
        {
            field.SetBackgroundColor(new RGB(24, 24, 24));

            field.Icon.SetTextColor(new RGB(111, 118, 124));
            field.Icon.SetBackgroundColor(RGB.Clear);
            field.Icon.SetText("ICN");
            //field.Icon.SetFont(new Font("Arial", 15));

            field.Title.SetTextColor(new RGB(255, 255, 255));
            field.Title.SetBackgroundColor(RGB.Clear);
            field.Title.SetFont(new Font("Arial", 15));
        }

        public static void Headline(QTextView view)
        {
            view.SetTextColor(new RGB(255, 255, 255));
            view.SetFont(new Font("Arial", 34, FontStyle.Bold));
            view.SetBackgroundColor(RGB.Clear);
#if __ANDROID__
            view.PlatformView.LetterSpacing  = 0.2f;
#endif
        }

        public static void FormHeader(TextViewWithIcon header)
        {
            header.SetBackgroundColor(new RGB(24, 255, 24));
            header.Title.SetTextColor(new RGB(255, 255, 255));
            header.Icon.SetTextColor(new RGB(255, 0, 0));
        }

        public const float FormHeaderHeight = 22;
    }

    public class ThemeAttribute : DecoratorAttribute
    {
        public ThemeAttribute(params string[] methods) : base(typeof(Theme), methods)
        {
        }
    }

}
