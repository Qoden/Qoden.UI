using System;
namespace Qoden.UI
{
#if __IOS__
    using View = UIKit.UIView;
#endif
#if __ANDROID__
    using View = Android.Views.View;
#endif
    public static class LinearLayoutBuilder_Extensions
    {
        public static LinearLayoutBuilder Add(this LinearLayoutBuilder builder, View view, Action<IViewLayoutBox> layout = null)
        {
            return builder.Add(new QView(view), layout);
        }

        public static LinearLayoutBuilder Add(this LinearLayoutBuilder builder, IPlatformView view, Action<IViewLayoutBox> layout = null)
        {
            return builder.Add(new LayoutParams(view, layout));
        }
    }
}
