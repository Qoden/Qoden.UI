using System.Drawing;
#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    /// <summary>
    /// Describes rectangle in terms of relative offsets from edges of <see cref="OuterBounds"/>.
    /// </summary>
    public interface ILayoutBox
    {
        /// <summary>
        /// Measurement unit for relative values.
        /// </summary>
        IUnit Unit { get; }
        /// <summary>
        /// Distance in pixels from <see cref="OuterBounds"/> left edge to box left edge.
        /// </summary>
        float Left { get; set; }
        /// <summary>
        /// Distance in pixels from <see cref="OuterBounds"/> right edge to view right edge.
        /// </summary>
        float Right { get; set; }
        /// <summary>
        /// Distance in pixels from <see cref="OuterBounds"/> top edge to view top edge.
        /// </summary>
        float Top { get; set; }
        /// <summary>
        /// Distance in pixels from <see cref="OuterBounds"/> bottom edge to view bottom edge.
        /// </summary>
        float Bottom { get; set; }
        /// <summary>
        /// Set box width in pixels.
        /// </summary>
        float Width { get; set; }
        /// <summary>
        /// Set box height in pixels.
        /// </summary>
        float Height { get; set; }
        /// <summary>
        /// Box center X position in pixels relative to <see cref="OuterBounds"/>.
        /// </summary>
        float CenterX { get; set; }
        /// <summary>
        /// Box center Y position in pixels relative to <see cref="OuterBounds"/>.
        /// </summary>
        float CenterY { get; set; }
        /// <summary>
        /// Outer bounds in view coordinate system in pixels
        /// </summary>
        RectangleF OuterBounds { get; }

    }
}

