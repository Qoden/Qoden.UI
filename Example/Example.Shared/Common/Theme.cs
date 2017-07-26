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

        public static class Colors
        {
            public static readonly RGB BG = new RGB(17, 17, 17);
            public static readonly RGB TableHeadText = new RGB(74, 82, 89);
            public static readonly RGB TableFieldLabelText = new RGB(111, 118, 124);
            public static readonly RGB TableFieldBg = new RGB(24, 24, 24);
            public static readonly RGB TableFieldHintText = new RGB(51, 56, 61);
            public static readonly RGB TableFieldText = new RGB(255, 255, 255);
            public static readonly RGB TableButtonIconText = new RGB(48, 112, 169);

            public static readonly RGB NavbarButtonColor = new RGB(47, 112, 170);

            public static readonly RGB Error = new RGB(178, 63, 63);
        }

        public static class Fonts 
        {
            public static readonly Font TableFieldLabel = new Font(".SFUIDisplay", 15);
            public static readonly Font TableFieldText = new Font(".SFUIDisplay", 15);
            public static readonly Font TableHeader = new Font(".SFUIDisplay-Semibold", 11);
            public static readonly Font Headline = new Font(".SFUIDisplay", 34, FontStyle.Bold);
            public static readonly Font HeadlineSmall = new Font(".SFUIDisplay-Semibold", 17);

            public static readonly Font NavbarButton = new Font(".SFUIDisplay", 17);
            public static readonly Font NavbarDefaultButton = new Font(".SFUIDisplay-Semibold", 17);
        }

        public static readonly Font Icons = new Font("proveo_ios", 25, FontStyle.Normal);
        public static readonly Font IconsSmall = new Font("proveo_ios", 17, FontStyle.Normal);

        public const float FormHeaderHeight = 45;
        public const float FormEmptyHeaderHeight = 20;

        public static void Field(View view)
        {
            view.SetBackgroundColor(Colors.TableFieldBg);
        }

        public static void TextField(ILabeledField field)
        {
            field.Label.SetTextColor(Colors.TableFieldLabelText);
            field.Label.SetBackgroundColor(RGB.Clear);
            field.Label.SetFont(Fonts.TableFieldLabel);

            field.Text.SetTextColor(Colors.TableFieldText);
            field.Text.SetBackgroundColor(RGB.Clear);
            TextFieldDefaults(field.Text);
        }

        public static void ButtonField(TextViewWithIcon field)
        {
            field.VerticalAdjustment = 1;

            field.Title.SetTextColor(Colors.TableFieldText);
            field.Title.SetFont(Fonts.TableFieldText);

            field.Icon.SetTextColor(Colors.TableButtonIconText);
            field.Icon.SetFont(Icons);
        }

        public static void Headline(FormHeaderView view)
        {
            view.SetBackgroundColor(Colors.BG);

            view.Title.SetTextColor(Colors.TableFieldText);
            view.Title.SetFont(Fonts.HeadlineSmall);
            view.Title.SetBackgroundColor(RGB.Clear);
#if __ANDROID__
            view.Title.PlatformView.LetterSpacing  = 0.2f;
#endif
            view.Done.SetTextColor(Colors.NavbarButtonColor);
            view.Done.SetText("Done");
            view.Done.SetFont(Fonts.NavbarDefaultButton);
            view.Done.SetBackgroundColor(RGB.Clear);

            view.Cancel.SetTextColor(Colors.NavbarButtonColor);
            view.Cancel.SetText("Cancel");
            view.Cancel.SetFont(Fonts.NavbarButton);
            view.Cancel.SetBackgroundColor(RGB.Clear);
        }

        public static void FormHeader(TextViewWithIcon header)
        {
            header.SetBackgroundColor(Colors.BG);
            header.VerticalAdjustment = 3;

            header.Title.SetTextColor(Colors.TableHeadText);
            header.Title.SetFont(Fonts.TableHeader);

            header.Icon.SetTextColor(Colors.TableHeadText);
            header.Icon.SetFont(IconsSmall);

            header.IconSpacing = 5;
        }

    }

    public class ThemeAttribute : DecoratorAttribute
    {
        public ThemeAttribute(params string[] methods) : base(typeof(Theme), methods)
        {
        }
    }
}
