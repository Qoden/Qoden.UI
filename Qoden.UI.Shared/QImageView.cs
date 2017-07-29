using System;
namespace Qoden.UI
{
#if __IOS__
    using ImageView = UIKit.UIImageView;
#endif
#if __ANDROID__
    using ImageView = Android.Widget.ImageView;
#endif

    public partial class QImageView : BaseView<ImageView>
    {
        public QImageView()
        {
        }

        public QImageView(ImageView target) : base(target)
        {
        }
    }

    public static partial class QImageView_Extensions
    {
        public static void SetImage(this IQView<ImageView> view, QImage image)
        {
            view.PlatformView.SetImage(image);
        }
    }
}
