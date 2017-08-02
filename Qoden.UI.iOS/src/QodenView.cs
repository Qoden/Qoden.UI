using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using Qoden.Binding;
using UIKit;

namespace Qoden.UI
{
    public class QodenView : UIView, ILayoutable
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

        public QodenView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        public QodenView(NSObjectFlag t) : base(t)
        {
            Initialize();
        }

        public QodenView(CGRect frame) : base(frame)
        {
            Initialize();
        }

        void Initialize()
        {
            CreateView();
        }

        public ViewHierarchyBuilder Builder { get => ViewHierarchyBuilder.Instance; }

        protected virtual void CreateView()
        {            
            hierarchy = new ViewHierarchy(this, Builder);
        }

        public sealed override void LayoutSubviews()
        {
            var layoutBuilder = new LayoutBuilder((RectangleF)Bounds);
            OnLayout(layoutBuilder);
            foreach (var box in layoutBuilder.Views)
            {
                box.Layout();
            }
        }

        void ILayoutable.OnLayout(LayoutBuilder layout)
        {
            OnLayout(layout);
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

        public sealed override CGSize SizeThatFits(CGSize size)
        {
            return SizeThatFits((SizeF)size);
        }

        public virtual SizeF SizeThatFits(SizeF bounds)
        {
            return ViewLayoutUtil.SizeThatFits(this, bounds);
        }

        IEventSource _clickTarget;

        public IEventSource ClickTarget()
        {
            if (_clickTarget == null)
            {
                _clickTarget = new UIViewClickEventSource(this);
            }
            return _clickTarget;
        }

        //public UIView PlatformView => this;

        //RectangleF IViewGeometry.Frame { get => (RectangleF)Frame; set => Frame = value; }

        //public IViewLayoutBox LayoutInBounds(RectangleF bounds, IUnit unit = null)
        //{
        //    return new PlatformViewLayoutBox(this, bounds, unit);
        //}
    }
}
