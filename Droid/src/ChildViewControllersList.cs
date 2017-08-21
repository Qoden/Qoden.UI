using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Qoden.Validation;

namespace Qoden.UI
{
    public class ChildViewControllersList
    {
        List<Fragment> _pendingFragments = new List<Fragment>();
        FragmentManager _manager;
        Context _context;

        public ChildViewControllersList(Context context, FragmentManager manager)
        {
            Assert.Argument(context, nameof(context)).NotNull();
            Assert.Argument(manager, nameof(manager)).NotNull();

            _manager = manager;
            _context = context;
            _manager.RegisterFragmentLifecycleCallbacks(new CleanupChildControllers(this), false);
        }

        public T GetChildViewController<T>(string key, Func<T> factory) where T : Fragment
        {
            var fragment = (T)_manager.FindFragmentByTag(key);
            if (fragment == null)
            {
                fragment = (T)_pendingFragments.Where(x => x.Tag == key).FirstOrDefault();
            }
            if (fragment == null)
            {
                fragment = factory();
                _pendingFragments.Add(fragment);
                if (!(fragment is DialogFragment))
                {
                    //Do not add dialog fragments to fragment managers since this results
                    //in dialog being displayed right away.
                    //Instead use Show method on DialogFragment to show it.
                    //Show method also adds fragment to manager.
                    _manager.BeginTransaction()
                           .Add(fragment, key)
                           .CommitAllowingStateLoss();    
                }

                var dc = fragment as IDetachedController;
                if (dc != null)
                {
                    dc.FragmentManager = _manager;
                    dc.Context = _context;
                }
            }
            return fragment;
        }

        class CleanupChildControllers : FragmentManager.FragmentLifecycleCallbacks
        {
            ChildViewControllersList _list;

            public CleanupChildControllers(ChildViewControllersList list)
            {
                _list = list;
            }

            public override void OnFragmentDestroyed(FragmentManager fm, Fragment f)
            {
                base.OnFragmentDestroyed(fm, f);
                _list._pendingFragments.Remove(f);
            }

            public override void OnFragmentCreated(FragmentManager fm, Fragment f, Bundle savedInstanceState)
            {
                base.OnFragmentCreated(fm, f, savedInstanceState);
                _list._pendingFragments.Remove(f);
            }
        }
    }
}
