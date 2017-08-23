using System;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Microsoft.Extensions.Logging;
using Qoden.Binding;
using Qoden.Validation;

namespace Qoden.UI
{
    public class QodenController : Fragment, IControllerHost, IViewHost
    {
        public ILogger Logger { get; set; }
        
        ViewHolder _view;

        protected QodenController(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
        }

        public QodenController()
        {
            Initialize();
        }

        private void Initialize()
        {
            _view = new ViewHolder(this);
            Logger = CreateLogger();
            if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
                Logger.LogInformation("{controller} Created", GetType().Name);
        }
        
        protected virtual ILogger CreateLogger()
        {
            return Config.LoggerFactory?.CreateLogger(GetType().Name);
        }

        BindingListHolder _bindings;
        public BindingList Bindings
        {
            get => _bindings.Value;
            set => _bindings.Value = value;
        }

        public sealed override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
                Logger.LogInformation("{controller} OnCreateView", GetType().Name);

            return _view.Value;
        }

        public sealed override void OnViewCreated(Android.Views.View view, Bundle savedInstanceState)
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
                Logger.LogInformation("{controller} OnViewCreated (ViewDidLoad)", GetType().Name);

            base.OnViewCreated(view, savedInstanceState);
            ViewDidLoad();
        }

        public new View View
        {
            get => _view.Value;
            set => _view.Value = value;
        }

        ChildViewControllersList _childControllers;
        public ChildViewControllersList ChildControllers 
        { 
            get
            {
                Assert.State(Context == null || ChildFragmentManager == null, nameof(ChildControllers))
                      .IsFalse("Cannot access {Key} wen fragment detached");
                if (_childControllers == null)
                {
                    _childControllers = new ChildViewControllersList(Context, ChildFragmentManager);
                }
                return _childControllers;
            }
        }

        /// <summary>
        /// Override this instead on OnViewCreated
        /// </summary>
        public virtual void ViewDidLoad()
        {                        
        }

        public sealed override void OnResume()
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
                Logger.LogInformation("{controller} OnResume (ViewWillAppear)", GetType().Name);

            base.OnResume();
            if (!Bindings.Bound)
            {
                Bindings.Bind();
                Bindings.UpdateTarget();
            }
            ViewWillAppear();
        }

        public sealed override void OnPause()
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
                Logger.LogInformation("{controller} OnPause (ViewWillDisappear)", GetType().Name);

            base.OnPause();
            Bindings.Unbind();
            ViewWillDisappear();
        }

        /// <summary>
        /// Override this instead on OnResume
        /// </summary>
        protected virtual void ViewWillAppear()
        { }

        /// <summary>
        /// Override this instead on OnPause
        /// </summary>
        protected virtual void ViewWillDisappear()
        { }

        /// <summary>
        /// Override this instead on CreateView
        /// </summary>
        public virtual void LoadView()
        {
        }
    }

    public class QodenController<T> : QodenController where T : Android.Views.View
    {
        public new T View
        {
            get => (T)base.View;
            set => base.View = value;
        }

        public override void LoadView()
        {
            if (Context == null)
            {
                throw new InvalidOperationException("Cannot access View before controller added to parent Activity/Fragment");
            }
            base.View = (T)Activator.CreateInstance(typeof(T), Context);
        }
    }
}
