﻿using System;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace Qoden.UI
{
    public class PlatformViewLayoutBox : ViewLayoutBox
    {
        IPlatformView _view;

        public PlatformViewLayoutBox(IPlatformView view, RectangleF r, IUnit unit) : base(r, unit)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public override IViewGeometry View => _view;

        public override void Layout()
        {
            _view.PlatformView.Frame = LayoutBounds;
        }
    }

    public static partial class ViewLayoutExtensions
    {
        public static SizeF SizeThatFits(this UIView v, SizeF box)
        {
            return (SizeF)v.SizeThatFits(box);
        }

        public static SizeF SizeThatFits(this UIView v, RectangleF box)
        {
            return (SizeF)v.SizeThatFits((CGSize)box.Size);
        }
    }
}
