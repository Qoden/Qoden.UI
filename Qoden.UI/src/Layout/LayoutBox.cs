using System;
using System.Drawing;
#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    /**
     * Note to developers.
     * LayoutBox describes rectangle in terms of offsets from outer bounds.
     * Horizontal (same for vertical) offsets can be described in a few 
     * different ways:
     * a) Left and Width
     * b) Left and Right
     * c) Left and CenterX
     * d) Right and Width
     * e) Right and CenterX
     * f) CenterX and Width
     * 
     * There are also few ambigious combinations like CenterX and Left and Right.
     * If client set such combination then LayoutBox discards ambigious 
     * element one. For example if client set Left then Right then CenterX 
     * LayoutBox discard Right and only Left and CenterX are left.
     * 
     * LayoutBox also calculates Preferred bounding box sizes for given 
     * parameters. When doing so it takes into ab
     * 
     **/
    public class LayoutBox : ILayoutBox
    {
        RectangleF outerBounds;
        IUnit unit = IdentityUnit.Identity;

        //These variables control horizontal dimensions
        public float Left { get; set; }
        public float Right{ get; set; }
        public float Width{ get; set; }
        public float CenterX{ get; set; }
        //These variables control vertical dimensions
        public float Top { get; set; }
        public float Bottom { get; set; } 
        public float Height { get; set; } 
        public float CenterY { get; set; }

        const float NOT_SET = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Qoden.UI.LayoutBox"/> class.
        /// </summary>
        /// <param name="outerBounds">Layout bounds in view coordinate system in pixels</param>
        public LayoutBox(RectangleF outerBounds) : this(outerBounds, IdentityUnit.Identity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Qoden.UI.LayoutBox"/> class.
        /// </summary>
        /// <param name="outerBounds">Layout bounds in view coordinate system in pixels</param>
        /// <param name="unit">Unit to be used for relative offsets</param>
        public LayoutBox(RectangleF outerBounds, IUnit unit)
        {
            this.outerBounds = outerBounds;
            Left = Right = Top = Bottom = Width = Height = CenterX = CenterY = NOT_SET;
            this.unit = unit ?? IdentityUnit.Identity;
        }

        /// <summary>
        /// Measurement unit for relative offsets
        /// </summary>
        public IUnit Unit => unit;

        public static bool IsSet(float val)
        {
            return Math.Abs(val - NOT_SET) > float.Epsilon;
        }

        public RectangleF OuterBounds => outerBounds;
    }
}

