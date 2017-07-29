using System;
using Android.App;
using Android.OS;
using Android.Runtime;

namespace Qoden.UI
{
    public class QodenApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public static Activity ActiveActivity { get; private set; }

        public QodenApplication(IntPtr handle, JniHandleOwnership transer) :base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            ActiveActivity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            ActiveActivity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
            ActiveActivity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
        }
    }
}
