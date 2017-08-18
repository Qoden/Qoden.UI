using System;
using System.Drawing;

namespace Qoden.UI
{
    public static class ViewLayoutUtil
    {
        public static SizeF PreferredSize(ILayoutable view, SizeF bounds)
        {
            var layout = new LayoutBuilder(new RectangleF(PointF.Empty, bounds));
            view.OnLayout(layout);
            return layout.PreferredSize;
        }
    }
}
