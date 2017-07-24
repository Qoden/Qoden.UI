using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Qoden.Binding;

namespace Qoden.UI
{
    public abstract class QodenActivity : AppCompatActivity
    {
        private Android.Views.View _view;
        public Android.Views.View View
        {
            get
            {
                if (_view == null)
                {
                    LoadView();
                }
                if (_view == null)
                {
                    throw new InvalidOperationException("Controller does not have view.");
                }
                return _view;
            }
            set
            {
                if (!IsViewLoaded)
                {
                    _view = value;
                    ViewDidLoad();
                }
                else
                {
                    throw new InvalidOperationException("Cannot change loaded view");
                }
            }
        }

        public virtual void ViewDidLoad()
        {
        }

        public virtual void LoadView()
        {
        }

        public bool IsViewLoaded => _view != null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportFragmentManager.RegisterFragmentLifecycleCallbacks(new CleanupChildControllers(this), false);
            SetContentView(View);
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

        List<Fragment> _pendingFragments = new List<Fragment>();

        public T GetChildViewController<T>(string key, Func<T> factory) where T : Fragment
        {
            var fragment = (T)SupportFragmentManager.FindFragmentByTag(key);
            if (fragment == null)
            {
                fragment = (T)_pendingFragments.Where(x => x.Tag == key).FirstOrDefault();
            }
            if (fragment == null)
            {
                fragment = factory();
                _pendingFragments.Add(fragment);
                SupportFragmentManager.BeginTransaction()
                                      .Add(fragment, key)
                                      .CommitAllowingStateLoss();
            }
            return fragment;
        }

        public T GetChildViewController<T>() where T : Fragment, new()
        {
            return GetChildViewController<T>(typeof(T).FullName);
        }

        public T GetChildViewController<T>(string key) where T : Fragment, new()
        {
            return GetChildViewController(key, () => new T());
        }

        class CleanupChildControllers : FragmentManager.FragmentLifecycleCallbacks
        {
            QodenActivity _activity;

            public CleanupChildControllers(QodenActivity activity)
            {
                _activity = activity;
            }

            public override void OnFragmentDestroyed(FragmentManager fm, Fragment f)
            {
                base.OnFragmentDestroyed(fm, f);
                _activity._pendingFragments.Remove(f);
            }

            public override void OnFragmentCreated(FragmentManager fm, Fragment f, Bundle savedInstanceState)
            {
                base.OnFragmentCreated(fm, f, savedInstanceState);
                _activity._pendingFragments.Remove(f);
            }
        }
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