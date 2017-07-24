using System;
using System.Drawing;
using Qoden.UI;

namespace Example
{
    public partial class TodoListViewItem
    {
        [View]
        public QTextView TitleLabel { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();
            this.SetBackgroundColor(new RGB(0, 255, 0));
            this.SetPadding(new EdgeInset(10, 10, 10, 10));
            TitleLabel.SetBackgroundColor(new RGB(255, 0, 0));
            TitleLabel.SetTextColor(new RGB(0, 0, 0));
#if __ANDROID__
            this.SetMinimumHeight(Units.Dp.ToIntPixels(60));
#endif
        }

        protected override void OnLayout(LayoutBuilder layout)
        {
            layout.View(TitleLabel).Left(0).Right(0).Top(0).Bottom(0);
        }
    }
}
