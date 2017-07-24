using System;
using Android.OS;
using Android.Views;
using Qoden.Binding;
using AndroidView = Android.Views.View;
using AndroidViewGroup = Android.Views.ViewGroup;
namespace Qoden.UI
{
    public class QodenController : Android.Support.V4.App.Fragment
    {
        BindingListHolder bindings;
        public BindingList Bindings
        {
            get => bindings.Value;
            set { bindings.Value = value; }
        }

        public override void OnViewCreated(AndroidView view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            ViewDidLoad();
        }

        public bool IsViewLoaded => View != null;

        public object ParentViewController 
        {
            get 
            {
                object parent = ParentFragment;
                if (parent == null)
                    parent = Activity;
                return parent;
            }
        }

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
    }

    public class QodenController<T> : QodenController where T : AndroidView
    {
        public override AndroidView OnCreateView(LayoutInflater inflater, AndroidViewGroup container, Bundle savedInstanceState)
        {
            if (_view == null)
                _view = (T)Activator.CreateInstance(typeof(T), Activity);
            return _view;
        }

        T _view;
        public new T View
        {
            get => _view;
            set 
            {
                if (_view != null)
                    throw new InvalidOperationException();
                _view = value ?? throw new ArgumentNullException();
            }
        }
    }
}
