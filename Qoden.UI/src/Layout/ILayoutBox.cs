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
        /// Set distance from <see cref="OuterBounds"/> left edge to box left edge.
        /// </summary>
        /// <param name="l">Distance in pixels</param>
        void SetLeft(Pixel l);
        /// <summary>
        /// Set distance from <see cref="OuterBounds"/> right edge to view right edge.
        /// </summary>
        /// <param name="r">Distance in pixels</param>
        void SetRight(Pixel r);
        /// <summary>
        /// Set distance from <see cref="OuterBounds"/> top edge to view top edge.
        /// </summary>
        /// <param name="t">Distance in pixels</param>
        void SetTop(Pixel t);
        /// <summary>
        /// Set distance from <see cref="OuterBounds"/> bottom edge to view bottom edge.
        /// </summary>
        /// <param name="b">Distance in pixels</param>
        void SetBottom(Pixel b);
        /// <summary>
        /// Set box width.
        /// </summary>
        /// <param name="w">Width in pixels</param>
        void SetWidth(Pixel w);
        /// <summary>
        /// Set box height.
        /// </summary>
        /// <param name="h">Height in pixels</param>
        void SetHeight(Pixel h);
        /// <summary>
        /// Set box center X position relative to <see cref="OuterBounds"/>.
        /// </summary>
        /// <param name="cx">Center x position in pixels</param>
        void SetCenterX(Pixel cx);
        /// <summary>
        /// Set box center Y position relative to <see cref="OuterBounds"/>.
        /// </summary>
        /// <param name="cy">Center y position in pixels</param>
        void SetCenterY(Pixel cy);
        //These variables control horizontal dimensions
        float Left { get; }
        float Right { get; }
        float Width { get; }
        float CenterX { get; }
        //These variables control vertical dimensions
        float Top { get; }
        float Bottom { get; }
        float Height { get; }
        float CenterY { get; }
        /// <summary>
        /// Caculated layout width in pixels
        /// </summary>
        float LayoutWidth { get; }
        /// <summary>
        /// Calculated layout height in pixels
        /// </summary>
        float LayoutHeight { get; }
        /// <summary>
        /// Calculated layout left position in view coordinates in pixels
        /// </summary>
        float LayoutLeft { get; }
        /// <summary>
        /// Calculated layout right position in view coordinates in pixels
        /// </summary>
        float LayoutRight { get; }
        /// <summary>
        /// Caculated layout top position in view coordinates in pixels
        /// </summary>
        float LayoutTop { get; }
        /// <summary>
        /// Calculated layout bottom position in view coordinates in pixels
        /// </summary>
        float LayoutBottom { get; }
        /// <summary>
        /// Caclulated layout center in view coordinates in pixels
        /// </summary>
        PointF LayoutCenter { get; }
        /// <summary>
        /// Caclualted layout size in pixels
        /// </summary>
        SizeF LayoutSize { get; }
        /// <summary>
        /// Calculated layout bounds in view coordinate system in pixels
        /// </summary>
        RectangleF LayoutBounds { get; }
        /// <summary>
        /// Outer bounds in view coordinate system in pixels
        /// </summary>
        RectangleF OuterBounds { get; }

        SizeF PreferredBoundingSize { get; }
    }
}

