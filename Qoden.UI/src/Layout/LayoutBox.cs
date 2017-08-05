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
     * There are also many ambigious combinations like CenterX and Left and Right.
     * If client set such ambigious combination then LayoutBox discard ambigious 
     * one. For example if client set Left then Right then CenterX LayoutBox 
     * discard Right and only Left and CenterX are left.
     * 
     * LayutBox also provides two sets of resulting dimensions.
     * a) LayoutXYZ (LayoutWidth, LayoutLeft, LayoutRight and so on)
     * b) PreferredXYZ (PreferredWidth, PreferredLeft, PreferredRight and so on)
     *      
     * LayoutXYZ family of properties describes rectangle inside provided 
     * OuterBounds. When calculated these properties prefer to use relative 
     * measures if possible. For instance if Left, Right and Width available 
     * then Layout property will choose Left and Right and won't use Width.
     * Of course if only Left and Width available then Layout will use it.
     * 
     * PreferredXYZ familty of properties uses different strategy. They use 
     * Width/Height when they are available. For example if Left, Right and 
     * Width available Preferred property will choose Left and Width.
     * 
     **/
    public class LayoutBox : ILayoutBox
    {
        RectangleF outerBounds;
        IUnit unit = IdentityUnit.Identity;

        //These variables control horizontal dimensions
        float left, right, width, centerX;
        //These variables control vertical dimensions
        float top, bottom, height, centerY;
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
            left = right = top = bottom = width = height = centerX = centerY = NOT_SET;
            this.unit = unit ?? IdentityUnit.Identity;
        }

        /// <summary>
        /// Measurement unit for relative offsets
        /// </summary>
        public IUnit Unit => unit;

        static bool IsSet(float val)
        {
            return Math.Abs(val - NOT_SET) > float.Epsilon;
        }

        public void SetLeft(Pixel l)
        {
            if (IsSet(centerX) && IsSet(width))
                centerX = NOT_SET;
            if (IsSet(right) && IsSet(width))
                right = NOT_SET;
            left = l.Value;
        }

        public void SetRight(Pixel r)
        {
            if (IsSet(centerX) && IsSet(width))
                centerX = NOT_SET;
            if (IsSet(left) && IsSet(width))
                width = NOT_SET;
            right = r.Value;
        }

        public void SetTop(Pixel t)
        {
            if (IsSet(centerY) && IsSet(height))
                centerY = NOT_SET;
            if (IsSet(bottom) && IsSet(height))
                height = NOT_SET;
            top = t.Value;
        }

        public void SetBottom(Pixel b)
        {
            if (IsSet(centerY) && IsSet(height))
                centerY = NOT_SET;
            if (IsSet(top) && IsSet(height))
                height = NOT_SET;
            bottom = b.Value;
        }

        public void SetWidth(Pixel w)
        {
            if (IsSet(left) && IsSet(right))
            {
                left = NOT_SET;
            }
            if (IsSet(centerX) && IsSet(left))
                centerX = NOT_SET;
            if (IsSet(centerX) && IsSet(right))
                centerX = NOT_SET;
            width = w.Value;
        }

        public void SetHeight(Pixel h)
        {
            if (IsSet(top) && IsSet(bottom))
            {
                top = NOT_SET;
            }
            if (IsSet(centerY) && IsSet(top))
                centerY = NOT_SET;
            if (IsSet(centerY) && IsSet(bottom))
                centerY = NOT_SET;
            height = h.Value;
        }

        public void SetCenterX(Pixel cx)
        {
            if (IsSet(left) && IsSet(right))
                right = NOT_SET;
            if (IsSet(left) && IsSet(width))
                left = NOT_SET;
            if (IsSet(right) && IsSet(width))
                right = NOT_SET;
            centerX = cx.Value;
        }

        public void SetCenterY(Pixel cy)
        {
            if (IsSet(top) && IsSet(bottom))
                bottom = NOT_SET;
            if (IsSet(top) && IsSet(height))
                top = NOT_SET;
            if (IsSet(bottom) && IsSet(height))
                bottom = NOT_SET;

            centerY = cy.Value;
        }

        public float LayoutWidth
        {
            get
            {
                if (IsSet(width))
                    return width;
                if (IsSet(left) && IsSet(right))
                    return outerBounds.Width - right - left;
                if (IsSet(centerX) && IsSet(left))
                    return (centerX - left) * 2;
                if (IsSet(centerX) && IsSet(right))
                    return (centerX - right) * 2;
                return outerBounds.Width;
            }
        }

        public float LayoutHeight
        {
            get
            {
                if (IsSet(height))
                    return height;
                if (IsSet(top) && IsSet(bottom))
                    return outerBounds.Height - bottom - top;
                if (IsSet(centerY) && IsSet(top))
                    return (centerY - top) * 2;
                if (IsSet(centerY) && IsSet(bottom))
                    return (centerY - bottom) * 2;
                return outerBounds.Height;
            }
        }

        public float LayoutLeft
        {
            get
            {
                if (IsSet(left))
                    return outerBounds.Left + left;
                if (IsSet(centerX) && IsSet(width))
                    return outerBounds.Left + centerX - width / 2;
                if (IsSet(right) && IsSet(width))
                    return outerBounds.Right - right - width;
                return outerBounds.Left;
            }
        }

        public float LayoutRight
        {
            get
            {
                if (IsSet(right))
                    return outerBounds.Right - right;
                if (IsSet(centerX) && IsSet(width))
                    return outerBounds.Left + centerX + width / 2;
                if (IsSet(left) && IsSet(width))
                    return outerBounds.Left + left + width;
                return outerBounds.Right;
            }
        }

        public float LayoutTop
        {
            get
            {
                if (IsSet(top))
                    return outerBounds.Top + top;
                if (IsSet(centerY) && IsSet(height))
                    return outerBounds.Top + centerY - height / 2;
                if (IsSet(bottom) && IsSet(height))
                    return outerBounds.Bottom - bottom - height;
                return outerBounds.Top;
            }
        }

        public float LayoutBottom
        {
            get
            {
                if (IsSet(bottom))
                    return outerBounds.Bottom - bottom;
                if (IsSet(centerY) && IsSet(height))
                    return outerBounds.Top + centerY + height / 2;
                if (IsSet(top) && IsSet(height))
                {
                    return outerBounds.Top + top + height;
                }
                return outerBounds.Bottom;
            }
        }

        public PointF LayoutCenter
        {
            get { return new PointF(LayoutLeft + LayoutWidth / 2, LayoutTop + LayoutHeight / 2); }
        }

        public SizeF LayoutSize
        {
            get { return new SizeF(LayoutWidth, LayoutHeight); }
        }

        public RectangleF LayoutBounds
        {
            get
            {
                return new RectangleF(LayoutLeft, LayoutTop, LayoutWidth, LayoutHeight);
            }
        }

        public RectangleF OuterBounds => outerBounds;
    }
}

