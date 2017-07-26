using System;
using Qoden.UI;

namespace Example
{
    public partial class PhoneNumberField : QodenView, ILabeledField
    {
        [View]
        public QTextView Label { get; private set; }
        [View]
        public QEditText Text { get; private set; }

        public float LabelWidth { get; set; }

        protected override void CreateView()
        {
            base.CreateView();
            PlatformCreate();
        }

        protected override void OnLayout(LayoutBuilder layout)
        {
            this.LayoutLabeledField(layout);
        }
    }
}
