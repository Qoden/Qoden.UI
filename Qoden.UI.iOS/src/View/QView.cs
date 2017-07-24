using System;
using System.Collections.Generic;
using System.Drawing;
using UIKit;

namespace Qoden.UI
{
    public interface IPlatformView<out T> : IQView<T>, IViewGeometry where T : UIView
    {}

    public class BaseView<T> : QView<T>, IPlatformView<T> where T : UIView
    {
        public BaseView()
        {
        }

        public BaseView(T target) : base(target)
        {
        }

        public RectangleF Frame 
        {
            get => (RectangleF)PlatformView.Frame;
            set => PlatformView.Frame = value;
        }

        public IViewLayoutBox LayoutInBounds(RectangleF bounds, IUnit unit = null)
        {
            return new PlatformViewLayoutBox(this, bounds, unit);
        }

        public SizeF SizeThatFits(SizeF bounds)
        {
            return (SizeF)PlatformView.SizeThatFits(bounds);
        }
    }

    public class QView : BaseView<UIView>
    {
        public QView()
        {
        }

        public QView(UIView target) : base(target)
        {
        }
    }

    public class QViewGroup : BaseView<UIView>
    {
        public QViewGroup()
        {
        }

        public QViewGroup(UIView target) : base(target)
        {
        }
    }

    public static class UIViewExtensions
    {
        public static QView<T> AsQView<T>(this T view) where T : UIView
        {
            return new QView<T> { PlatformView = view };
        }

        public static IEnumerable<UIView> Subviews(this IQView<UIView> view)
        {
            return view.PlatformView.Subviews();
        }

        public static IEnumerable<UIView> Subviews(this UIView view)
        {
            foreach (var v in view.Subviews)
            {
                yield return v;
            }
        }

        public static void SetBackgroundColor(this IQView<UIView> view, RGB bgColor)
        {
            view.PlatformView.SetBackgroundColor(bgColor);
        }

        public static void SetBackgroundColor(this UIView view, RGB bgColor)
        {
            view.BackgroundColor = bgColor.ToColor();
        }

        public static void SetPadding(this IQView<UIView> view, EdgeInset padding)
        {
            view.PlatformView.SetPadding(padding);
        }

        public static void SetPadding(this UIView view, EdgeInset padding)
        {
            view.LayoutMargins = new UIEdgeInsets(padding.Left,
                                                  padding.Top,
                                                  padding.Right,
                                                  padding.Bottom);
        }

        public static RectangleF Frame(this IQView<UIView> view)
        {
            return view.PlatformView.Frame();
        }

        public static RectangleF Frame(this UIView view)
        {
            return (RectangleF)view.Frame;
        }

        public static void SetVisibility(this IQView<UIView> view, bool visible)
        {
            view.PlatformView.SetVisibility(visible);
        }

        public static void SetVisibility(this UIView view, bool visible)
        {
            view.Hidden = !visible;
        }

        public static bool GetVisibility(this IQView<UIView> view)
        {
            return view.PlatformView.GetVisibility();
        }

        public static bool GetVisibility(this UIView view)
        {
            return !view.Hidden;
        }
    }
}
