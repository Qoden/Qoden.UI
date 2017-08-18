using System;
namespace Qoden.UI.Wrappers
{
#if __IOS__
    using PlatformImageView = UIKit.UIImageView;
#endif
#if __ANDROID__
    using PlatformImageView = Android.Widget.ImageView;
#endif

    public struct ImageView
    {
        public static implicit operator PlatformImageView(ImageView area) { return area.PlatformView; }

        public PlatformImageView PlatformView { get; set; }

        public View AsView() { return new View() { PlatformView = PlatformView }; }

        public Image Image
        {
#if __IOS__
            get => new Image() { PlatformImage = PlatformView.Image };
            set => PlatformView.Image = value.PlatformImage;
#endif
#if __ANDROID__
            get => new Image() { PlatformImage = PlatformView.Drawable };
            set => PlatformView.SetImageDrawable(value.PlatformImage);
#endif
        }
    }

    public static class ImageViewExtensions
    {

        public static ImageView ImageView(this ViewBuilder b)
        {
#if __IOS__
            return new ImageView() { PlatformView = b.AddSubview(new PlatformImageView()) };
#endif
#if __ANDROID__
            return new ImageView() { PlatformView = b.AddSubview(new PlatformImageView(b.Context)) };
#endif
        }

        public static ImageView AsImageView(this PlatformImageView imageView)
        {
            return new ImageView() { PlatformView = imageView };
        }
    }
}
