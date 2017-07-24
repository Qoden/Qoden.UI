using System;
using System.Drawing;
using Android.Views;
using AndroidView = Android.Views.View;

namespace Qoden.UI
{
    public class PlatformViewLayoutBox : ViewLayoutBox
    {
        IPlatformView<View> _view;

        public PlatformViewLayoutBox(IPlatformView<View> view, RectangleF r, IUnit unit) : base(r, unit)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public override IViewGeometry View => _view;

        public override void Layout()
        {
            var parentWidthMeasureSpec = AndroidView.MeasureSpec.MakeMeasureSpec((int)Math.Round(Bounds.Width), MeasureSpecMode.AtMost);
            var parentHeightMeasureSpec = AndroidView.MeasureSpec.MakeMeasureSpec((int)Math.Round(Bounds.Height), MeasureSpecMode.AtMost);
            int childWidthMeasureSpec = ViewGroup.GetChildMeasureSpec(parentWidthMeasureSpec,
                                                            0, (int)Math.Round(LayoutWidth));
            int childHeightMeasureSpec = ViewGroup.GetChildMeasureSpec(parentHeightMeasureSpec,
                                                             0, (int)Math.Round(LayoutHeight));
            _view.PlatformView.Measure(childWidthMeasureSpec, childHeightMeasureSpec);
            _view.PlatformView.Layout((int)Math.Round(LayoutLeft),
                        (int)Math.Round(LayoutTop),
                        (int)Math.Round(LayoutRight),
                        (int)Math.Round(LayoutBottom));
        }
    }

    public static partial class ViewLayoutExtensions
    {
        public static SizeF SizeThatFits(this View v, SizeF parentBounds)
        {
            var ws = AndroidView.MeasureSpec.MakeMeasureSpec((int)Math.Round(parentBounds.Width), MeasureSpecMode.AtMost);
            var hs = AndroidView.MeasureSpec.MakeMeasureSpec((int)Math.Round(parentBounds.Height), MeasureSpecMode.AtMost);
            v.Measure(ws, hs);
            return new SizeF(v.MeasuredWidth, v.MeasuredHeight);
        }

        public static SizeF SizeThatFits(this View v, RectangleF box)
        {
            return v.SizeThatFits(box.Size);
        }
    }
}
