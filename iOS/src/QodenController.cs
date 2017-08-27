using System;
using System.Linq;
using Foundation;
using Microsoft.Extensions.Logging;
using Qoden.Binding;
using UIKit;

namespace Qoden.UI
{
    public class QodenController<T> : UIViewController where T : UIView, new()
    {
	    public ILogger Logger { get; set; }

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
			Logger = CreateLogger();
			if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
				Logger.LogInformation("Created");
		}

	    protected virtual ILogger CreateLogger()
	    {
		    return Config.LoggerFactory?.CreateLogger(GetType().Name);
	    }

	    public override void LoadView()
		{
			base.View = new T();
		}

		public new T View
		{
			get => base.View as T;
			set 
            {
                if (!IsViewLoaded)
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
			if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
				Logger.LogInformation("View did load");
			base.ViewDidLoad();
		}

		public override void ViewWillAppear(bool animated)
		{
			if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
				Logger.LogInformation("View will appear");
			base.ViewWillAppear(animated);
			Bindings.Bind();
			Bindings.UpdateTarget();
            ViewWillAppear();
		}
		public override void ViewWillDisappear(bool animated)
		{
			if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
				Logger.LogInformation("View will disappear");
			base.ViewWillDisappear(animated);
			Bindings.Unbind();
            ViewWillDisappear();
		}

        protected virtual void ViewWillAppear()
        { }

        protected virtual void ViewWillDisappear()
        { }

        BindingListHolder _bindings;
        public BindingList Bindings
        {
            get => _bindings.Value;
            set { _bindings.Value = value; }
        }

        protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			//TODO implement logging and return it
			if (Logger != null && Logger.IsEnabled(LogLevel.Information)) Logger?.LogInformation("Disposed");
		}

        public TController GetChildViewController<TController>(string key, Func<TController> factory) where TController : UIViewController
        {
            var childControllers = ChildViewControllers;
            var childController = (TController)childControllers.FirstOrDefault(x => x.GetTag() == key);
            if (childController == null)
            {
                childController = factory();
                childController.SetTag(key);
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

        internal IViewModelStore ViewModelStore = new ViewModelStore();
        
	}

	public static class QodenControllerExtnerions
	{
		public static IViewModelStore GetViewModelStore<T>(this QodenController<T> controller) where T : UIView, new()
		{
			return controller.ViewModelStore;
		}
	}

    public static class QodenChildController
    {
        public static readonly NSString Tag = new NSString("Qoden_ChildController_Tag");

        public static string GetTag(this UIViewController controller)
        {
            var nsTag = (NSString)AssociatedObject.Get(controller, Tag);
            return nsTag.ToString();
        }

        public static void SetTag(this UIViewController controller, string tag)
        {
            AssociatedObject.Set(controller, Tag, new NSString(tag), AssociationPolicy.COPY);
        }
    }

}
