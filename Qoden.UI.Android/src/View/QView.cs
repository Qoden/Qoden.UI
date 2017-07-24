using System;
using System.Drawing;
using Android.Views;

namespace Qoden.UI
{
    public interface IPlatformView<out T> : IQView<T>, IViewGeometry where T : View
    { }

    public class BaseView<T> : QView<T>, IPlatformView<T> where T : View
    {
        public BaseView()
        {
        }

        public BaseView(T target) : base(target)
        {
        }

        public RectangleF Frame
        {
            get => PlatformView.Frame();
            set
            {
                PlatformView.Layout((int)Math.Round(value.Left),
		                            (int)Math.Round(value.Top),
		                            (int)Math.Round(value.Right),
		                            (int)Math.Round(value.Bottom));
            }
        }

        public IViewLayoutBox LayoutInBounds(RectangleF bounds, IUnit unit = null)
        {
            return new PlatformViewLayoutBox(this, bounds, unit);
        }

        public SizeF SizeThatFits(SizeF bounds)
        {
            return PlatformView.SizeThatFits(bounds);
        }
    }

    public class QView : BaseView<View>
    {
        public QView()
        {
        }

        public QView(View target) : base(target)
        {
        }
    }

    public static class ViewExtensions
    {
        public static QView<T> AsQView<T>(this T view) where T : View
        {
            return new QView<T> { PlatformView = view };
        }

        public static void SetBackgroundColor(this IQView<View> view, RGB bgColor)
        {
            view.PlatformView.SetBackgroundColor(bgColor);
        }

        public static void SetBackgroundColor(this View view, RGB bgColor)
        {
            view.SetBackgroundColor(bgColor.ToColor());
        }

        public static void SetPadding(this IQView<View> view, EdgeInset padding)
        {
            view.PlatformView.SetPadding(padding);
        }

        public static void SetPadding(this View view, EdgeInset padding)
        {
            view.SetPadding((int)Math.Round(padding.Left),
                            (int)Math.Round(padding.Top),
                            (int)Math.Round(padding.Right),
                            (int)Math.Round(padding.Bottom));
        }

        public static RectangleF Frame(this IQView<View> view)
        {
            return view.PlatformView.Frame();
        }

        public static RectangleF Frame(this View view)
        {
            return new RectangleF(view.Left, view.Top, view.Width, view.Height);
        }

        public static void SetVisibility(this IQView<View> view, bool visible)
        {
            view.PlatformView.SetVisibility(visible);
        }

        public static void SetVisibility(this View view, bool visible)
        {
            view.Visibility = visible ? ViewStates.Visible : ViewStates.Gone;
        }

        public static bool GetVisibility(this IQView<View> view)
        {
            return view.PlatformView.GetVisibility();
        }

        public static bool GetVisibility(this View view)
        {
            return view.Visibility == ViewStates.Visible;
        }
    }
}
