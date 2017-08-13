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

        /// <summary>
        /// Override this instead on OnCreate
        /// </summary>
        public virtual void ViewDidLoad()
        {
        }

        /// <summary>
        /// Override this to create view
        /// </summary>
        public virtual void LoadView()
        {
        }

        protected sealed override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
             _view = new ViewHolder(this);
            ChildControllers = new ChildViewControllersList(this, SupportFragmentManager);
            SetContentView(View);
            ViewDidLoad();
        }

        protected sealed override void OnResume()
        {
            base.OnResume();
            if (!Bindings.Bound)
            {
                Bindings.Bind();
                Bindings.UpdateTarget();
            }
            ViewWillAppear();
        }

        protected sealed override void OnPause()
        {
            base.OnPause();
            Bindings.Unbind();
            ViewWillDisappear();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
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