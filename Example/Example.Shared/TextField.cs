using System;
using System.Drawing;
using Qoden.UI;

namespace Example
{
    public partial class TextField : QodenView
    {
        [View]
        public QEditText Text { get; private set; }

        [View]
        public QTextView Label { get; private set; }

        public float LabelWidth { get; set; }

        protected override void OnLayout(LayoutBuilder layout)
        {
            var label = layout.View(Label)
                 .Left(20).AutoHeight().Width(Pixel.Val(LabelWidth)).CenterVertically();
            layout.View(Text)
                .After(label.LayoutBounds, 9.5f).AutoHeight().Right(0).CenterVertically();
        }
    }
}
