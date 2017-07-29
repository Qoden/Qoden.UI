using System;
using System.Drawing;
using System.Reflection;

namespace Qoden.UI
{
    public abstract class ViewLayoutBox : LayoutBox, IViewLayoutBox
    {
        SizeF? _measuredSize;

        public ViewLayoutBox(RectangleF r, IUnit unit) : base(r, unit)
        {            
        }

        public override string ToString()
        {
            return string.Format("[ViewLayoutBox: {0}, {1}, {2}, {3}]", LayoutLeft, LayoutTop, LayoutWidth, LayoutHeight);
        }

        public SizeF MeasuredSize
        {
            get => _measuredSize.GetValueOrDefault();
            private set
            {
                _measuredSize = value;
            }
        }

        private SizeF BoundingSize(float? maxWidth = null, float? maxHeight = null)
        {
            float w = maxWidth.GetValueOrDefault(Bounds.Width);
            float h = maxHeight.GetValueOrDefault(Bounds.Height);
            return new SizeF(Unit.ToPixels(w).Value, Unit.ToPixels(h).Value);
        }

        public IViewLayoutBox AutoWidth(float? maxWidth = null)
        {
            var size = BoundingSize(maxWidth);
            MeasuredSize = View.SizeThatFits(size);
            this.Width(MeasuredSize.Width);
            return this;
        }

        public IViewLayoutBox AutoHeight(float? maxHeight = null)
        {
            var size = BoundingSize(null, maxHeight);
            MeasuredSize = View.SizeThatFits(size);
            this.Height(MeasuredSize.Height);
            return this;
        }

        public IViewLayoutBox AutoSize(float? maxWidth = null, float? maxHeight = null)
        {
            var size = BoundingSize(maxWidth, maxHeight);
            MeasuredSize = View.SizeThatFits(size);
            this.Width(MeasuredSize.Width);
            this.Height(MeasuredSize.Height);
            return this;
        }

        public abstract void Layout();
        public abstract IViewGeometry View { get; }
    }
}
