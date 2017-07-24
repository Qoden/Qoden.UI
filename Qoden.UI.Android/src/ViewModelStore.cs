using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace Qoden.UI
{
    public static class AndroidViewModelStoreExtensions
    {
        public static IViewModelStore GetViewModelStore(this FragmentActivity owner)
        {
            return ViewModelStoreFragment.FindOrCreate(owner);
        }

        public static IViewModelStore GetViewModelStore(this Fragment owner)
        {
            return ViewModelStoreFragment.FindOrCreate(owner);
        }
    }

    /// <summary>
    /// This fragment stores view model during Activity or Fragment reload.
    /// </summary>
    public class ViewModelStoreFragment : Fragment
    {
        public static readonly string HOLDER_TAG = "qoden.ui.ViewModelHolder";
        IViewModelStore _store = new ViewModelStore();

        public ViewModelStoreFragment()
        {
            RetainInstance = true;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _pendingActivityHolders.Remove(Activity);
        }

        public override void OnDestroy()
        {            
            base.OnDestroy();
            _store.Clear();
        }

        static Dictionary<Activity, ViewModelStoreFragment> _pendingActivityHolders = new Dictionary<Activity, ViewModelStoreFragment>();
        static Dictionary<Fragment, ViewModelStoreFragment> _pendingFragmentHolders = new Dictionary<Fragment, ViewModelStoreFragment>();
        static Application.IActivityLifecycleCallbacks _activityCleanup;
        static FragmentManager.FragmentLifecycleCallbacks _fragmentCleanup = new CleanPendingFragmentHolders();

        internal static IViewModelStore FindOrCreate(FragmentActivity owner)
        {
            var fm = owner.SupportFragmentManager;
            var holder = (ViewModelStoreFragment)fm.FindFragmentByTag(ViewModelStoreFragment.HOLDER_TAG);
            if (holder != null)
            {
                return holder._store;
            }
            if (_pendingActivityHolders.TryGetValue(owner, out holder))
            {
                return holder._store;
            }
            if (_activityCleanup == null)
            {
                _activityCleanup = new CleanPendingActivityHolders();
                owner.Application.RegisterActivityLifecycleCallbacks(_activityCleanup);
            }
            holder = Create(fm);
            _pendingActivityHolders.Add(owner, holder);
            return holder._store;
        }

        internal static IViewModelStore FindOrCreate(Fragment owner)
        {
            var fm = owner.ChildFragmentManager;
            var holder = (ViewModelStoreFragment)fm.FindFragmentByTag(ViewModelStoreFragment.HOLDER_TAG);
            if (holder != null)
            {
                return holder._store;
            }
            if (_pendingFragmentHolders.TryGetValue(owner, out holder))
            {
                return holder._store;
            }

            owner.FragmentManager.RegisterFragmentLifecycleCallbacks(_fragmentCleanup, false);
            holder = Create(fm);
            _pendingFragmentHolders.Add(owner, holder);

            return holder._store;
        }

        static ViewModelStoreFragment Create(FragmentManager ft)
        {
            var holder = new ViewModelStoreFragment();
            ft.BeginTransaction()
              .Add(holder, HOLDER_TAG)
              .CommitAllowingStateLoss();
            return holder;
        }

        class CleanPendingFragmentHolders : FragmentManager.FragmentLifecycleCallbacks
        {
            public override void OnFragmentDestroyed(FragmentManager fm, Fragment f)
            {
                base.OnFragmentDestroyed(fm, f);
                _pendingFragmentHolders.Remove(f);
            }
        }

        class CleanPendingActivityHolders : Java.Lang.Object, Application.IActivityLifecycleCallbacks
        {
            public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
            {
            }

            public void OnActivityDestroyed(Activity activity)
            {
                _pendingActivityHolders.Remove(activity);
            }

            public void OnActivityPaused(Activity activity)
            { }

            public void OnActivityResumed(Activity activity)
            { }

            public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
            { }

            public void OnActivityStarted(Activity activity)
            { }

            public void OnActivityStopped(Activity activity)
            { }
        }
    }
}
