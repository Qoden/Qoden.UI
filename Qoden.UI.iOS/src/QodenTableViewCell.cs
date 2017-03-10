using System;
using UIKit;
using Foundation;
using Qoden.Binding;

namespace Qoden.UI.iOS
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
			hierarchy = new ViewHierarchy(new PlatformView(this));
			BackgroundColor = UIColor.White;
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

	public class QodenTableViewCell<ContentT> : QodenTableViewCell
		where ContentT : UIView, new()
	{
		public QodenTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
		{
		}

		public QodenTableViewCell(UITableViewCellStyle style, NSString reuseIdentifier) : base(style, reuseIdentifier)
		{
		}

		public QodenTableViewCell()
		{
		}

		public QodenTableViewCell(NSCoder coder) : base(coder)
		{
		}

		public QodenTableViewCell(NSObjectFlag t) : base(t)
		{
		}

		public QodenTableViewCell(IntPtr handle) : base(handle)
		{
		}

		public QodenTableViewCell(CoreGraphics.CGRect frame) : base(frame)
		{
		}

		protected override void CreateView()
		{
			base.CreateView();
			Content = new ContentT();
		}

		private ContentT content;

		public virtual ContentT Content
		{
			get { return content; }
			set
			{
				if (content != null)
				{
					content.RemoveFromSuperview();
				}
				content = value;
				ContentView.AddSubview(content);
			}
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			content.Frame = ContentView.LayoutBox()
				.Left(0).Right(0).Top(0).Bottom(0)
				.AsCGRect();
		}
	}
}
