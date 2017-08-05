using System;
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
        /// Set distance from <see cref="OuterBounds"/> left edge to view left edge.
        /// </summary>
        /// <param name="l">Distance in <see cref="Unit"/></param>
        void SetLeft(float l);

        /// <summary>
        /// Set distance from <see cref="OuterBounds"/> right edge to view right edge.
        /// </summary>
        /// <param name="r">Distance in pixels</param>
        void SetRight(Pixel r);
        /// <summary>
        /// Set distance from <see cref="OuterBounds"/> right edge to view right edge.
        /// </summary>
        /// <param name="r">Distance in <see cref="Unit"/></param>
        void SetRight(float r);

        /// <summary>
        /// Set distance from <see cref="OuterBounds"/> top edge to view top edge.
        /// </summary>
        /// <param name="t">Distance in pixels</param>
        void SetTop(Pixel t);
        /// <summary>
        /// Set distance from <see cref="OuterBounds"/> top edge to view top edge.
        /// </summary>
        /// <param name="t">Distance in <see cref="Unit"/></param>
        void SetTop(float t);

        /// <summary>
        /// Set distance from <see cref="OuterBounds"/> bottom edge to view bottom edge.
        /// </summary>
        /// <param name="b">Distance in pixels</param>
        void SetBottom(Pixel b);
        /// <summary>
        /// Set distance from <see cref="OuterBounds"/> bottom edge to view bottom edge.
        /// </summary>
        /// <param name="b">Distance in <see cref="Unit"/></param>
        void SetBottom(float b);
        /// <summary>
        /// Set box width.
        /// </summary>
        /// <param name="w">Width in pixels</param>
        void SetWidth(Pixel w);
        /// <summary>
        /// Set box width.
        /// </summary>
        /// <param name="w">Width in <see cref="Unit"/></param>
        void SetWidth(float w);
        /// <summary>
        /// Set box height.
        /// </summary>
        /// <param name="h">Height in pixels</param>
        void SetHeight(Pixel h);
        /// <summary>
        /// Set box height.
        /// </summary>
        /// <param name="h">Height in <see cref="Unit"/></param>
        void SetHeight(float h);
        /// <summary>
        /// Set box center X position relative to <see cref="OuterBounds"/>.
        /// </summary>
        /// <param name="cx">Center x position in pixels</param>
        void SetCenterX(Pixel cx);
        /// <summary>
        /// Set box center X position relative to <see cref="OuterBounds"/>.
        /// </summary>
        /// <param name="cx">Center x position in <see cref="Unit"/></param>
        void SetCenterX(float cx);
        /// <summary>
        /// Set box center Y position relative to <see cref="OuterBounds"/>.
        /// </summary>
        /// <param name="cy">Center y position in pixels</param>
        void SetCenterY(Pixel cy);
        /// <summary>
        /// Set box center Y position relative to <see cref="OuterBounds"/>.
        /// </summary>
        /// <param name="cy">Center y position in <see cref="Unit"/></param>
        void SetCenterY(float cy);
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
    }

    public class LayoutBox : ILayoutBox
    {
        RectangleF outerBounds;
        IUnit unit = IdentityUnit.Identity;

        float left, right, top, bottom, width, height, centerX, centerY;
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
                right = NOT_SET;
            if (IsSet(left) && IsSet(width))
                left = NOT_SET;
            if (IsSet(right) && IsSet(width))
                right = NOT_SET;
            centerX = cx.Value;
        }

        public void SetCenterX(float cx)
        {
            SetCenterX(unit.ToPixels(cx));
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

        public void SetCenterY(float cy)
        {
            SetCenterY(unit.ToPixels(cy));
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

        public float Left
        {
            get
            {
                if (IsSet(left))
                    return left;
                if (IsSet(centerX) && IsSet(width))
                    return centerX - width / 2;
                return NOT_SET;
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

    public static class LayoutBoxCenter
    {
        public static T CenterHorizontally<T>(this T box, Pixel dx) where T : ILayoutBox
        {
            box.SetCenterX(Pixel.Val(box.OuterBounds.Left + box.OuterBounds.Width / 2 + dx.Value));
            return box;
        }

        public static T CenterHorizontally<T>(this T box, float dx = 0) where T : ILayoutBox
        {
            return box.CenterHorizontally(box.Unit.ToPixels(dx));
        }

        public static T CenterVertically<T>(this T box, Pixel dx) where T : ILayoutBox
        {
            box.SetCenterY(Pixel.Val(box.OuterBounds.Top + box.OuterBounds.Height / 2 + dx.Value));
            return box;
        }

        public static T CenterVertically<T>(this T box, float dx = 0) where T : ILayoutBox
        {
            return box.CenterVertically(box.Unit.ToPixels(dx));
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
        /// <summary>
        /// Place this box before provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Before<T>(this T box, RectangleF reference, Pixel dx) where T : ILayoutBox
        {
            var referenceOffset = box.OuterBounds.Right - reference.Left;
            box.SetRight(Pixel.Val(referenceOffset + dx.Value));
            return box;
        }

        /// <summary>
        /// Place this box before provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Before<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            return box.Before(reference, box.Unit.ToPixels(dx));
        }

        /// <summary>
        /// Place this box after provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T After<T>(this T box, RectangleF reference, Pixel dx) where T : ILayoutBox
        {
            var referenceOffset = reference.Right - box.OuterBounds.Left;
            box.SetLeft(Pixel.Val(referenceOffset + dx.Value));
            return box;
        }
        /// <summary>
        /// Place this box after provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T After<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            return box.After(reference, box.Unit.ToPixels(dx));
        }

        /// <summary>
        /// Place this box below provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Below<T>(this T box, RectangleF reference, Pixel dx) where T : ILayoutBox
        {
            var referenceOffset = reference.Bottom - box.OuterBounds.Top;
            box.SetTop(Pixel.Val(referenceOffset + dx.Value));
            return box;
        }

        /// <summary>
        /// Place this box below provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Below<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            return box.Below(reference, box.Unit.ToPixels(dx));
        }
        /// <summary>
        /// Place this box above provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Above<T>(this T box, RectangleF reference, Pixel dx) where T : ILayoutBox
        {
            var referenceOffset = box.OuterBounds.Bottom - reference.Top;
            box.SetBottom(Pixel.Val(referenceOffset + dx.Value));
            return box;
        }
        /// <summary>
        /// Place this box above provided reference box. Reference is in view coordinate system.
        /// </summary>
        public static T Above<T>(this T box, RectangleF reference, float dx = 0) where T : ILayoutBox
        {
            return box.Above(reference, box.Unit.ToPixels(dx));
        }
    }

    public static class LayoutBoxMinMax
    {
        public static T MinWidth<T>(this T box, float mw) where T : ILayoutBox
        {
            return box.MinWidth(box.Unit.ToPixels(mw));
        }

        public static T MinHeight<T>(this T box, float mh) where T : ILayoutBox
        {
            return box.MinHeight(box.Unit.ToPixels(mh));
        }

        public static T MinWidth<T>(this T box, Pixel mw) where T : ILayoutBox
        {
            if (box.LayoutWidth < mw.Value)
            {
                box.SetWidth(mw);
            }
            return box;
        }

        public static T MinHeight<T>(this T box, Pixel mh) where T : ILayoutBox
        {
            if (box.LayoutHeight < mh.Value)
            {
                box.SetHeight(mh);
            }

            return box;
        }

        public static T MaxWidth<T>(this T box, float mw) where T : ILayoutBox
        {
            return box.MaxWidth(box.Unit.ToPixels(mw));
        }

        public static T MaxHeight<T>(this T box, float mh) where T : ILayoutBox
        {
            return box.MaxHeight(box.Unit.ToPixels(mh));
        }

        public static T MaxWidth<T>(this T box, Pixel mw) where T : ILayoutBox
        {
            if (box.LayoutWidth > mw.Value)
            {
                box.SetWidth(mw);
            }
            return box;
        }

        public static T MaxHeight<T>(this T box, Pixel mh) where T : ILayoutBox
        {
            if (box.LayoutHeight > mh.Value)
            {
                box.SetHeight(mh);
            }

            return box;
        }
    }
}

