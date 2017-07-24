using System;
using UIKit;
using Foundation;
using Qoden.Binding;
using System.Drawing;

namespace Qoden.UI
{
	public class QodenTableViewCell : UITableViewCell
	{
		ViewHierarchy hierarchy;

		public QodenTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
		{
			Initialize();
		}

		public QodenTableViewCell()
		{
			Initialize();
		}

		public QodenTableViewCell(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		public QodenTableViewCell(NSObjectFlag t) : base(t)
		{
			Initialize();
		}

		public QodenTableViewCell(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		public QodenTableViewCell(CoreGraphics.CGRect frame) : base(frame)
		{
			Initialize();
		}

		public QodenTableViewCell(UITableViewCellStyle style, NSString reuseIdentifier) : base(style, reuseIdentifier)
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

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var layout = new LayoutBuilder((RectangleF)Bounds);
            OnLayout(layout);
            foreach (var v in layout.Views)
            {
                v.Layout();
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
				bindings?.Unbind();
			}
			base.Dispose(disposing);
		}


		public override void WillMoveToSuperview(UIView newsuper)
		{
			base.WillMoveToSuperview(newsuper);

			if (bindings != null)
			{
				if (newsuper != null)
				{
					Bindings.Bind();
					Bindings.UpdateTarget();
				}
				else
				{
					Bindings.Unbind();
				}
			}
		}

		private BindingList bindings;
		public BindingList Bindings
		{
			get
			{
				if (bindings == null)
					bindings = new BindingList();
				return bindings;
			}
		}
	}
}
