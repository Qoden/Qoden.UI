using System;
using UIKit;

namespace Qoden.UI
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
			hierarchy = new ViewHierarchy(this, ViewHierarchyBuilder.Instance);
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
