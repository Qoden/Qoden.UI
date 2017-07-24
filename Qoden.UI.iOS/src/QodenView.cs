using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Qoden.UI
{
    public class QodenView : UIView
    {
        ViewHierarchy hierarchy;

        public QodenView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public QodenView()
        {
            Initialize();
        }

        public QodenView(NSCoder coder) : base (coder)
        {
            Initialize();
        }

        public QodenView(NSObjectFlag t) : base (t)
        {
            Initialize();
        }

        public QodenView(CGRect frame) : base (frame)
        {
            Initialize();
        }

        void Initialize()
        {
            CreateView();
        }

        protected virtual void CreateView()
        {
            hierarchy = new ViewHierarchy(this, ViewHierarchyBuilder.Instance);
            BackgroundColor = UIColor.White;
        }

        public ViewHierarchyBuilder Builder { get => ViewHierarchyBuilder.Instance; }

        public override void LayoutSubviews()
        {
            var layoutBuilder = new LayoutBuilder((RectangleF)Bounds);
            OnLayout(layoutBuilder);
            foreach (var box in layoutBuilder.Views)
            {
                box.Layout();
            }
        }

        protected virtual void OnLayout(LayoutBuilder layout)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                hierarchy?.Dispose();
            }
            base.Dispose(disposing);
        }

        public override CGSize SizeThatFits(CGSize size)
        {
            return SizeThatFits((SizeF)size);
        }

        public virtual SizeF SizeThatFits(SizeF bounds)
        {
            var layout = new LayoutBuilder(new RectangleF(PointF.Empty, bounds));
            OnLayout(layout);
            float l = int.MaxValue, t = int.MaxValue, r = int.MinValue, b = int.MinValue;
            foreach (var v in layout.Views)
            {
                var frame = v.LayoutBounds;
                l = Math.Min(frame.Left, l);
                r = Math.Max(frame.Right, r);
                t = Math.Min(frame.Top, t);
                b = Math.Max(frame.Bottom, b);
            }
            return new SizeF(Math.Abs(r - l), Math.Abs(b - t));
        }
    }
}
