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
        public static T MakeView<T>(this IViewHierarchyBuilder builder) where T : View
        {
            return (T)builder.MakeView(typeof(T));
        }

        public static QView MakeQView<T>(this IViewHierarchyBuilder builder) where T : View
        {
            return new QView(builder.MakeView<T>());
        }
    }
}
