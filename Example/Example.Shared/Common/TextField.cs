using System;
using System.Drawing;
using Qoden.UI;

namespace Example
{
    public partial class TextField : QodenView, ILabeledField
    {
        [View]
        public QEditText Text { get; protected set; }

        [View]
        public QTextView Label { get; protected set; }

        public float LabelWidth { get; set; }

        protected override void OnLayout(LayoutBuilder layout)
        {
            this.LayoutLabeledField(layout);
        }
    }
}
