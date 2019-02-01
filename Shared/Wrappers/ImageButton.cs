using System;
using Qoden.Binding;
#if __IOS__
using UIKit;
using Foundation;
using PlatformButton = UIKit.UIButton;
#endif
#if __ANDROID__
using Android.Graphics.Drawables;
using PlatformButton = Android.Widget.ImageButton;
using Android.Graphics;
#endif

namespace Qoden.UI.Wrappers
{
    public struct ImageButton
    {
        public static implicit operator PlatformButton(ImageButton area) { return area.PlatformView; }
        public PlatformButton PlatformView { get; set; }
        public View AsView() { return new View() { PlatformView = PlatformView }; }

        public void SetImage(Image image)
        {
#if __ANDROID__
            PlatformView.SetImageDrawable(image.PlatformImage);
#elif __IOS__
            PlatformView.SetImage(image, UIKit.UIControlState.Normal);
#endif
        }

        public EventCommandTrigger ClickTarget()
        {
            return PlatformView.ClickTarget();
        }
    }

    public static class ImageButtonExtensions
    {
        /// <summary>
        /// Create new button view and add it to parent
        /// </summary>
        public static ImageButton ImageButton(this ViewBuilder b, bool addSubview = true)
        {
#if __IOS__
            var platformButton = new PlatformButton(UIButtonType.Custom);
            var button = new ImageButton() { PlatformView = platformButton };
#endif
#if __ANDROID__
            var button = new ImageButton() { PlatformView = new PlatformButton(b.Context) };
            button.PlatformView.SetPadding(0, 0, 0, 0);
            button.PlatformView.Background = new ColorDrawable(Color.Transparent);
#endif
            if (addSubview) b.AddSubview(button.PlatformView);
            return button;
        }

        /// <summary>
        /// Wrap convert platform view with cross platform wrapper
        /// </summary>
        public static ImageButton AsView(this PlatformButton view)
        {
            return new ImageButton() { PlatformView = view };
        }
    }
}
