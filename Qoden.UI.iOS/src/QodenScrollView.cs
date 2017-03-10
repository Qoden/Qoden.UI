using System;
using UIKit;

namespace Qoden.UI.iOS
{
	public class QodenScrollView : UIScrollView
	{
		ViewHierarchy hierarchy;

		public QodenScrollView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		public QodenScrollView ()
		{
			Initialize ();
		}

		void Initialize ()
		{
			CreateView ();
		}

		protected virtual void CreateView ()
		{
			hierarchy = new ViewHierarchy(new PlatformView(this));
			BackgroundColor = UIColor.White;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				hierarchy?.Dispose();
			}
			base.Dispose (disposing);
		}
	}
	
}
