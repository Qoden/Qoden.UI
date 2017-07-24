using System;
using System.Drawing;

namespace Qoden.UI.Test
{
    public class FakeViewLayoutBox : LayoutBox, IViewLayoutBox
    {
        public FakeView View { get; set; }

        public FakeViewLayoutBox(FakeView view, RectangleF outerBounds) : base(outerBounds)
        {
            View = view;
        }

        public FakeViewLayoutBox(RectangleF outerBounds) : base(outerBounds)
        {
        }

        public IViewLayoutBox AutoHeight()
        {
            SetHeight(View.Frame.Height);
            return this;
        }

        public IViewLayoutBox AutoSize()
        {
            return AutoWidth().AutoHeight();
        }

        public IViewLayoutBox AutoWidth()
        {
            SetWidth(View.Frame.Width);
            return this;
        }

        public void Layout()
        {
            View.Frame = LayoutBounds;
        }

        public RectangleF ViewFrame => View.Frame;
    }
}
