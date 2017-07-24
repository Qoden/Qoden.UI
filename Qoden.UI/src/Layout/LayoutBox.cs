using System;
using System.Drawing;
#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    /// <summary>
    /// Contains layout parameters relative to provided rectangle.
    /// </summary>
    public interface ILayoutBox 
    {
        /// <summary>
        /// Measurement unit for relative offsets
        /// </summary>
        IUnit Unit { get; }

        void SetLeft(Pixel l);

        void SetLeft(float l);

        void SetRight(Pixel r);

        void SetRight(float r);

        void SetTop(Pixel t);

        void SetTop(float t);

        void SetBottom(Pixel b);

        void SetBottom(float b);

        void SetWidth(Pixel w);

        void SetWidth(float w);

        void SetHeight(Pixel h);

        void SetHeight(float h);

        void SetCenterX(Pixel cx);

        void SetCenterX(float cx);

        void SetCenterY(Pixel cy);

        void SetCenterY(float cy);

        /// <summary>
        /// Caculated layout width in pixels
        /// </summary>
        float LayoutWidth { get; }

        /// <summary>
        /// Calculated layout left position in pixels
        /// </summary>
        float LayoutHeight { get; }

        /// <summary>
        /// Calculated layout right position in pixels
        /// </summary>
        float LayoutLeft { get; }

        /// <summary>
        /// Calculated layout right position in pixels
        /// </summary>
        float LayoutRight { get; }

        /// <summary>
        /// Caculated layout top position in pixels
        /// </summary>
        float LayoutTop { get; }

        /// <summary>
        /// Calculated layout rectable bottom position in pixels
        /// </summary>
        float LayoutBottom { get; }

        /// <summary>
        /// Caclulated layout center in pixels
        /// </summary>
        PointF LayoutCenter { get; }

        /// <summary>
        /// Caclualted layout size in pixels
        /// </summary>
        SizeF LayoutSize { get; }

        /// <summary>
        /// Calculated layout bounds in pixels
        /// </summary>
        RectangleF LayoutBounds { get; }

        /// <summary>
        /// Outer bounds in pixels
        /// </summary>
        RectangleF Bounds { get; }
    }

    public class LayoutBox : ILayoutBox
    {
        RectangleF bounds;
        IUnit unit = IdentityUnit.Identity;

        float left, right, top, bottom, width, height, centerX, centerY;
        const float NOT_SET = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Qoden.UI.LayoutBox"/> class.
        /// </summary>
        /// <param name="outerBounds">Layout bounds in pixels</param>
        public LayoutBox(RectangleF outerBounds) : this(outerBounds, IdentityUnit.Identity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Qoden.UI.LayoutBox"/> class.
        /// </summary>
        /// <param name="outerBounds">Layout bounds in pixels</param>
        /// <param name="unit">Unit to be used for relative offsets</param>
        public LayoutBox(RectangleF outerBounds, IUnit unit)
        {
            this.bounds = outerBounds;
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
                width = NOT_SET;
            left = l.Value;
        }

        public void SetLeft(float l)
        {
            SetLeft(unit.ToPixels(l));
        }

        public void SetRight(Pixel r)
        {
            if (IsSet(centerX) && IsSet(width))
                centerX = NOT_SET;
            if (IsSet(left) && IsSet(width))
                width = NOT_SET;
            right = r.Value;
        }

        public void SetRight(float r)
        {
            SetRight(unit.ToPixels(r));
        }

        public void SetTop(Pixel t)
        {
            if (IsSet(centerY) && IsSet(height))
                centerY = NOT_SET;
            if (IsSet(bottom) && IsSet(height))
                height = NOT_SET;
            top = t.Value;
        }

        public void SetTop(float t)
        {
            SetTop(unit.ToPixels(t));
        }

        public void SetBottom(Pixel b)
        {
            if (IsSet(centerY) && IsSet(height))
                centerY = NOT_SET;
            if (IsSet(top) && IsSet(height))
                height = NOT_SET;
            bottom = b.Value;
        }

        public void SetBottom(float b)
        {
            SetBottom(unit.ToPixels(b));
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

        public void SetWidth(float w)
        {
            SetWidth(unit.ToPixels(w));
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

        public void SetHeight(float h)
        {
            SetHeight(unit.ToPixels(h));
        }

        public void SetCenterX(Pixel cx)
        {
            if (IsSet(left) && IsSet(right))
            {
                width = right - left;
                left = right = NOT_SET;
            }
            if (IsSet(left) && IsSet(width))
                width = NOT_SET;
            if (IsSet(right) && IsSet(width))
                width = NOT_SET;
            centerX = cx.Value;
        }

        public void SetCenterX(float cx)
        {
            SetCenterX(unit.ToPixels(cx));
        }

        public void SetCenterY(Pixel cy)
        {
            if (IsSet(top) && IsSet(bottom))
            {
                height = bounds.Height - bottom - top;
                top = bottom = NOT_SET;
            }
            if (IsSet(top) && IsSet(height))
                height = NOT_SET;
            if (IsSet(bottom) && IsSet(height))
                height = NOT_SET;

            centerY = cy.Value;
        }

        public void SetCenterY(float cy)
        {
            SetCenterY(unit.ToPixels(cy));
        }

        /// <summary>
        /// Caculated layout width in pixels
        /// </summary>
        public float LayoutWidth
        {
            get
            {
                if (IsSet(width))
                    return width;
                if (IsSet(left) && IsSet(right))
                    return bounds.Width - right - left;
                if (IsSet(centerX) && IsSet(left))
                    return (centerX - left) * 2;
                if (IsSet(centerX) && IsSet(right))
                    return (centerX - right) * 2;
                return 0;
            }
        }

        /// <summary>
        /// Calculated layout left position in pixels
        /// </summary>
        public float LayoutHeight
        {
            get
            {
                if (IsSet(height))
                    return height;
                if (IsSet(top) && IsSet(bottom))
                    return bounds.Height - bottom - top;
                if (IsSet(centerY) && IsSet(top))
                    return (centerY - top) * 2;
                if (IsSet(centerY) && IsSet(bottom))
                    return (centerY - bottom) * 2;
                return 0;
            }
        }

        /// <summary>
        /// Calculated layout right position in pixels
        /// </summary>
        public float LayoutLeft
        {
            get
            {
                if (IsSet(left))
                    return bounds.Left + left;
                if (IsSet(centerX) && IsSet(width))
                    return bounds.Left + centerX - width / 2;
                if (IsSet(right) && IsSet(width))
                    return bounds.Right - right - width;
                return 0;
            }
        }

        /// <summary>
        /// Calculated layout right position in pixels
        /// </summary>
        public float LayoutRight
        {
            get
            {
                if (IsSet(right))
                    return bounds.Right - right;
                if (IsSet(centerX) && IsSet(width))
                    return bounds.Left + centerX + width / 2;
                if (IsSet(left) && IsSet(width))
                    return bounds.Left + left + width;
                return 0;
            }
        }

        /// <summary>
        /// Caculated layout top position in pixels
        /// </summary>
        public float LayoutTop
        {
            get
            {
                if (IsSet(top))
                    return bounds.Top + top;
                if (IsSet(centerY) && IsSet(height))
                    return bounds.Top + centerY - height / 2;
                if (IsSet(bottom) && IsSet(height))
                    return bounds.Bottom - bottom - height;
                return 0;
            }
        }

        /// <summary>
        /// Calculated layout rectable bottom position in pixels
        /// </summary>
        public float LayoutBottom
        {
            get
            {
                if (IsSet(bottom))
                    return bounds.Bottom - bottom;
                if (IsSet(centerY) && IsSet(height))
                    return bounds.Top + centerY + height / 2;
                if (IsSet(top) && IsSet(height))
                {
                    return bounds.Top + top + height;
                }
                return NOT_SET;
            }
        }

        /// <summary>
        /// Caclulated layout center in pixels
        /// </summary>
        public PointF LayoutCenter
        {
            get { return new PointF(LayoutLeft + LayoutWidth / 2, LayoutTop + LayoutHeight / 2); }
        }

        /// <summary>
        /// Caclualted layout size in pixels
        /// </summary>
        public SizeF LayoutSize
        {
            get { return new SizeF(LayoutWidth, LayoutHeight); }
        }

        /// <summary>
        /// Calculated layout bounds in pixels
        /// </summary>
        public RectangleF LayoutBounds
        {
            get
            {
                return new RectangleF(LayoutLeft, LayoutTop, LayoutWidth, LayoutHeight);
            }
        }

        /// <summary>
        /// Outer bounds in pixels
        /// </summary>
        public RectangleF Bounds => bounds;
    }

    public static class LayoutBoxCenter
    {
        public static T CenterHorizontally<T>(this T box, RectangleF view, Pixel dx) where T : ILayoutBox
        {
            box.SetCenterX(Pixel.Val(view.Left + view.Width / 2 + dx.Value));
            return box;
        }

        public static T CenterHorizontally<T>(this T box, RectangleF view, float dx = 0) where T : ILayoutBox
        {
            return box.CenterHorizontally(view, box.Unit.ToPixels(dx));
        }

        public static T CenterHorizontally<T>(this T box, float dx = 0) where T : ILayoutBox
        {
            return box.CenterHorizontally(box.Bounds, box.Unit.ToPixels(dx));
        }

        public static T CenterHorizontally<T>(this T box, Pixel dx) where T : ILayoutBox
        {
            return box.CenterHorizontally(box.Bounds, dx);
        }

        public static T CenterVertically<T>(this T box, RectangleF view, Pixel dx) where T : ILayoutBox
        {
            box.SetCenterY(Pixel.Val(view.Top + view.Height / 2 + dx.Value));
            return box;
        }

        public static T CenterVertically<T>(this T box, RectangleF view, float dx = 0) where T : ILayoutBox
        {
            return box.CenterVertically(view, box.Unit.ToPixels(dx));
        }

        public static T CenterVertically<T>(this T box, float dx = 0) where T : ILayoutBox
        {
            return box.CenterVertically(box.Bounds, box.Unit.ToPixels(dx));
        }

        public static T CenterVertically<T>(this T box, Pixel dx) where T : ILayoutBox
        {
            return box.CenterVertically(box.Bounds, dx);
        }
    }

    public static class LayoutBoxRectangle
    {
        public static T Width<T>(this T box, float w) where T : ILayoutBox
        {
            box.SetWidth(w);
            return box;
        }

        public static T Width<T>(this T box, Pixel w) where T : ILayoutBox
        {
            box.SetWidth(w);
            return box;
        }

        public static T Height<T>(this T box, float h) where T : ILayoutBox
        {
            box.SetHeight(h);
            return box;
        }

        public static T Height<T>(this T box, Pixel h) where T : ILayoutBox
        {
            box.SetHeight(h);
            return box;
        }

        public static T Left<T>(this T box, float l) where T : ILayoutBox
        {
            box.SetLeft(l);
            return box;
        }

        public static T Left<T>(this T box, Pixel l) where T : ILayoutBox
        {
            box.SetLeft(l);
            return box;
        }

        public static T Right<T>(this T box, float r) where T : ILayoutBox
        {
            box.SetRight(r);
            return box;
        }

        public static T Right<T>(this T box, Pixel r) where T : ILayoutBox
        {
            box.SetRight(r);
            return box;
        }

        public static T Top<T>(this T box, float t) where T : ILayoutBox
        {
            box.SetTop(t);
            return box;
        }

        public static T Top<T>(this T box, Pixel t) where T : ILayoutBox
        {
            box.SetTop(t);
            return box;
        }

        public static T Bottom<T>(this T box, float b) where T : ILayoutBox
        {
            box.SetBottom(b);
            return box;
        }

        public static T Bottom<T>(this T box, Pixel b) where T : ILayoutBox
        {
            box.SetBottom(b);
            return box;
        }
    }

    public static class LayoutBoxRelative
    {
        public static T Before<T>(this T box, RectangleF reference, Pixel dx) where T : ILayoutBox
        {
            box.SetRight(Pixel.Val(box.Bounds.Width - reference.Left + dx.Value));
            return box;
        }

        public static T Before<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            return box.Before(reference, box.Unit.ToPixels(dx));
        }

        public static T After<T>(this T box, RectangleF reference, Pixel dx) where T : ILayoutBox
        {
            box.SetLeft(Pixel.Val(reference.Right + dx.Value));
            return box;
        }

        public static T After<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            return box.After(reference, box.Unit.ToPixels(dx));
        }

        public static T Below<T>(this T box, RectangleF reference, Pixel dx) where T : ILayoutBox
        {
            box.SetTop(Pixel.Val(reference.Bottom + dx.Value));
            return box;
        }

        public static T Below<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            return box.Below(reference, box.Unit.ToPixels(dx));
        }

        public static T Above<T>(this T box, RectangleF reference, Pixel dx) where T : ILayoutBox
        {
            box.SetBottom(Pixel.Val((box.Bounds.Height - reference.Top) + dx.Value));
            return box;
        }

        public static T Above<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            return box.Above(reference, box.Unit.ToPixels(dx));
        }
    }
}

