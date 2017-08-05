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
            return layout.PreferredSize;
        }
    }
}
