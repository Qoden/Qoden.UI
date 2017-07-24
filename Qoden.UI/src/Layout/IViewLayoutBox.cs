using System;

namespace Qoden.UI
{
    public interface IViewLayoutBox : ILayoutBox
    {
        void Layout();
        IViewLayoutBox AutoSize();
        IViewLayoutBox AutoHeight();
        IViewLayoutBox AutoWidth();
    }
}
