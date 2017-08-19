using System;
#if __IOS__
    using PlatformTextField = UIKit.UITextField;
#endif
#if __ANDROID__
using PlatformTextField = Android.Widget.EditText;
#endif

namespace Qoden.UI.Wrappers
{
    public struct TextField
    {
        public static implicit operator PlatformTextField(TextField area) { return area.PlatformView; }
        public PlatformTextField PlatformView { get; set; }
        public View AsView() { return new View() { PlatformView = PlatformView }; }

        public string Text
        {
            get => PlatformView.Text;
            set => PlatformView.Text = value;
        }

        public string Hint
        {
#if __IOS__
            get => PlatformView.Placeholder;
            set => PlatformView.Placeholder = value;
#endif
#if __ANDROID__
            get => PlatformView.Hint;
            set => PlatformView.Hint = value;
#endif
        }

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

        public void SetTextAlignment(TextAlignment alignment)
        {
#if __IOS__
            PlatformView.TextAlignment = alignment.ToUITextAlignment();
#endif
#if __ANDROID__
            PlatformView.Gravity = alignment.ToGravityFlags();
#endif
        }
    }

    public static class TextFieldExtensions
    {
        public static TextField TextField(this ViewBuilder b)
        {
#if __IOS__
            return new TextField() { PlatformView = b.AddSubview(new PlatformTextField()) };
#endif
#if __ANDROID__
            return new TextField() { PlatformView = b.AddSubview(new PlatformTextField(b.Context)) };
#endif
        }

        public static TextField AsTextField(this PlatformTextField textField)
        {
            return new TextField() { PlatformView = textField };
        }
    }
}
