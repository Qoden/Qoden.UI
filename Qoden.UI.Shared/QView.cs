using System;
using System.Collections.Generic;
using System.Drawing;
using Qoden.Binding;

namespace Qoden.UI
{
#if __IOS__
    using View = UIKit.UIView;
    using ViewGroup = UIKit.UIView;
    using Control = UIKit.UIControl;
#endif
#if __ANDROID__
    using View = Android.Views.View;
    using ViewGroup = Android.Views.ViewGroup;
    using Control = Android.Views.View;
#endif

    public interface IPlatformView : IViewGeometry, IQView<View>
    {
    }

    public partial class BaseView<T> : QView<T>, IPlatformView where T : View
    {
        public BaseView()
        {
        }

        public BaseView(T target) : base(target)
        {
        }

        public RectangleF Frame
        {
            get  
            {
                return PlatformView.Frame();
            }
            set
            {
                PlatformView.SetFrame(value);
            }
        }

        public IViewLayoutBox MakeViewLayoutBox(RectangleF bounds, IUnit unit = null)
        {
            return new PlatformViewLayoutBox(this, bounds, unit);
        }

        public SizeF SizeThatFits(SizeF bounds)
        {
            return QView_Extensions.SizeThatFits(PlatformView, bounds);
        }

        //View IPlatformView.PlatformView => PlatformView;
        View IQView<View>.PlatformView => PlatformView;
        View IQView<View>.Create(IViewHierarchyBuilder builder)
        {
            return Create(builder);
        }
    }

    public partial class QView
    {
        public QView()
        {
        }

        public QView(View target) : base(target)
        {
        }

        public static T Wrap<T>(object platformView) where T : IViewWrapper, new()
        {
            return new T()
            {
                PlatformView = platformView
            };
        }
    }

    public partial class QViewGroup : BaseView<ViewGroup>
    {
        public QViewGroup()
        {
        }

        public QViewGroup(ViewGroup target) : base(target)
        {
        }
    }

    public static partial class QView_Extensions
    {
        public static void SetBackgroundColor(this IQView<View> view, RGB bgColor)
        {
            view.PlatformView.SetBackgroundColor(bgColor);
        }

        public static void SetPadding(this IQView<View> view, EdgeInsets padding)
        {
            view.PlatformView.SetPadding(padding);
        }

        public static RectangleF Frame(this IQView<View> view)
        {
            return view.PlatformView.Frame();
        }

        public static void SetFrame(this IQView<View> view, RectangleF value)
        {
            view.PlatformView.SetFrame(value);
        }

        public static void SetVisibility(this IQView<View> view, bool visible)
        {
            view.PlatformView.SetVisibility(visible);
        }

        public static bool GetVisibility(this IQView<View> view)
        {
            return view.PlatformView.GetVisibility();
        }

        public static IEnumerable<View> Subviews(this IQView<ViewGroup> view)
        {
            return view.PlatformView.Subviews();
        }

        public static void SetEnabled(this IQView<Control> view, bool enabled)
        {
            view.PlatformView.SetEnabled(enabled);
        }
    }
}
