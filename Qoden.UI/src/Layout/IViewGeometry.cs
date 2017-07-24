using System;
using System.Drawing;

namespace Qoden.UI
{
    public interface IViewGeometry
    {
        RectangleF Frame { get; set; }
        SizeF SizeThatFits(SizeF bounds);
        IViewLayoutBox LayoutInBounds(RectangleF bounds, IUnit unit = null);
    }
}
