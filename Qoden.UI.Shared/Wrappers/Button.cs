using System;

#if __IOS__
using UIKit;
using PlatformButton = UIKit.UIButton;
#endif
#if __ANDROID__
using PlatformButton = Android.Widget.Button;
#endif

namespace Qoden.UI.Wrappers
{
    public struct Button
    {
        public static implicit operator PlatformButton(Button area) { return area.PlatformView; }
        public PlatformButton PlatformView { get; set; }
        public View AsView() { return new View() { PlatformView = PlatformView }; }

        public void SetFont(Font font)
        {
#if __IOS__
            PlatformView.Font = font.ToFont();
#endif
#if __ANDROID__
            PlatformView.Typeface = TypefaceCollection.Get(font.Name, font.Style);
            PlatformView.TextSize = font.Size;
#endif
        }

        public void SetTextColor(RGB color)
        {
#if __IOS__
            PlatformView.SetTitleColor(color.ToColor(), UIControlState.Normal);
#endif
#if __ANDROID__
            PlatformView.SetTextColor(color.ToColor());
#endif
        }

        public void SetTextAlignment(TextAlignment alignment)
        {
#if __IOS__
            PlatformView.TitleLabel.TextAlignment = alignment.ToUITextAlignment();
#endif
#if __ANDROID__
            PlatformView.Gravity = alignment.ToGravityFlags();
#endif
        }
    }

    public static class ButtonExtensions
    {
        /// <summary>
        /// Create new button view and add it to parent
        /// </summary>
        public static Button Button(this ViewBuilder b)
        {
#if __IOS__
            var platformButton = new PlatformButton(UIButtonType.Custom);
            var button = new Button() { PlatformView = b.AddSubview(platformButton) };
            return button;
#endif
#if __ANDROID__
            var button = new Button() { PlatformView = b.AddSubview(new PlatformButton(b.Context)) };
            button.PlatformView.SetAllCaps(false);
            return button;
#endif
        }

        /// <summary>
        /// Wrap convert platform view with cross platform wrapper
        /// </summary>
        public static Button AsView(this PlatformButton view)
        {
            return new Button() { PlatformView = view };
        }
    }
}
