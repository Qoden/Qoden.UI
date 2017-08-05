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

        public void SetLeft(Pixel l)
        {
            Left = l.Value;
        }

        public void SetRight(Pixel r)
        {
            Right = r.Value;
        }

        public void SetTop(Pixel t)
        {
            Top = t.Value;
        }

        public void SetBottom(Pixel b)
        {
            Bottom = b.Value;
        }

        public void SetWidth(Pixel w)
        {
            Width = w.Value;
        }

        public void SetHeight(Pixel h)
        {
            Height = h.Value;
        }

        public void SetCenterX(Pixel cx)
        {
            CenterX = cx.Value;
        }

        public void SetCenterY(Pixel cy)
        {
            CenterY = cy.Value;
        }

        public float LayoutWidth
        {
            get
            {
                if (IsSet(Width))
                    return Width;
                if (IsSet(CenterX) && IsSet(Left))
                    return (CenterX - Left) * 2;
                if (IsSet(CenterX) && IsSet(Right))
                    return (outerBounds.Width - Right - CenterX) * 2;
                if (IsSet(Left) && IsSet(Right))
                    return outerBounds.Width - Right - Left;
                return outerBounds.Width;
            }
        }

        public float LayoutHeight
        {
            get
            {
                if (IsSet(Height))
                    return Height;
                if (IsSet(Top) && IsSet(Bottom))
                    return outerBounds.Height - Bottom - Top;
                if (IsSet(CenterY) && IsSet(Top))
                    return (CenterY - Top) * 2;
                if (IsSet(CenterY) && IsSet(Bottom))
                    return (outerBounds.Height - Bottom - CenterY) * 2;
                return outerBounds.Height;
            }
        }

        public float LayoutLeft
        {
            get
            {
                if (IsSet(Left))
                    return outerBounds.Left + Left;
                if (IsSet(CenterX) && IsSet(Width))
                    return outerBounds.Left + CenterX - Width / 2;
                if (IsSet(Right) && IsSet(Width))
                    return outerBounds.Right - Right - Width;
                return outerBounds.Left;
            }
        }

        public float LayoutRight
        {
            get
            {
                if (IsSet(Right))
                    return outerBounds.Right - Right;
                if (IsSet(Left) && IsSet(Width))
                    return outerBounds.Left + Left + Width;
                if (IsSet(CenterX) && IsSet(Width))
                    return outerBounds.Left + CenterX + Width / 2;
                return outerBounds.Right;
            }
        }

        public float LayoutTop
        {
            get
            {
                if (IsSet(Top))
                    return outerBounds.Top + Top;
                if (IsSet(CenterY) && IsSet(Height))
                    return outerBounds.Top + CenterY - Height / 2;
                if (IsSet(Bottom) && IsSet(Height))
                    return outerBounds.Bottom - Bottom - Height;
                return outerBounds.Top;
            }
        }

        public float LayoutBottom
        {
            get
            {
                if (IsSet(Bottom))
                    return outerBounds.Bottom - Bottom;
                if (IsSet(Top) && IsSet(Height))
                    return outerBounds.Top + Top + Height;
                if (IsSet(CenterY) && IsSet(Height))
                    return outerBounds.Top + CenterY + Height / 2;
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

        public float PreferredBoundingWidth
        {
            get
            {
                if (IsSet(Left) && IsSet(Right) && IsSet(Width))
                {
                    return Left + Right + Width;
                }

                if (IsSet(Left) && IsSet(CenterX) && IsSet(Right))
                {
                    return Left + (CenterX - Left) * 2 + Right;
                }

                if (IsSet(Left) && IsSet(Width))
                {
                    return Left + Width;
                }

                if (IsSet(Left) && IsSet(CenterX))
                {
                    return Left + (CenterX - Left) * 2;
                }

                if (IsSet(Right) && IsSet(Width))
                {
                    return Right + Width;
                }

                if (IsSet(Width) && IsSet(CenterX))
                    return CenterX + Width/2;

                if (IsSet(Width))
                    return Width;

                return LayoutWidth;
            }
        }

        public float PreferredBoundingHeight
        {
            get
            {
                if (IsSet(Top) && IsSet(Bottom) && IsSet(Height))
                {
                    return Top + Bottom + Height;
                }

                if (IsSet(Top) && IsSet(CenterY) && IsSet(Bottom))
                {
                    return Top + (CenterY - Top) * 2 + Bottom;
                }

                if (IsSet(Top) && IsSet(Height))
                {
                    return Top + Height;
                }

                if (IsSet(Top) && IsSet(CenterY))
                {
                    return Top + (CenterY - Top) * 2;
                }

                if (IsSet(Bottom) && IsSet(Height))
                {
                    return Bottom + Height;
                }

                if (IsSet(Height) && IsSet(CenterY))
                    return CenterY + Height / 2;

                if (IsSet(Height))
                    return Height;

                return LayoutHeight;
            }
        }

        public SizeF PreferredBoundingSize
        {
            get
            {
                return new SizeF(PreferredBoundingWidth, PreferredBoundingHeight);
            }
        }

        public RectangleF OuterBounds => outerBounds;
    }
}

