using System;
using Android.Views;

namespace Qoden.UI
{
    public static class TextAlignmentExtensions
    {
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
    }
}
