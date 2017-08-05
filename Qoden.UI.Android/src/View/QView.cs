using System;
using System.Collections.Generic;
using System.Drawing;
using Android.Views;
using Qoden.Binding;

namespace Qoden.UI
{
    public partial class BaseView<T>
    {
        public EventHandlerSource<T> ClickTarget()
        {
            return PlatformView.ClickTarget();
        }
    }

    public partial class QView : BaseView<View>
    {
    }

    public partial class QViewGroup : BaseView<ViewGroup>
    {
    }

    public static partial class QView_Extensions
    {
        public static void SetBackgroundColor(this View view, RGB bgColor)
        {
            view.SetBackgroundColor(bgColor.ToColor());
        }

        public static void SetPadding(this View view, EdgeInsets padding)
        {
            view.SetPadding((int)Math.Round(padding.Left),
                            (int)Math.Round(padding.Top),
                            (int)Math.Round(padding.Right),
                            (int)Math.Round(padding.Bottom));
        }

        public static RectangleF Bounds(this View view)
        {
            return new RectangleF(0, 0, view.Width, view.Height);
        }

        public static RectangleF Frame(this View view)
        {
            return new RectangleF(view.Left, view.Top, view.Width, view.Height);
        }

        public static void SetFrame(this View view, RectangleF value)
        {
            view.Layout((int)Math.Round(value.Left),
                        (int)Math.Round(value.Top),
                        (int)Math.Round(value.Right),
                        (int)Math.Round(value.Bottom));
        }

        public static void SetVisibility(this View view, bool visible)
        {
            view.Visibility = visible ? ViewStates.Visible : ViewStates.Gone;
        }

        public static bool GetVisibility(this View view)
        {
            return view.Visibility == ViewStates.Visible;
        }

        public static IEnumerable<View> Subviews(this ViewGroup view)
        {
            var count = view.ChildCount;
            for (int i = 0; i < count; ++i)
            {
                yield return view.GetChildAt(i);
            }
        }

        public static void SetEnabled(this View view, bool enabled)
        {
            view.Enabled = enabled;
        }

        public static SizeF SizeThatFits(this View v, SizeF parentBounds)
        {
            var ws = View.MeasureSpec.MakeMeasureSpec((int)Math.Round(parentBounds.Width), MeasureSpecMode.AtMost);
            var hs = View.MeasureSpec.MakeMeasureSpec((int)Math.Round(parentBounds.Height), MeasureSpecMode.AtMost);
            v.Measure(ws, hs);
            return new SizeF(v.MeasuredWidth, v.MeasuredHeight);
        }
    }
}
