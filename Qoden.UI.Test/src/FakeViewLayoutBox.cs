﻿using System;
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

        public IViewLayoutBox AutoHeight(float? maxHeight = null)
        {
            SetHeight(Math.Max(View.Frame.Height, maxHeight.GetValueOrDefault()));
            return this;
        }

        public IViewLayoutBox AutoSize(float? maxWidth = null, float? maxHeight = null)
        {
            return AutoWidth(maxWidth).AutoHeight(maxHeight);
        }

        public IViewLayoutBox AutoWidth(float? maxWidth = null)
        {
            SetWidth(Math.Max(View.Frame.Width, maxWidth.GetValueOrDefault()));
            return this;
        }

        public void Layout()
        {
            View.Frame = LayoutBounds;
        }

        public RectangleF ViewFrame => View.Frame;
    }
}
