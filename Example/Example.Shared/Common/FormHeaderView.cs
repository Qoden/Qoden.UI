using System;
using Qoden.UI;

namespace Example
{
    public partial class FormHeaderView : QodenView
    {
        [View]
        public QTextView Title { get; private set; }
        [View]
        public QButton Done { get; private set; }
        [View]
        public QButton Cancel { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();
            DisplayButtons = true;
        }

        protected override void OnLayout(LayoutBuilder layout)
        {
            if (DisplayButtons)
            {
                layout.View(Cancel)
                      .Left(17).CenterVertically().AutoSize();
                layout.View(Title)
                      .CenterHorizontally().CenterVertically().AutoSize();
                layout.View(Done)
                      .Right(17).CenterVertically().AutoSize();
            }
            else
            {
                layout.View(Title)
                      .Left(20).Top(46).AutoSize();
            }
        }

        public bool DisplayButtons
        {
            get { return Done.GetVisibility(); }
            set
            {
                Done.SetVisibility(value);
                Cancel.SetVisibility(value);
            }
        }

        public float MyHeight { get; internal set; } = 80;
    }
}
