using System;
#if __IOS__
using PlatformBar = UIKit.UINavigationBar;
#elif __ANDROID__
using Android.Widget;
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
            // todo: find a better way to apply font. avoid searching internal view hierarchy
            for (var i = 0; i < PlatformView.ChildCount; i++)
            {
                if (PlatformView.GetChildAt(i) is TextView titleView)
                {
                    titleView.AsLabel().SetFont(font);
                    break;
                }
            }
#endif
        }

        public void SetForegroundTintColor(RGB color)
        {
#if __IOS__
            PlatformView.TintColor = color.ToColor();
#elif __ANDROID__
            PlatformView.ForegroundTintList = Android.Content.Res.ColorStateList.ValueOf(color.ToColor());
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

    public struct MenuItemInfo
    {
        public string Title { get; set; }
        public Image Icon { get; set; }
        public int Id { get; set; }
        public Binding.Command Command { get; set; }
    }
}
