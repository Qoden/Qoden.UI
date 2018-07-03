using System;
using Qoden.Binding;
#if __IOS__
using Foundation;
using UIKit;
using PlatformTextField = UIKit.UITextField;
#endif
#if __ANDROID__
using PlatformTextField = Android.Widget.EditText;
#endif

namespace Qoden.UI.Wrappers
{
    public struct TextField
    {
        public TextField(object view)
        {
            PlatformView = (PlatformTextField)view;
        }

        public static implicit operator PlatformTextField(TextField area) { return area.PlatformView; }
        public PlatformTextField PlatformView { get; set; }
        public View AsView() { return new View() { PlatformView = PlatformView }; }

        public string Text
        {
            get => PlatformView.Text;
        }

        public void SetText(string text)
        {
            PlatformView.Text = text;
        }

        public string Hint
        {
#if __IOS__
            get => PlatformView.Placeholder;
#endif
#if __ANDROID__
            get => PlatformView.Hint;
#endif
        }

        public void SetHint(string value)
        {
#if __IOS__
            //Set to attributedPlaceholder for color to save
            var outRange = new NSRange();
            PlatformView.AttributedPlaceholder = new NSAttributedString(value, null, (UIColor)PlatformView.AttributedPlaceholder?.GetAttribute(UIStringAttributeKey.ForegroundColor, 0, out outRange));
#endif
#if __ANDROID__
            PlatformView.Hint = value;
#endif
        }

        public void SetHintColor(RGB color)
        {
#if __IOS__
        	// If you set empty string for text in case Placeholder == null, then AttributedPlaceholder will be null,
        	// and you cannot retreive the color later.
            PlatformView.AttributedPlaceholder = new NSAttributedString(PlatformView.Placeholder ?? " ", null, color.ToColor());
#endif
#if __ANDROID__
            PlatformView.SetHintTextColor(color.ToColor());
#endif
        }

#if __IOS__
        public void SetHint(NSAttributedString value)
        {
            PlatformView.AttributedPlaceholder = value;
        }
#endif

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
            //https://stackoverflow.com/questions/46742074/uitextfield-text-color-is-not-changing-while-not-in-focus
            if (!PlatformView.IsFirstResponder)
                PlatformView.AttributedText = PlatformView.AttributedText;
#endif
#if __ANDROID__
            PlatformView.SetTextColor(color.ToColor());
#endif
        }

        public void SetTextAlignment(QodenTextAlignment alignment)
        {
#if __IOS__
            PlatformView.TextAlignment = alignment.ToUITextAlignment();
#endif
#if __ANDROID__
            PlatformView.Gravity = alignment.ToGravityFlags();
#endif
        }

        public IProperty<string> TextProperty()
        {
            return PlatformView.TextProperty();
        }
    }

    public static class TextFieldExtensions
    {
        public static TextField TextField(this ViewBuilder b, bool addSubview = true)
        {
#if __IOS__
            var view = new TextField() { PlatformView = new PlatformTextField() };
#endif
#if __ANDROID__
            var view = new TextField() { PlatformView = new PlatformTextField(b.Context) };
#endif
            if (addSubview) b.AddSubview(view.PlatformView);
            return view;
        }

        public static TextField AsTextField(this PlatformTextField textField)
        {
            return new TextField() { PlatformView = textField };
        }
    }
}
