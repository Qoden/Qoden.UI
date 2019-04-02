using System;
using System.Collections.Generic;
using Foundation;
using Microsoft.Extensions.Logging;
using Qoden.Binding;
using Qoden.UI.Wrappers;
using UIKit;

namespace Qoden.UI
{
	public class QodenController : UIViewController
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

        public void ClearStackAndPush(UIViewController controller, bool animated = true) => NavigationController.SetViewControllers(new UIViewController[] { controller }, animated);

        public void Push(UIViewController controller, bool animated = true) => NavigationController.PushViewController(controller, animated);

        public void Pop(bool animated = true) => NavigationController.PopViewController(animated);

        public void Present(UIViewController controller, bool animated = true, Action completionHandler = null, bool withNavigation = false) 
	        => PresentViewController(withNavigation ? new UINavigationController(controller) : controller, animated, completionHandler);

		public void Dismiss(bool animated) => DismissViewController(animated, null);
		public void Dismiss() => Dismiss(true);

        protected virtual void ViewWillAppear()
        { }

        protected virtual void ViewWillDisappear()
        { }

        BindingListHolder _bindings;
        public BindingList Bindings
        {
            get => _bindings.Value;
            set => _bindings.Value = value;
        }

        protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			//TODO implement logging and return it
			if (Logger != null && Logger.IsEnabled(LogLevel.Information)) Logger?.LogInformation("Disposed");
		}

	    private Dictionary<string, UIViewController> _childControllers;
	    
        public TController GetChildViewController<TController>(string key, Func<TController> factory) where TController : UIViewController
        {
	        if (_childControllers == null)
	        {
		        _childControllers = new Dictionary<string, UIViewController>();
	        }
	       	UIViewController childController;
	        if (!_childControllers.TryGetValue(key, out childController))
	        {
		        childController = factory();
		        _childControllers[key] = childController;
                AddChildViewController(childController);
	        }
	        return (TController)childController;
        }

        public List<MenuItemInfo> MenuItems
        {
            set
            {
                var leftButtons = new List<UIBarButtonItem>();
                var rightButtons = new List<UIBarButtonItem>();
                foreach(var item in value)
                {
                    // todo: setup correctly. title etc...
                    var sideButtons = item.Side == Side.Left ? leftButtons : rightButtons;
                    var barButtonItem = new UIBarButtonItem(item.Icon, UIBarButtonItemStyle.Plain,
                        (sender, eventArgs) =>
                        {
                            item.Command?.Execute();
                        });
                    sideButtons.Add(barButtonItem);	
                }
                NavigationItem.SetRightBarButtonItems(rightButtons.ToArray(), true);
				NavigationItem.SetLeftBarButtonItems(leftButtons.ToArray(), true);
            }
        }

        internal IViewModelStore ViewModelStore = new ViewModelStore();

        public bool ToolbarVisible
        {
            get 
			{
				if(NavigationController != null)
					return !NavigationController.NavigationBarHidden;
				
				if(PresentingViewController?.NavigationController != null) 
					return !PresentingViewController.NavigationController.NavigationBarHidden;
				
				if (TabBarController?.PresentingViewController is UITabBarController && TabBarController.NavigationController != null)
					return !TabBarController.NavigationController.NavigationBarHidden;
				
				return false;
			}
            set
            {
                if (NavigationController != null)
                {
                    NavigationController.SetNavigationBarHidden(!value, true);
                } else if (PresentingViewController != null)
                {
                    PresentingViewController.NavigationController?.SetNavigationBarHidden(!value, true);
                } else if (TabBarController?.PresentingViewController is UITabBarController)
                {
                    TabBarController.NavigationController?.SetNavigationBarHidden(!value, true);
                }
            }
        }
	}
	
    public class QodenController<T> : QodenController where T : UIView, new()
    {
	    public QodenController()
	    {
	    }

	    public QodenController(NSCoder coder) : base(coder)
	    {
	    }

	    public QodenController(NSObjectFlag t) : base(t)
	    {
	    }

	    public QodenController(IntPtr handle) : base(handle)
	    {
	    }

	    public QodenController(string nibName, NSBundle bundle) : base(nibName, bundle)
	    {
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
	}

	public static class QodenControllerExtentions
	{
		public static IViewModelStore GetViewModelStore(this QodenController controller)
		{
			return controller.ViewModelStore;
		}
		
		public static T GetChildViewController<T>(this QodenController host, string key, Func<T> factory) where T : UIViewController
		{
			return host.GetChildViewController(key, factory);
		}

		public static T GetChildViewController<T>(this QodenController host) where T : UIViewController, new()
		{
			return host.GetChildViewController<T>(typeof(T).FullName);
		}

		public static T GetChildViewController<T>(this QodenController host, string key) where T : UIViewController, new()
		{
			return host.GetChildViewController(key, () => new T());
		}
	}
}
