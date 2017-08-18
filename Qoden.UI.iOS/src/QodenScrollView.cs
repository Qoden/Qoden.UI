using System;
using System.Drawing;
using UIKit;

namespace Qoden.UI
{
	public class QodenScrollView : UIScrollView, ILayoutable
	{
		public ViewBuilder Builder { get; private set; }

		public QodenScrollView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		public QodenScrollView ()
		{
			Initialize ();
		}

        void Initialize()
        {
            Builder = new ViewBuilder(this);
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

        public sealed override CoreGraphics.CGSize SizeThatFits(CoreGraphics.CGSize size)
        {
            return PreferredSize((SizeF)size);
        }

        public virtual SizeF PreferredSize(SizeF bounds)
        {
            return ViewLayoutUtil.PreferredSize(this, bounds);
        }

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				Builder.Dispose();
			}
			base.Dispose (disposing);
		}
	}
	
}
