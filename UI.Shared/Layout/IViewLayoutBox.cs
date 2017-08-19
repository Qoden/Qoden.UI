using System;

namespace Qoden.UI
{
    public interface IViewLayoutBox : ILayoutBox
    {
        void Layout();
        IViewLayoutBox AutoSize(float? maxWidth = null, float? maxHeight = null);
        IViewLayoutBox AutoHeight(float? maxHeight = null);
        IViewLayoutBox AutoWidth(float? maxWidth = null);
    }
}
