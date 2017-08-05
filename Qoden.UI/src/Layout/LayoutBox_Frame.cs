using System.Drawing;
#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    public static class LayoutBox_Frame
    {
        /// <summary>
        /// Calculated layout width in pixels
        /// </summary>
        public static float FrameWidth(this ILayoutBox box)
        {
            if (LayoutBox.IsSet(box.Width))
                return box.Width;
            if (LayoutBox.IsSet(box.CenterX) && LayoutBox.IsSet(box.Left))
                return (box.CenterX - box.Left) * 2;
            if (LayoutBox.IsSet(box.CenterX) && LayoutBox.IsSet(box.Right))
                return (box.OuterBounds.Width - box.Right - box.CenterX) * 2;
            if (LayoutBox.IsSet(box.Left) && LayoutBox.IsSet(box.Right))
                return box.OuterBounds.Width - box.Right - box.Left;
            return box.OuterBounds.Width;
        }
        /// <summary>
        /// Calculated layout height in pixels
        /// </summary>
        public static float FrameHeight(this ILayoutBox box)
        {
            if (LayoutBox.IsSet(box.Height))
                return box.Height;
            if (LayoutBox.IsSet(box.Top) && LayoutBox.IsSet(box.Bottom))
                return box.OuterBounds.Height - box.Bottom - box.Top;
            if (LayoutBox.IsSet(box.CenterY) && LayoutBox.IsSet(box.Top))
                return (box.CenterY - box.Top) * 2;
            if (LayoutBox.IsSet(box.CenterY) && LayoutBox.IsSet(box.Bottom))
                return (box.OuterBounds.Height - box.Bottom - box.CenterY) * 2;
            return box.OuterBounds.Height;
        }
        /// <summary>
        /// Calculated layout left position in view coordinates in pixels
        /// </summary>
        public static float FrameLeft(this ILayoutBox box)
        {
            if (LayoutBox.IsSet(box.Left))
                return box.OuterBounds.Left + box.Left;
            if (LayoutBox.IsSet(box.CenterX) && LayoutBox.IsSet(box.Width))
                return box.OuterBounds.Left + box.CenterX - box.Width / 2;
            if (LayoutBox.IsSet(box.Right) && LayoutBox.IsSet(box.Width))
                return box.OuterBounds.Right - box.Right - box.Width;
            return box.OuterBounds.Left;
        }
        /// <summary>
        /// Calculated layout right position in view coordinates in pixels
        /// </summary>
        public static float FrameRight(this ILayoutBox box)
        {
            if (LayoutBox.IsSet(box.Right))
                return box.OuterBounds.Right - box.Right;
            if (LayoutBox.IsSet(box.Left) && LayoutBox.IsSet(box.Width))
                return box.OuterBounds.Left + box.Left + box.Width;
            if (LayoutBox.IsSet(box.CenterX) && LayoutBox.IsSet(box.Width))
                return box.OuterBounds.Left + box.CenterX + box.Width / 2;
            return box.OuterBounds.Right;
        }
        /// <summary>
        /// Caculated layout top position in view coordinates in pixels
        /// </summary>
        public static float FrameTop(this ILayoutBox box)
        {
            if (LayoutBox.IsSet(box.Top))
                return box.OuterBounds.Top + box.Top;
            if (LayoutBox.IsSet(box.CenterY) && LayoutBox.IsSet(box.Height))
                return box.OuterBounds.Top + box.CenterY - box.Height / 2;
            if (LayoutBox.IsSet(box.Bottom) && LayoutBox.IsSet(box.Height))
                return box.OuterBounds.Bottom - box.Bottom - box.Height;
            return box.OuterBounds.Top;
        }
        /// <summary>
        /// Calculated layout bottom position in view coordinates in pixels
        /// </summary>
        public static float FrameBottom(this ILayoutBox box)
        {
            if (LayoutBox.IsSet(box.Bottom))
                return box.OuterBounds.Bottom - box.Bottom;
            if (LayoutBox.IsSet(box.Top) && LayoutBox.IsSet(box.Height))
                return box.OuterBounds.Top + box.Top + box.Height;
            if (LayoutBox.IsSet(box.CenterY) && LayoutBox.IsSet(box.Height))
                return box.OuterBounds.Top + box.CenterY + box.Height / 2;
            return box.OuterBounds.Bottom;
        }
        /// <summary>
        /// Caclulated layout center in view coordinates in pixels
        /// </summary>
        public static PointF FrameCenter(this ILayoutBox box)
        {
            return new PointF(box.FrameLeft() + box.FrameWidth() / 2, box.FrameTop() + box.FrameHeight() / 2);
        }
        /// <summary>
        /// Caclualted layout size in pixels
        /// </summary>
        public static SizeF FrameSize(this ILayoutBox box)
        {
            return new SizeF(box.FrameWidth(), box.FrameHeight());
        }
        /// <summary>
        /// Calculated layout bounds in view coordinate system in pixels
        /// </summary>
        public static RectangleF Frame(this ILayoutBox box)
        {
            return new RectangleF(box.FrameLeft(), box.FrameTop(), box.FrameWidth(), box.FrameHeight());
        }

        public static float PreferredBoundingWidth(this ILayoutBox box)
        {
            if (LayoutBox.IsSet(box.Left) && LayoutBox.IsSet(box.Right) && LayoutBox.IsSet(box.Width))
            {
                return box.Left + box.Right + box.Width;
            }

            if (LayoutBox.IsSet(box.Left) && LayoutBox.IsSet(box.CenterX) && LayoutBox.IsSet(box.Right))
            {
                return box.Left + (box.CenterX - box.Left) * 2 + box.Right;
            }

            if (LayoutBox.IsSet(box.Left) && LayoutBox.IsSet(box.Width))
            {
                return box.Left + box.Width;
            }

            if (LayoutBox.IsSet(box.Left) && LayoutBox.IsSet(box.CenterX))
            {
                return box.Left + (box.CenterX - box.Left) * 2;
            }

            if (LayoutBox.IsSet(box.Right) && LayoutBox.IsSet(box.Width))
            {
                return box.Right + box.Width;
            }

            if (LayoutBox.IsSet(box.Width) && LayoutBox.IsSet(box.CenterX))
                return box.CenterX + box.Width / 2;

            if (LayoutBox.IsSet(box.Width))
                return box.Width;

            return box.FrameWidth();
        }

        public static float PreferredBoundingHeight(this ILayoutBox box)
        {
            if (LayoutBox.IsSet(box.Top) && LayoutBox.IsSet(box.Bottom) && LayoutBox.IsSet(box.Height))
            {
                return box.Top + box.Bottom + box.Height;
            }

            if (LayoutBox.IsSet(box.Top) && LayoutBox.IsSet(box.CenterY) && LayoutBox.IsSet(box.Bottom))
            {
                return box.Top + (box.CenterY - box.Top) * 2 + box.Bottom;
            }

            if (LayoutBox.IsSet(box.Top) && LayoutBox.IsSet(box.Height))
            {
                return box.Top + box.Height;
            }

            if (LayoutBox.IsSet(box.Top) && LayoutBox.IsSet(box.CenterY))
            {
                return box.Top + (box.CenterY - box.Top) * 2;
            }

            if (LayoutBox.IsSet(box.Bottom) && LayoutBox.IsSet(box.Height))
            {
                return box.Bottom + box.Height;
            }

            if (LayoutBox.IsSet(box.Height) && LayoutBox.IsSet(box.CenterY))
                return box.CenterY + box.Height / 2;

            if (LayoutBox.IsSet(box.Height))
                return box.Height;

            return box.FrameHeight();
        }

        /// <summary>
        /// Gets the size of the preferred outer bounds. This bounds can fit 
        /// entire box with all specified parameters such as Left, Right, Width.
        /// </summary>
        public static SizeF PreferredBoundingSize(this ILayoutBox box)
        {
            return new SizeF(box.PreferredBoundingWidth(), box.PreferredBoundingHeight());
        }
    }
}

