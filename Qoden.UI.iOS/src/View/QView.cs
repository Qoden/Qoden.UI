using System;
using System.Collections.Generic;
using System.Drawing;
using Qoden.Binding;
using UIKit;

namespace Qoden.UI
{
    public partial class BaseView<T> : QView<T>, IPlatformView where T : UIView
    {

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
        static BaseView()
        {
            if (LinkerTrick.False)
            {
                new UIView();
            }
        }
#pragma warning restore RECS0026 // Possible unassigned object created by 'new' 
    }

    public partial class QView : BaseView<UIView>
    {
    }

    public partial class QViewGroup : BaseView<UIView>
    {
    }

    public partial class QControl<T> : BaseView<T> where T : UIControl
    {
        public QControl()
        {
        }

        public QControl(T target) : base(target)
        {
        }

        public EventHandlerSource<T> ClickTarget()
        {
            return PlatformView.ClickTarget();
        }
    }

    public static partial class QView_Extensions
    {
        public static IEnumerable<UIView> Subviews(this UIView view)
        {
            foreach (var v in view.Subviews)
            {
                yield return v;
            }
        }

        public static void SetBackgroundColor(this UIView view, RGB bgColor)
        {
            view.BackgroundColor = bgColor.ToColor();
        }

        public static void SetPadding(this UIView view, EdgeInset padding)
        {
            view.LayoutMargins = new UIEdgeInsets(padding.Left,
                                                  padding.Top,
                                                  padding.Right,
                                                  padding.Bottom);
        }

        public static RectangleF Frame(this UIView view)
        {
            return (RectangleF)view.Frame;
        }

        public static void SetFrame(this UIView view, RectangleF frame)
        {
            view.Frame = frame;
        }

        public static void SetVisibility(this UIView view, bool visible)
        {
            view.Hidden = !visible;
        }

        public static bool GetVisibility(this UIView view)
        {
            return !view.Hidden;
        }

        public static void SetEnabled(this UIControl view, bool enabled)
        {
            view.Enabled = enabled;
        }

        public static SizeF SizeThatFits(this UIView view, SizeF bounds)
        {
            return (SizeF)view.SizeThatFits(bounds);
        }
    }
}
