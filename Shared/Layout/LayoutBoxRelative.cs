using System.Drawing;
#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    public static class LayoutBoxRelative
    {
        /// <summary>
        /// Place this box before provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Before<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            var referenceOffset = box.OuterBounds.Right - reference.Left;
            box.MarginRight = referenceOffset + dx;
            return box;
        }
        
        /// <summary>
        /// Place this box before provided reference layout box. Reference is in view coordinate system.
        /// </summary>
        public static T Before<T>(this T box, ILayoutBox reference, float dx = 0) where T : ILayoutBox
        {
            return box.Before(reference.Frame(), dx);
        }

        /// <summary>
        /// Place this box after provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T After<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            var referenceOffset = reference.Right - box.OuterBounds.Left;
            box.MarginLeft = referenceOffset + dx;
            return box;
        }
               
        /// <summary>
        /// Place this box after provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T After<T>(this T box, ILayoutBox reference, float dx = 0) where T : ILayoutBox
        {
            return box.After(reference.Frame(), dx);
        }

        /// <summary>
        /// Place this box below provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Below<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            var referenceOffset = reference.Bottom - box.OuterBounds.Top;
            box.MarginTop = referenceOffset + dx;
            return box;
        }
        /// <summary>
        /// Place this box below provided reference layout box. Reference is in view coordinate system.
        /// </summary>
        public static T Below<T>(this T box, ILayoutBox reference, float dx = 0) where T : ILayoutBox
        {
            return box.Below(reference.Frame(), dx);
        }
        
        /// <summary>
        /// Place this box above provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Above<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            var referenceOffset = box.OuterBounds.Bottom - reference.Top;
            box.MarginBottom = referenceOffset + dx;
            return box;
        }
        
        /// <summary>
        /// Place this box above provided reference layout box. Reference is in view coordinate system.
        /// </summary>
        public static T Above<T>(this T box, ILayoutBox reference, float dx = 0) where T : ILayoutBox
        {
            return box.Above(reference.Frame(), dx);
        }
    }
}

