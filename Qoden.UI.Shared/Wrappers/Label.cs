using System;
#if __IOS__
using Foundation;
using PlatformLabel = UIKit.UILabel;
#endif
#if __ANDROID__
    using PlatformLabel = Android.Widget.TextView;
#endif

namespace Qoden.UI.Wrappers
{

    public struct Label
    {
        public static implicit operator PlatformLabel(Label area) { return area.PlatformView; }

        public PlatformLabel PlatformView { get; set; }

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
            PlatformView.TextColor = color.ToColor();
#endif
#if __ANDROID__
            PlatformView.SetTextColor(color.ToColor());
#endif
        }

        public string Text
        {
            get => PlatformView.Text;
            set => PlatformView.Text = value;
        }

        public void SetAlignment(TextAlignment value)
        {
#if __IOS__
            PlatformView.TextAlignment = value.ToUITextAlignment();
#endif
#if __ANDROID__
            PlatformView.Gravity = value.ToGravityFlags();
#endif
        }

#if __IOS__
        public void SetText(NSAttributedString text)
        {
            PlatformView.AttributedText = text;
        }
#endif
    }

    public static class LabelExtensions
    {
        public static Label Label(this ViewBuilder b)
        {
#if __IOS__
            return new Label() { PlatformView = b.AddSubview(new PlatformLabel()) };
#endif
#if __ANDROID__
            var label = new Label() { PlatformView = b.AddSubview(new PlatformLabel(b.Context)) };
            //This is to mimic iOS UILabel behavior which makes more sense as a default
            label.PlatformView.Gravity = Android.Views.GravityFlags.CenterVertical;
            return label;
#endif
        }

        public static Label AsLabel(this PlatformLabel label)
        {
            return new Label() { PlatformView = label };
        }
    }
}
