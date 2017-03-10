using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Qoden.UI.iOS
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
			hierarchy = new ViewHierarchy(new PlatformView(this));
			BackgroundColor = UIColor.White;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				hierarchy?.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
