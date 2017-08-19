using System;
using Android.Support.V4.App;

namespace Qoden.UI
{
    public interface IControllerHost
    {
        ChildViewControllersList ChildControllers { get; }
    }

    public static class ControllerHost_Extensions
    {
        public static T GetChildViewController<T>(this IControllerHost host, string key, Func<T> factory) where T : Fragment
        {
            return host.ChildControllers.GetChildViewController(key, factory);
        }

        public static T GetChildViewController<T>(this IControllerHost host) where T : Fragment, new()
        {
            return host.GetChildViewController<T>(typeof(T).FullName);
        }

        public static T GetChildViewController<T>(this IControllerHost host, string key) where T : Fragment, new()
        {
            return host.GetChildViewController(key, () => new T());
        }
    }
}
