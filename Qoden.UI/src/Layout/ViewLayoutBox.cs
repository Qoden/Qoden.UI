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
            return string.Format("[ViewLayoutBox: {0}, {1}, {2}, {3}]", FrameLeft, FrameTop, FrameWidth, FrameHeight);
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
            var w = maxWidth.HasValue ? Unit.ToPixels(maxWidth.Value).Value : OuterBounds.Width;
            var h = maxHeight.HasValue ? Unit.ToPixels(maxHeight.Value).Value : OuterBounds.Height;
            return new SizeF(w, h);
        }

        public IViewLayoutBox AutoWidth(float? maxWidth = null)
        {
            var size = BoundingSize(maxWidth);
            MeasuredSize = View.SizeThatFits(size);
            this.Width(Pixel.Val(MeasuredSize.Width));
            return this;
        }

        public IViewLayoutBox AutoHeight(float? maxHeight = null)
        {
            var size = BoundingSize(null, maxHeight);
            MeasuredSize = View.SizeThatFits(size);
            this.Height(Pixel.Val(MeasuredSize.Height));
            return this;
        }

        public IViewLayoutBox AutoSize(float? maxWidth = null, float? maxHeight = null)
        {
            var size = BoundingSize(maxWidth, maxHeight);
            MeasuredSize = View.SizeThatFits(size);
            this.Width(Pixel.Val(MeasuredSize.Width));
            this.Height(Pixel.Val(MeasuredSize.Height));
            return this;
        }

        public abstract void Layout();

        public abstract IViewGeometry View { get;  }
    }

    public class SpacerLayoutBox : ViewLayoutBox
    {
        private SpacerGeometry _spacer;

        public SpacerLayoutBox(SpacerGeometry geometry, RectangleF r, IUnit unit) : base(r, unit)
        {
            _spacer = geometry;
        }

        public SpacerLayoutBox(RectangleF r, IUnit unit) : base(r, unit)
        {
            _spacer = new SpacerGeometry();
        }

        public override IViewGeometry View => _spacer;

        public override void Layout()
        {
        }
    }

    public class SpacerGeometry : IViewGeometry
    {
        public RectangleF Frame
        {
            get;
            set;
        }

        public IViewLayoutBox MakeViewLayoutBox(RectangleF bounds, IUnit unit = null)
        {
            return new SpacerLayoutBox(this, bounds, unit);
        }

        public SizeF SizeThatFits(SizeF bounds)
        {
            return Frame.Size;
        }
    }
}
