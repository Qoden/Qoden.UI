using System.Drawing;
#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    public static class LayoutBox_Relative
    {
        /// <summary>
        /// Place this box before provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Before<T>(this T box, RectangleF reference, Pixel dx) where T : ILayoutBox
        {
            var referenceOffset = box.OuterBounds.Right - reference.Left;
            box.SetRight(Pixel.Val(referenceOffset + dx.Value));
            return box;
        }

        /// <summary>
        /// Place this box before provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Before<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            return box.Before(reference, box.Unit.ToPixels(dx));
        }

        /// <summary>
        /// Place this box after provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T After<T>(this T box, RectangleF reference, Pixel dx) where T : ILayoutBox
        {
            var referenceOffset = reference.Right - box.OuterBounds.Left;
            box.SetLeft(Pixel.Val(referenceOffset + dx.Value));
            return box;
        }
        /// <summary>
        /// Place this box after provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T After<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            return box.After(reference, box.Unit.ToPixels(dx));
        }

        /// <summary>
        /// Place this box below provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Below<T>(this T box, RectangleF reference, Pixel dx) where T : ILayoutBox
        {
            var referenceOffset = reference.Bottom - box.OuterBounds.Top;
            box.SetTop(Pixel.Val(referenceOffset + dx.Value));
            return box;
        }

        /// <summary>
        /// Place this box below provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Below<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            return box.Below(reference, box.Unit.ToPixels(dx));
        }
        /// <summary>
        /// Place this box above provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Above<T>(this T box, RectangleF reference, Pixel dx) where T : ILayoutBox
        {
            var referenceOffset = box.OuterBounds.Bottom - reference.Top;
            box.SetBottom(Pixel.Val(referenceOffset + dx.Value));
            return box;
        }
        /// <summary>
        /// Place this box above provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Above<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            return box.Above(reference, box.Unit.ToPixels(dx));
        }
    }
}

