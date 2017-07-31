using System;
using UIKit;

namespace Qoden.UI
{
    public partial class QImageView
    {
#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
        static QImageView()
        {
            if (LinkerTrick.False)
            {
                new UIImageView();
            }
        }
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'    
    }

    public static partial class QImageView_Extensions
    {
        public static void SetImage(this UIImageView view, QImage image)
        {
            view.Image = image.PlatformImage;
        }
    }
}
