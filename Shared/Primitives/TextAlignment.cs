#if __IOS__
using System;
using UIKit;
#endif
#if __ANDROID__
using System;
using Android.Views;
#endif

namespace Qoden.UI
{
    public enum QodenTextAlignment
    {
        Left, Center, Right
    }

    public static class TextAlignmentExtensions
    {
#if __IOS__

        public static UITextAlignment ToUITextAlignment(this QodenTextAlignment alignment)
        {
            switch (alignment)
            {
                case QodenTextAlignment.Center:
                    return UITextAlignment.Center;
                case QodenTextAlignment.Left:
                    return UITextAlignment.Left;
                case QodenTextAlignment.Right:
                    return UITextAlignment.Right;
                default:
                    throw new ArgumentException(nameof(alignment));
            }
        }
#endif

#if __ANDROID__
        public static GravityFlags ToGravityFlags(this QodenTextAlignment alignment)
        {
            switch (alignment)
            {
                case QodenTextAlignment.Center:
                    return GravityFlags.Center;
                case QodenTextAlignment.Left:
                    return GravityFlags.Left;
                case QodenTextAlignment.Right:
                    return GravityFlags.Right;
                default:
                    throw new ArgumentException(nameof(alignment));
            }
        }
#endif
    }
}
