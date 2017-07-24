using System;
using System.Drawing;

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

        public IViewLayoutBox AutoWidth()
        {
            EnsureMeasured();
            this.Width(MeasuredSize.Width);
            return this;
        }

        public IViewLayoutBox AutoHeight()
        {
            EnsureMeasured();
            this.Height(MeasuredSize.Height);
            return this;
        }

        public IViewLayoutBox AutoSize()
        {
            EnsureMeasured();
            return AutoWidth().AutoHeight();
        }

        void EnsureMeasured()
        {
            if (!_measuredSize.HasValue)
            {
                MeasuredSize = View.SizeThatFits(Bounds.Size);
            }
        }

        public abstract void Layout();
        public abstract IViewGeometry View { get; }
    }
}
