using System;
using System.Drawing;
using Android.Content;
using Android.Widget;

namespace Qoden.UI
{
    public class QodenScrollView : ScrollView, ILayoutable
    {
        public ViewBuilder Builder { get => InnerView.Builder; }

        public Func<LayoutBuilder, SizeF> InnerLayout 
        {
            get => InnerView.LayoutAction;
            set => InnerView.LayoutAction = value;
        }

        ScrollViewLayout InnerView { get; set; }

        public QodenScrollView(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer) => Initialize();
        public QodenScrollView(Context context) : base(context) => Initialize();

        void Initialize()
        {
            InnerView = new ScrollViewLayout(Context);
            AddView(InnerView);
        }

        void ILayoutable.OnLayout(LayoutBuilder layout) => OnLayout(layout);

        protected void OnLayout(LayoutBuilder layout) 
        {
            layout.View(InnerView)
                  .AutoSize()
                  .Left(0).Right(0);
        }

        protected sealed override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var rect = new RectangleF(0, 0, r - l, b - t);
            var layoutBuilder = new LayoutBuilder(rect);
            OnLayout(layoutBuilder);
            foreach (var box in layoutBuilder.Views)
                box.Layout();
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Builder.Dispose();
            }
            base.Dispose(disposing);
        }

        class ScrollViewLayout : QodenView 
        {
            public ScrollViewLayout(Context context) : base(context) {}

            public Func<LayoutBuilder, SizeF> LayoutAction { get; set; }

            protected override void OnLayout(LayoutBuilder layout)
            {
                base.OnLayout(layout);
                var size = LayoutAction(layout);
                layout.SetPreferredHeight(size.Height);
            }
        }
    }
}
