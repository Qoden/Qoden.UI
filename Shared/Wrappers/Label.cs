using System;
using Qoden.Binding;
#if __IOS__
using UIKit;
using Foundation;
using PlatformLabel = UIKit.UILabel;
#endif
#if __ANDROID__
using Android.Graphics;
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
        
        public void SetUnderline()
        {
#if __IOS__
            var underlineAttribute = new UIStringAttributes {UnderlineStyle = NSUnderlineStyle.Single};
            PlatformView.AttributedText = new NSAttributedString(Text ?? "", underlineAttribute);
#elif __ANDROID__
            PlatformView.PaintFlags |= PaintFlags.UnderlineText;
#endif
        }

        public string Text
        {
            get => PlatformView.Text;
            set => PlatformView.Text = value;
        }

        public void SetTextAlignment(QodenTextAlignment value)
        {
#if __IOS__
            PlatformView.TextAlignment = value.ToUITextAlignment();
#endif
#if __ANDROID__
            PlatformView.Gravity = value.ToGravityFlags();
#endif
        }

        public void SetText(string text)
        {
            PlatformView.Text = text;
        }

#if __IOS__
        public void SetText(NSAttributedString text)
        {
            PlatformView.AttributedText = text;
        }
#endif
		public IProperty<string> TextProperty()
		{
			return PlatformView.TextProperty();
		}
    }

    public static class LabelExtensions
    {
        public static Label Label(this ViewBuilder b, bool addSubview = true)
        {
#if __IOS__
            var view = new Label() { PlatformView = new PlatformLabel() };
#endif
#if __ANDROID__
            var view = new Label() { PlatformView = new PlatformLabel(b.Context) };
            //This is to mimic iOS UILabel behavior which makes more sense as a default
            view.PlatformView.Gravity = Android.Views.GravityFlags.CenterVertical;
#endif
            if (addSubview) b.AddSubview(view.PlatformView);
            return view;
        }

        public static Label AsLabel(this PlatformLabel label)
        {
            return new Label() { PlatformView = label };
        }
    }
}
