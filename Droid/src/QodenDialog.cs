using System;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Microsoft.Extensions.Logging;
using Qoden.Binding;
using Qoden.Validation;

namespace Qoden.UI
{
    public class QodenDialog : DialogFragment, IControllerHost, IViewHost, IDetachedController
    {
        public ILogger Logger { get; set; }
        
        ViewHolder _view;

        protected QodenDialog(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
        }

        public QodenDialog()
        {
            Initialize();
        }

        private void Initialize()
        {
            _view = new ViewHolder(this);
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

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            Logger = CreateLogger();
            if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
                Logger.LogInformation("{controller} OnAttach", GetType().Name);

            ChildControllers = new ChildViewControllersList(context, ChildFragmentManager);
        }

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
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
            if (!_view.DidLoad)
            {
                _view.DidLoad = true;
                ViewDidLoad();
            }
        }
        
        public override void OnDismiss(IDialogInterface dialog)
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
                Logger.LogInformation("{controller} OnDismiss", GetType().Name);

            base.OnDismiss(dialog);
            IsDisplayed = false;
        }
        
        public new View View
        {
            get => _view.Value;
            set => _view.Value = value;
        }

        public ChildViewControllersList ChildControllers { get; private set; }

        public virtual void ViewDidLoad()
        {
        }

        public sealed override void OnResume()
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
                Logger.LogInformation("{controller} OnResume (ViewWillAppear)", GetType().Name);

            base.OnResume();
            Bindings.Bind();
            Bindings.UpdateTarget();
            ViewWillAppear();
        }

        public sealed override void OnPause()
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
                Logger.LogInformation("{controller} OnResume (ViewWillDisappear)", GetType().Name);

            base.OnPause();
            Bindings.Unbind();
            ViewWillDisappear();
        }

        public bool IsDisplayed
        {
            get;
            private set;
        }

        public event EventHandler WillShow;

        public event EventHandler WillHide;

        public void Show()
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
                Logger.LogInformation("{controller} Show", GetType().Name);

            WillShow?.Invoke(this, EventArgs.Empty);
            var fm = ((IDetachedController)this).FragmentManager;
            if (fm == null)
            {
                throw new InvalidOperationException("Cannot find Target Fragment Manager");
            }
            base.Show(fm, Tag);
            IsDisplayed = true;
        }

        public void Hide()
        {
            if (IsDisplayed)
            {
                if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
                    Logger.LogInformation("{controller} Hide", GetType().Name);

                WillHide?.Invoke(this, EventArgs.Empty);
                base.Dismiss();
            }
        }

        public virtual void LoadView()
        {
        }

        protected virtual void ViewWillAppear()
        {
        }

        protected virtual void ViewWillDisappear()
        {            
        }

        FragmentManager _fragmentManager;
        FragmentManager IDetachedController.FragmentManager 
        {
            get 
            {
                if (_fragmentManager == null)
                {
                    _fragmentManager = FragmentManager;
                }

                if (_fragmentManager == null)
                {
                    var appCompatActivity = QodenApplication.ActiveActivity as AppCompatActivity;
                    _fragmentManager = appCompatActivity?.SupportFragmentManager;
                }

                if (_fragmentManager != null && FragmentManager != null && _fragmentManager != FragmentManager)
                {
                    throw new InvalidOperationException("Internal inconsistencty detected. " +
                                                        "Detached Fragment Manager is not the " +
                                                        "same as FragmentManager supplied by system.");
                }

                return _fragmentManager;
            }
            set => _fragmentManager = Assert.Property(value).NotNull().Value;
        }

        private Context _context;
        Context IDetachedController.Context
        {
            get
            {
                if (_context == null)
                {
                    _context = Context;
                }
                return _context;
            }
            set => _context = value;
        }
    }

    public class QodenDialog<T> : QodenDialog where T : Android.Views.View
    {
        public new T View
        {
            get => (T)base.View;
            set => base.View = value;
        }

        public override void LoadView()
        {            
            var ctx = ((IDetachedController)this).Context;
            if (ctx == null)
            {
                throw new InvalidOperationException("Dialog does not have Context");
            }
            base.View = (T)Activator.CreateInstance(typeof(T), ctx);
        }
    }
}
