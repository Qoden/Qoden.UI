using System;
namespace Qoden.UI
{
#if __IOS__
    using View = UIKit.UIView;
#endif
#if __ANDROID__
    using View = Android.Views.View;
#endif

    public static class ViewHierarchyBuilder_Extensions
    {
        public static QView<T> MakeQView<T>(this IViewHierarchyBuilder builder) where T : View
        {
            var platformView = builder.MakeView(typeof(T));
            var qView = new QView<T>();
            qView.PlatformView = (T)platformView;
            return qView;
        }

        public static T MakeView<T>(this IViewHierarchyBuilder builder) where T : View
        {
            return (T)builder.MakeView(typeof(T));
        }
    }
}
