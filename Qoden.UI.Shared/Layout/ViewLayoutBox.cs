using System;
using System.Drawing;

namespace Qoden.UI
{
#if __IOS__
    using PlatformView = UIKit.UIView;
#endif
#if __ANDROID__
    using Android.Views;
    using static Android.Views.View;
    using ViewGroup = Android.Views.ViewGroup;
    using PlatformView = Android.Views.View;
#endif

    public class ViewLayoutBox : LayoutBox, IViewLayoutBox
    {
        SizeF? _measuredSize;

        public ViewLayoutBox(PlatformView v, RectangleF r, IUnit unit) : base(r, unit)
        {
            View = v;
        }

        public override string ToString()
        {
            return string.Format("[ViewLayoutBox: Left:{0}, Top:{1}, Width:{2}, Height:{3}]", this.FrameLeft(), this.FrameTop(), this.FrameWidth(), this.FrameHeight());
        }

        public SizeF MeasuredSize
        {
            get => _measuredSize.GetValueOrDefault();
            private set
            {
                _measuredSize = value;
            }
        }

        private SizeF BoundingSize(float? maxWidth = null, float? maxHeight = null)
        {
            var w = maxWidth.HasValue ? Unit.ToPixels(maxWidth.Value).Value : OuterBounds.Width;
            var h = maxHeight.HasValue ? Unit.ToPixels(maxHeight.Value).Value : OuterBounds.Height;
            return new SizeF(w, h);
        }

        public IViewLayoutBox AutoWidth(float? maxWidth = null)
        {
            var size = BoundingSize(maxWidth);
            MeasuredSize = View.PreferredSize(size);
            this.Width(Pixel.Val(MeasuredSize.Width));
            return this;
        }

        public IViewLayoutBox AutoHeight(float? maxHeight = null)
        {
            var size = BoundingSize(null, maxHeight);
            MeasuredSize = View.PreferredSize(size);
            this.Height(Pixel.Val(MeasuredSize.Height));
            return this;
        }

        public IViewLayoutBox AutoSize(float? maxWidth = null, float? maxHeight = null)
        {
            var size = BoundingSize(maxWidth, maxHeight);
            MeasuredSize = View.PreferredSize(size);
            this.Width(Pixel.Val(MeasuredSize.Width));
            this.Height(Pixel.Val(MeasuredSize.Height));
            return this;
        }

        public void Layout()
        {
#if __IOS__
            View.Frame = this.Frame();
#elif __ANDROID__
            var parentWidthMeasureSpec = MeasureSpec.MakeMeasureSpec((int)Math.Round(OuterBounds.Width), MeasureSpecMode.AtMost);
            var parentHeightMeasureSpec = MeasureSpec.MakeMeasureSpec((int)Math.Round(OuterBounds.Height), MeasureSpecMode.AtMost);

            var frame = this.Frame();
            int childWidthMeasureSpec = ViewGroup.GetChildMeasureSpec(parentWidthMeasureSpec,
                                                            0, (int)Math.Round(frame.Width));
            int childHeightMeasureSpec = ViewGroup.GetChildMeasureSpec(parentHeightMeasureSpec,
                                                             0, (int)Math.Round(frame.Height));
            View.Measure(childWidthMeasureSpec, childHeightMeasureSpec);
            View.Layout((int)Math.Round(frame.Left),
                        (int)Math.Round(frame.Top),
                        (int)Math.Round(frame.Right),
                        (int)Math.Round(frame.Bottom));
#else
            View.Frame = this.Frame();
#endif
        }

        public PlatformView View { get; private set; }
    }
}
