using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Qoden.UI
{
    public partial class QButton : QControl<UIButton>
    {
        public override UIButton Create(IViewHierarchyBuilder builder)
        {
            return new UIButton(UIButtonType.Custom);
        }
    }

    public static partial class QButtonExtensions
    {
        public static void SetText(this UIButton view, string text)
        {
            view.SetTitle(text, UIControlState.Normal);
        }

        public static void SetTextColor(this UIButton view, RGB color)
        {
            view.SetTitleColor(color.ToColor(), UIControlState.Normal);
        }

        public static void SetFont(this UIButton view, Font font)
        {
            view.Font = font.ToFont();
        }
    }
}
