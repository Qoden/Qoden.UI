using System;
using System.Drawing;

namespace Qoden.UI.Test
{
    public class QView : BaseView<FakeView>
    {
        public QView()
        {
        }

        public QView(FakeView target) : base(target)
        {
        }
    }

    public class BaseView<T> : QView<T>, IViewGeometry where T : FakeView
    {
        public BaseView()
        {
        }

        public BaseView(T target) : base(target)
        {
        }

        public RectangleF Frame
        {
            get => PlatformView.Frame;
            set => PlatformView.Frame = value;
        }

        public IViewLayoutBox LayoutInBounds(RectangleF bounds, IUnit unit = null)
        {
            return new FakeViewLayoutBox(PlatformView, bounds);
        }

        public SizeF SizeThatFits(SizeF bounds)
        {
            return PlatformView.Frame.Size;
        }
    }
}
