using System;
using System.Collections.Generic;
using System.Drawing;

namespace Qoden.UI
{
    public class LayoutBuilder
    {
        private List<IViewLayoutBox> _boxes = new List<IViewLayoutBox>();

        public LayoutBuilder(RectangleF bounds)
        {
            Bounds = bounds;
        }

        public RectangleF Bounds { get; private set; }

        public EdgeInset Padding { get; set; }

        public IViewLayoutBox View(IViewGeometry v, RectangleF? bounds = null, EdgeInset? padding = null, IUnit units = null)
        {
            var b = bounds ?? Bounds;
            var p = padding ?? Padding;
            var layoutBounds = new RectangleF(b.X + p.Left,
                                              b.Y + p.Top,
                                              b.Width - p.Left - p.Right,
                                              b.Height - p.Top - p.Bottom);
            var box = v.MakeViewLayoutBox(layoutBounds, units);
            _boxes.Add(box);
            return box;
        }

        public virtual void Layout()
        {
            foreach (var v in Views)
            {
                v.Layout();
            }
        }

        public SizeF GetPreferredSize()
        {
            var max = new PointF(float.MinValue, float.MinValue);
            var min = new PointF(float.MaxValue, float.MaxValue);
            foreach (var v in Views)
            {
                max.X = Math.Max(max.X, v.LayoutRight);
                max.Y = Math.Max(max.Y, v.LayoutBottom);

                min.X = Math.Max(min.X, v.LayoutLeft);
                min.Y = Math.Max(min.Y, v.LayoutTop);
            }

            return new SizeF(Math.Abs(max.X - min.X), Math.Abs(max.Y - min.Y));
        }

        public IEnumerable<IViewLayoutBox> Views => _boxes;
    }
}
