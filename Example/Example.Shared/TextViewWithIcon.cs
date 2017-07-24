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

        protected override void OnLayout(LayoutBuilder layout)
        {
            var icon = layout.View(Icon)
                .AutoSize().Left(15).CenterVertically();

            layout.View(Title)
                 .After(icon.LayoutBounds, 16).Right(16).CenterVertically().AutoHeight();
        }
    }
}
