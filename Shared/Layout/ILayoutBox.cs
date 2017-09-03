using System.Drawing;
#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    /// <summary>
    /// Describes rectangle in terms of relative offsets from edges of <see cref="OuterBounds"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Horizontal (same for vertical) dimensions can be described in a few 
    /// different ways:
    /// <list>
    ///   <item>Left and Width</item>
    ///   <item>Left and Right</item>
    ///   <item>Left and CenterX</item>
    ///   <item>Right and Width</item>
    ///   <item>Right and CenterX</item>
    ///   <item>CenterX and Width</item>
    /// </list>
    /// </para>
    /// <para>
    /// There are also few ambigious combinations like CenterX and Left and Right.
    /// If client set such combination then LayoutBox discards ambigious 
    /// element. When doing so LayoutBox takes into account following offsets priorities:
    /// <list>
    ///   <item>Let and Top</item>
    ///   <item>Width and Height</item>
    ///   <item>CenterX and CenterY</item>
    ///   <item>Right and Bottom</item>
    /// </list>  
    /// For example layout box has Left and Width set and then client set Right.
    /// This is obviously ambigious and Left or Width has to be discarded. In this case Width is disacarded since
    /// it is less important than Left.
    /// </para>
    /// <para>
    /// Most of calculations on LayoutBox implemented as extension methods.
    /// You can implement simple ILayoutBox interface on any object and get all 
    /// of LayoutBox calculation for free.
    /// </para>
    /// <para>
    /// LayoutBox can also calculate size required to correctly wrap given 
    /// box, see <see cref="T:LayoutBox_Frame.BoundingFrame"/>.
    /// </para>
    /// </remarks>
    public interface ILayoutBox
    {
        /// <summary>
        /// Measurement unit for relative values.
        /// </summary>
        IUnit Unit { get; }
        /// <summary>
        /// Distance in pixels from <see cref="OuterBounds"/> left edge to box left edge.
        /// </summary>
        float MarginLeft { get; set; }
        /// <summary>
        /// Distance in pixels from <see cref="OuterBounds"/> right edge to view right edge.
        /// </summary>
        float MarginRight { get; set; }
        /// <summary>
        /// Distance in pixels from <see cref="OuterBounds"/> top edge to view top edge.
        /// </summary>
        float MarginTop { get; set; }
        /// <summary>
        /// Distance in pixels from <see cref="OuterBounds"/> bottom edge to view bottom edge.
        /// </summary>
        float MarginBottom { get; set; }
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
        
        /// <summary>
        /// Left coordinate inside <see cref="OuterBounds"/>
        /// </summary>
        float Left  {get; }
        /// <summary>
        /// Right coordinate inside <see cref="OuterBounds"/>
        /// </summary>
        float Right  {get; }
        /// <summary>
        /// Top coordinate inside <see cref="OuterBounds"/>
        /// </summary>
        float Top  {get; }
        /// <summary>
        /// Bottom coordinate inside <see cref="OuterBounds"/>
        /// </summary>
        float Bottom  {get; }

        /// <summary>
        /// Layout box rectangle inside <see cref="OuterBounds"/>.
        /// </summary>
        RectangleF Bounds  {get; }
        /// <summary>
        /// Layout box size.
        /// </summary>
        SizeF Size  {get; }
        /// <summary>
        /// Layout box rectangle inside view.
        /// </summary>
        RectangleF Frame  {get; }
    }
}

