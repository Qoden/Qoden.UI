using System;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Qoden.Binding;
using Qoden.Validation;

namespace Qoden.UI
{
    public class QodenDialog : DialogFragment, IControllerHost, IViewHost, IDetachedController
    {
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

        BindingListHolder bindings;
        public BindingList Bindings
        {
            get => bindings.Value;
            set { bindings.Value = value; }
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            ChildControllers = new ChildViewControllersList(context, ChildFragmentManager);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return _view.Value;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            ViewDidLoad();
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

        public override void OnResume()
        {
            base.OnResume();
            Bindings.Bind();
            Bindings.UpdateTarget();
        }

        public override void OnStop()
        {
            base.OnStop();
            Bindings.Unbind();
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
                WillHide?.Invoke(this, EventArgs.Empty);
                base.Dismiss();
            }
        }

        public virtual void LoadView()
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
            set 
            {
                _fragmentManager = Assert.Property(value).NotNull().Value;
            }
        }

        Context _context;
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

    public class QodenDialog<T> : QodenDialog where T : View
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
