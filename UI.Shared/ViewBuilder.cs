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
        public ViewBuilder(PlatformViewGroup parent, Android.Content.Context context)
        {
            Parent = parent;
            Context = context;
            Views = new List<PlatformView>();
        }
#endif
#if __IOS__
        public ViewBuilder(PlatformViewGroup parent)
        {
            Parent = parent;
            Views = new List<PlatformView>();
        }
#endif
        public T AddSubview<T>(T view) where T : PlatformView
        {            
            Views.Add(view);
#if __ANDROID__
            view.Id = Views.Count;
#endif
            Parent.AsView().AddSubview(view);
            return view;
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

