using System;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Qoden.Binding;
namespace Qoden.UI
{
    public class QodenController : Fragment, IControllerHost, IViewHost
    {
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
        }

        BindingListHolder bindings;
        public BindingList Bindings
        {
            get => bindings.Value;
            set { bindings.Value = value; }
        }

        public override void OnAttach(Android.Content.Context context)
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

        public virtual void LoadView()
        {
        }
    }

    public class QodenController<T> : QodenController where T : View
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
