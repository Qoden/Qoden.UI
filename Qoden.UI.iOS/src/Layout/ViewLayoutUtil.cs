using System;
using System.Drawing;

namespace Qoden.UI
{
    public static class ViewLayoutUtil
    {
        public static SizeF SizeThatFits(ILayoutable view, SizeF bounds)
        {
            var layout = new LayoutBuilder(new RectangleF(PointF.Empty, bounds));
            view.OnLayout(layout);
            float l = int.MaxValue, t = int.MaxValue, r = int.MinValue, b = int.MinValue;
            foreach (var v in layout.Views)
            {
                var frame = v.LayoutBounds;
                l = Math.Min(frame.Left, l);
                r = Math.Max(frame.Right, r);
                t = Math.Min(frame.Top, t);
                b = Math.Max(frame.Bottom, b);
            }

            // Top and Left padings already assumed by 'r' and 'b' coordinates
            // since layout performed in padded rectangle.
            return new SizeF((float)(r + layout.Padding.Right),
                             (float)(b + layout.Padding.Bottom));
        }
    }
}
