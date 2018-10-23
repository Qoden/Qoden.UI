using System;
#if __IOS__
using PlatformBar = UIKit.UINavigationBar;
#elif __ANDROID__
using Android.Text;
using Android.Text.Style;
using Android.Graphics;
using PlatformBar = Android.Support.V7.Widget.Toolbar;
#endif
namespace Qoden.UI.Wrappers
{
    public struct ActionBar
    {
        public static implicit operator PlatformBar(ActionBar area) { return area.PlatformView; }
        public PlatformBar PlatformView { get; set; }
        public View AsView() { return new View() { PlatformView = PlatformView }; }

        public void SetFont(Font font)
        {
#if __IOS__
            var oldAttrs = PlatformView.TitleTextAttributes?.Dictionary;
            if(oldAttrs == null) 
            {
                PlatformView.TitleTextAttributes = new UIKit.UIStringAttributes()
                {
                    Font = font.ToFont()
                };
            } else 
            {
                PlatformView.TitleTextAttributes = new UIKit.UIStringAttributes(oldAttrs)
                {
                    Font = font.ToFont()
                };
            }
#elif __ANDROID__
            var typeface = TypefaceCollection.Get(font.Name, font.Style);
            var fontSize = new AbsoluteSizeSpan((int) font.Size.Dp());

            var spannableString = new SpannableString(PlatformView.Title);
            spannableString.SetSpan(new CustomTypefaceSpan(typeface), 0, spannableString.Length(), 0);
            spannableString.SetSpan(fontSize, 0, spannableString.Length(), 0);

            PlatformView.TitleFormatted = spannableString;
#endif
        }

        public void SetForegroundTintColor(RGB color)
        {
#if __IOS__
            PlatformView.TintColor = color.ToColor();
#elif __ANDROID__
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                PlatformView.ForegroundTintList = Android.Content.Res.ColorStateList.ValueOf(color.ToColor());
            }
#endif
        }

        public void SetBackgroundColor(RGB color)
        {
#if __IOS__
            PlatformView.BarTintColor = color.ToColor();
#elif __ANDROID__
            PlatformView.AsView().SetBackgroundColor(color);
#endif
        }

        public void SetTextColor(RGB color)
        {
#if __IOS__
            var oldAttrs = PlatformView.TitleTextAttributes?.Dictionary;
            if (oldAttrs == null)
            {
                PlatformView.TitleTextAttributes = new UIKit.UIStringAttributes()
                {
                    ForegroundColor = color.ToColor()
                };  
            }
            else
            {
                PlatformView.TitleTextAttributes = new UIKit.UIStringAttributes(oldAttrs)
                {
                    ForegroundColor = color.ToColor()
                };
            }
            PlatformView.TitleTextAttributes.ForegroundColor = color.ToColor();
            PlatformView.SetNeedsDisplay();
#elif __ANDROID__
            PlatformView.SetTitleTextColor(color.ToColor());
#endif
        }
    }
    public static class ActionBarExtensions
    {
        public static ActionBar GetActionBar(this QodenController controller, bool addSubview = true)
        {
#if __IOS__
            return controller.NavigationController.NavigationBar.AsActionBar();
#elif __ANDROID__
            // todo: unbound from QodenActivity type somehow
            return ((QodenActivity)controller.Activity).Toolbar.AsActionBar();
#endif
        }

        public static ActionBar AsActionBar(this PlatformBar bar) => new ActionBar() { PlatformView = bar };
    }

    public enum Side { None, Left, Right }

    public struct MenuItemInfo
    {
        public string Title { get; set; }
        public Image Icon { get; set; }
        public int Id { get; set; }
        public Binding.Command Command { get; set; }
        public Side Side { get; set; }
    }

#if __ANDROID__
    // Such class is required to apply custom typeface
    public class CustomTypefaceSpan : MetricAffectingSpan
    {
        private readonly Typeface _typeface;

        public CustomTypefaceSpan(Typeface typeface)
        {
            _typeface = typeface;
        }

        public override void UpdateDrawState(TextPaint tp)
        {
            ApplyCustomTypeface(tp, _typeface);
        }

        public override void UpdateMeasureState(TextPaint tp)
        {
            ApplyCustomTypeface(tp, _typeface);
        }

        private static void ApplyCustomTypeface(Paint paint, Typeface typeface)
        {
            paint.SetTypeface(typeface);
        }
    }
#endif
}
