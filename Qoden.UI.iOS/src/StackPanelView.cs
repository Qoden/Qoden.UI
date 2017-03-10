using System;
using UIKit;
using CoreGraphics;
using System.Drawing;
using System.Linq;

namespace Qoden.UI.iOS
{
	public class StackPanelView : UIView
	{
		private readonly StackLayout layout = new StackLayout();

		public StackPanelView ()
		{
		}

		public StackPanelView (Foundation.NSCoder coder) : base (coder)
		{
		}
		

		public StackPanelView (Foundation.NSObjectFlag t) : base (t)
		{
		}
		

		public StackPanelView (IntPtr handle) : base (handle)
		{
		}
		

		public StackPanelView (CGRect frame) : base (frame)
		{
		}

		public override void AddSubview(UIView view)
		{
			layout.Add (new PlatformView(view) );
			base.AddSubview (view);
		}

		public void AddSubview (UIView view, float size)
		{
			layout.Add (new PlatformView(view), size);
			base.AddSubview (view);
		}

		public void AddSubview (UIView view, float size, Padding margins)
		{
			layout.Add (new PlatformView(view), size, margins);
			base.AddSubview (view);
		}

		public void AddSubview (StackLayoutInfo info)
		{
			layout.Add (info);
			base.AddSubview (info.View.Native as UIView);
		}

		public override void WillRemoveSubview (UIView uiview)
		{
			base.WillRemoveSubview (uiview);
			layout.Remove (new PlatformView(uiview));
		}

		public override void LayoutSubviews ()
		{
			layout.Layout (new PlatformView(this));
		}

		public LayoutDirection Direction {
			get { return layout.Direction; }
			set {
				layout.Direction = value;
				SetNeedsLayout ();
			}
		}

		public override CGSize SizeThatFits (CGSize size)
		{
			var bounds = new RectangleF (PointF.Empty, (SizeF)size);
			if (layout.Direction == LayoutDirection.TopBottom || layout.Direction == LayoutDirection.BottomTop)
			{
				var height = layout.Measure(bounds).DefaultIfEmpty().Max(p => p.Frame.Bottom);
				return new CGSize(size.Width, height);
			}
			var width = layout.Measure(bounds).DefaultIfEmpty().Max(p => p.Frame.Right);
			return new CGSize(width, size.Height);
		}
	}
}

