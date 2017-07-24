using System;
using System.Linq;
using Foundation;
using Qoden.Binding;
using UIKit;

namespace Qoden.UI
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
		}

		public override void LoadView()
		{
			base.View = new T();
		}

		public new T View
		{
			get { return base.View as T; }
			set 
            {
                if (!this.IsViewLoaded)
                {
                    base.View = value;
                    ViewDidLoad();
                }
                else
                {
                    throw new InvalidOperationException("Cannot change loaded view");
                }
            }
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

        BindingListHolder bindings;
        public BindingList Bindings
        {
            get => bindings.Value;
            set { bindings.Value = value; }
        }

        protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			//TODO implement logging and return it
			//LOG.Info("Disposed");
		}

        public TController GetChildViewController<TController>(string key, Func<TController> factory) where TController : UIViewController
        {
            var childControllers = ChildViewControllers;
            var childController = (TController)childControllers.Where(x => x.GetTag() == key).FirstOrDefault();
            if (childController == null)
            {
                childController = factory();
                childController.SetTag(key);
                AddChildViewController(childController);
            }
            return childController;
        }

        public TController GetChildViewController<TController>() where TController : UIViewController, new()
        {
            return GetChildViewController<TController>(typeof(TController).FullName);
        }

        public TController GetChildViewController<TController>(string key) where TController : UIViewController, new()
        {
            return GetChildViewController(key, () => new TController());
        }

        IViewModelStore _viewModelStore = new ViewModelStore();
        public IViewModelStore GetViewModelStore()
        {
            return _viewModelStore;
        }
	}

    public static class QodenChildController
    {
        public static readonly NSString TAG = new NSString("Qoden_ChildController_Tag");

        public static string GetTag(this UIViewController controller)
        {
            var nsTag = (NSString)AssociatedObject.Get(controller, TAG);
            return nsTag.ToString();
        }

        public static void SetTag(this UIViewController controller, string tag)
        {
            AssociatedObject.Set(controller, TAG, new NSString(tag), AssociationPolicy.COPY);
        }
    }

}
