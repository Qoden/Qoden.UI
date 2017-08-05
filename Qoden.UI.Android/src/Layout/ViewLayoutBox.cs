using System;
using System.Drawing;
using Android.Views;
using AndroidView = Android.Views.View;

namespace Qoden.UI
{
    public class PlatformViewLayoutBox : ViewLayoutBox
    {
        private IPlatformView _view;

        public PlatformViewLayoutBox(IPlatformView view, RectangleF r, IUnit unit) : base(r, unit ?? Units.PlatformDefault)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public override IViewGeometry View => _view;

        public override void Layout()
        {
            var parentWidthMeasureSpec = AndroidView.MeasureSpec.MakeMeasureSpec((int)Math.Round(OuterBounds.Width), MeasureSpecMode.AtMost);
            var parentHeightMeasureSpec = AndroidView.MeasureSpec.MakeMeasureSpec((int)Math.Round(OuterBounds.Height), MeasureSpecMode.AtMost);

            var frame = this.Frame();
            int childWidthMeasureSpec = ViewGroup.GetChildMeasureSpec(parentWidthMeasureSpec,
                                                            0, (int)Math.Round(frame.Width));
            int childHeightMeasureSpec = ViewGroup.GetChildMeasureSpec(parentHeightMeasureSpec,
                                                             0, (int)Math.Round(frame.Height));
            _view.PlatformView.Measure(childWidthMeasureSpec, childHeightMeasureSpec);
            _view.PlatformView.Layout((int)Math.Round(frame.Left),
                        (int)Math.Round(frame.Top),
                        (int)Math.Round(frame.Right),
                        (int)Math.Round(frame.Bottom));
        }
    }
}
