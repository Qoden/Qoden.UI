using System;
using System.Drawing;

#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    public static class LayoutBoxFrame
    {
        /// <summary>
        /// <see cref="ILayoutBox"/> rectangle in <see cref="ILayoutBox.OuterBounds"/> coordinates.
        /// </summary>
        public static RectangleF Bounds(this ILayoutBox box)
        {
            return new RectangleF(box.MarginLeft, box.MarginTop, box.Width, box.Height);
        }

        /// <summary>
        /// <see cref="ILayoutBox"/> rectangle in view coordinates.
        /// </summary>
        public static RectangleF Frame(this ILayoutBox box)
        {
            return new RectangleF(
                box.MarginLeft + box.OuterBounds.Left,
                box.MarginTop + box.OuterBounds.Top,
                box.Width,
                box.Height);
        }
        
        /// <summary>
        /// <see cref="ILayoutBox"/> size
        /// </summary>
        public static SizeF Size(this ILayoutBox box)
        {
            return new SizeF(box.Width, box.Height);
        }

        /// <summary>
        /// Caclulated layout center in view coordinates in pixels.
        /// </summary>
        public static PointF Center(this ILayoutBox box)
        {
            var frame = box.Frame();
            return new PointF(frame.Left + frame.Width / 2, frame.Top + frame.Height / 2);
        }
        
        /// <summary>
        /// Layout box center in <see cref="ILayoutBox.OuterBounds"/> coordinates in pixels.
        /// </summary>
        public static PointF BoundsCenter(this ILayoutBox box)
        {
            var frame = box.Bounds();
            return new PointF(frame.Left + frame.Width / 2, frame.Top + frame.Height / 2);
        }
    }
}