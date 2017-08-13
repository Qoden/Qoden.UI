using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Qoden.UI
{
    public partial class QButton : QControl<UIButton>
    {
#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
        static QButton()
        {
            if (LinkerTrick.False)
            {
                new UIButton(UIButtonType.Custom);
            }
        }
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

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
            view.SizeToFit();
        }

        public static void SetTextColor(this UIButton view, RGB color)
        {
            view.SetTitleColor(color.ToColor(), UIControlState.Normal);
        }

        public static void SetFont(this UIButton view, Font font)
        {
            view.Font = font.ToFont();
        }

        public static void SetTextAlignment(this UIButton view, TextAlignment alignment)
        {
            view.TitleLabel.TextAlignment = alignment.ToUITextAlignment();
        }
    }
}
