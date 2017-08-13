using System;
using UIKit;

namespace Qoden.UI
{
    public static class TextAlignmentExtensions
    {
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
    }
}
