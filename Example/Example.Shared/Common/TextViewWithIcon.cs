using System;
using Qoden.UI;

namespace Example
{
    public partial class TextViewWithIcon : QodenView
    {
        [View]
        public QTextView Icon { get; private set; }

        [View]
        public QTextView Title { get; private set; }
        public int VerticalAdjustment { get; set; } = 0;
        public int IconSpacing { get; set; } = 16;

        protected override void OnLayout(LayoutBuilder layout)
        {
            var icon = layout.View(Icon)
                .AutoSize().Left(15).CenterVertically(VerticalAdjustment);

            layout.View(Title)
                 .After(icon.LayoutBounds, IconSpacing).Right(16).CenterVertically(VerticalAdjustment).AutoHeight();
        }
    }
}
