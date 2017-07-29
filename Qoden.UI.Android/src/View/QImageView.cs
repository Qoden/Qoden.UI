using System;
using Android.Widget;

namespace Qoden.UI
{
    public partial class QImageView
    {        
    }

    public static partial class QImageView_Extensions
    {
        public static void SetImage(this ImageView view, QImage image)
        {
            view.SetImageDrawable(image.PlatformImage);
        }
    }
}
