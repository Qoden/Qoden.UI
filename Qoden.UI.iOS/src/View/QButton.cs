using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Qoden.UI
{
    public static class UIButtonExtensions
    {
        public static void SetText(this IQView<UIButton> view, string text)
        {
            view.PlatformView.SetText(text);
        }

        public static void SetText(this UIButton view, string text)
        {
            view.SetTitle(text, UIControlState.Normal);
        }
    }

    public class QButton : BaseView<UIButton>
    {
        public QButton()
        {
        }

        public QButton(UIButton target) : base(target)
        {
        }

        public override UIButton Create(IViewHierarchyBuilder builder)
        {
            return new UIButton(UIButtonType.Custom);
        }
    }
}
