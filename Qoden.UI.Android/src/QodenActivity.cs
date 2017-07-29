using System;
using Android.OS;
using Android.Support.V7.App;
using Qoden.Binding;

namespace Qoden.UI
{
    public abstract class QodenActivity : AppCompatActivity, IControllerHost, IViewHost
    {
        ViewHolder _view;
        public Android.Views.View View
        {
            get => _view.Value;
            set => _view.Value = value;
        }

        public virtual void ViewDidLoad()
        {
        }

        public virtual void LoadView()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
             _view = new ViewHolder(this);
            ChildControllers = new ChildViewControllersList(this, SupportFragmentManager);
            SetContentView(View);
            ViewDidLoad();
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            if (!Bindings.Bound)
            {
                Bindings.Bind();
                Bindings.UpdateTarget();
            }
        }

        public override void OnPostCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnPostCreate(savedInstanceState, persistentState);
            if (!Bindings.Bound)
            {
                Bindings.Bind();
                Bindings.UpdateTarget();
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            Bindings.Unbind();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        BindingListHolder bindings;
        public BindingList Bindings
        {
            get => bindings.Value;
            set { bindings.Value = value; }
        }

        public ChildViewControllersList ChildControllers { get; private set; }
    }

    public class QodenActivity<T> : QodenActivity where T : Android.Views.View
    {
        public new T View
        {
            get => (T)base.View;
            set { base.View = value; }
        }

        public override void LoadView()
        {
            base.View = (T)Activator.CreateInstance(typeof(T), this);
        }
    }
}