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
    public enum TextAlignment
    {
        Left, Center, Right
    }

    public static class TextAlignmentExtensions
    {
#if __IOS__

        public static UITextAlignment ToUITextAlignment(this TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Center:
                    return UITextAlignment.Center;
                case TextAlignment.Left:
                    return UITextAlignment.Left;
                case TextAlignment.Right:
                    return UITextAlignment.Right;
                default:
                    throw new ArgumentException(nameof(alignment));
            }
        }
#endif

#if __ANDROID__
        public static GravityFlags ToGravityFlags(this TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Center:
                    return GravityFlags.Center;
                case TextAlignment.Left:
                    return GravityFlags.Left;
                case TextAlignment.Right:
                    return GravityFlags.Right;
                default:
                    throw new ArgumentException(nameof(alignment));
            }
        }
#endif
    }
}
