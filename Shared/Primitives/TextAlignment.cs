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
    [Flags]
    public enum QodenTextAlignment
    {
        Left = 1, Center = 2, Right = 4
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
                case QodenTextAlignment.Center | QodenTextAlignment.Right:
                    return UITextAlignment.Right;
                case QodenTextAlignment.Center | QodenTextAlignment.Left:
                    return UITextAlignment.Left;
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
                case QodenTextAlignment.Center | QodenTextAlignment.Right:
                    return GravityFlags.Center | GravityFlags.Right;
                case QodenTextAlignment.Center | QodenTextAlignment.Left:
                    return GravityFlags.Center | GravityFlags.Left;
                default:
                    throw new ArgumentException(nameof(alignment));
            }
        }
#endif
    }
}
