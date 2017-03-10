using System;
using Foundation;
using Qoden.Binding;
using Qoden.Validation;
using UIKit;

namespace Qoden.UI.iOS
{
	public class QodenController<T> : UIViewController where T : UIView, new()
	{
		//TODO implement logging and return it
		//ILogger _log;
		//protected ILogger LOG { get { return _log ?? (_log = LoggerFactory.GetLogger(GetType().Name)); } }

		public QodenController()
		{
			Initialize();
		}

		public QodenController(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		public QodenController(NSObjectFlag t) : base(t)
		{
			Initialize();
		}

		public QodenController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		public QodenController(string nibName, NSBundle bundle) : base(nibName, bundle)
		{
			Initialize();
		}

		void Initialize()
		{
			//TODO implement logging and return it
			//LOG.Info("Created");
			Bindings = new BindingList();
		}

		public override void LoadView()
		{
			base.View = new T();
		}

		public new T View
		{
			get { return base.View as T; }
			set { base.View = value; }
		}

		public override void ViewDidLoad()
		{
			//TODO implement logging and return it
			//LOG.Info("View did load");
			base.ViewDidLoad();
		}

		public override void ViewWillAppear(bool animated)
		{
			//TODO implement logging and return it
			//LOG.Info("View will appear");
			base.ViewWillAppear(animated);
			Bindings.Bind();
			Bindings.UpdateTarget();
		}

		public override void ViewWillDisappear(bool animated)
		{
			//TODO implement logging and return it
			//LOG.Info("View will disappear");
			base.ViewWillDisappear(animated);
			Bindings.Unbind();
		}

		BindingList bindings;
		public BindingList Bindings
		{
			get
			{
				if (bindings == null)
				{
					bindings = new BindingList();
				}
				return bindings;
			}
			set
			{
				Assert.Argument(value, "value").NotNull();
				bindings = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			//TODO implement logging and return it
			//LOG.Info("Disposed");
		}
	}
}
