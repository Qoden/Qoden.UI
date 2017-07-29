using System;
using UIKit;

namespace Qoden.UI
{
    public partial class QImageView
    {        
    }

    public static partial class QImageView_Extensions
    {
        public static void SetImage(this UIImageView view, QImage image)
        {
            view.Image = image.PlatformImage;
        }
    }
}
