using System;
using System.Collections.Generic;
using Qoden.UI.Wrappers;

namespace Qoden.UI
{
#if __IOS__
    using PlatformView = UIKit.UIView;
    using PlatformViewGroup = UIKit.UIView;
#endif
#if __ANDROID__
    using PlatformView = Android.Views.View;
    using PlatformViewGroup = Android.Views.ViewGroup;
#endif

    //Note struct here for extra efficiency
    /// <summary>
    /// View builder provides methods to build view hierarchy which can be used in Shared code
    /// </summary>
    public struct ViewBuilder
    {
        public PlatformViewGroup Parent;
        public List<PlatformView> Views;
#if __ANDROID__
        public Android.Content.Context Context;
        object[] ViewConstructorArgs;
        public ViewBuilder(PlatformViewGroup parent, Android.Content.Context context)
        {
            Parent = parent;
            Context = context;
            Views = new List<PlatformView>();
            ViewConstructorArgs = new object[1];
            ViewConstructorArgs[0] = context;
        }
#endif
#if __IOS__
        public ViewBuilder(PlatformViewGroup parent)
        {
            Parent = parent;
            Views = new List<PlatformView>();
        }
#endif

        public PlatformView AddSubview(PlatformView view) 
        {
            Views.Add(view);
#if __ANDROID__
            view.Id = Android.Views.View.GenerateViewId();
#endif
            Parent.AsView().AddSubview(view);
            return view;
        }

        public T Add<T>(T view) where T : PlatformView
        {
            AddSubview(view);
            return view;
        }

        public T Add<T>() where T : PlatformView
        {
            return Add(Create<T>());
        }

        public PlatformView Add(Type t)
        {
            return AddSubview(Create(t));
        }

        public T Create<T>() where T : PlatformView
        {
            return (T)Create(typeof(T));
        }

        public PlatformView Create(Type t)
        {
#if __IOS__
            return (PlatformView)Activator.CreateInstance(t);
#endif
#if __ANDROID__
            return (PlatformView)Activator.CreateInstance(t, ViewConstructorArgs);
#endif
        }

        public void RemoveSubview(PlatformView view)
        {
            if (view != null)
            {
                view.AsView().RemoveFromSuperview();
                Views.Remove(view);
            }
        }

        public void Dispose()
        {
            foreach (var v in Views)
            {
                v.Dispose();
            }
        }
    }
}

