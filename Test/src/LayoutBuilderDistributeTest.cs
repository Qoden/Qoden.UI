using System.Drawing;
using Xunit;

namespace Qoden.UI.Test
{
    public class LayoutBuilderDistributeTest
    {
        private LayoutBuilder Layout { get; } = new LayoutBuilder(new RectangleF(0, 0, 100, 100));

        [Fact]
        public void DistributeView()
        {
            var view1 = new PlatformView();
            var view2 = new PlatformView();
            Layout.Padding = new EdgeInsets(10, 10, 10, 10);
            var view1Box = Layout.View(view1).Left(10).Top(10).Width(20).Height(20);
            var view2Box = Layout.View(view2).Left(40).Top(30).Width(10).Height(10);

            Layout.DistributeHorizontally(view1, view2);
            Assert.Equal(new RectangleF(10, 20, 20, 20), view1Box.Frame);
            Assert.Equal(new RectangleF(80, 40, 10, 10), view2Box.Frame);
            
            Layout.DistributeVertically(view1, view2);
            Assert.Equal(new RectangleF(10, 10, 20, 20), view1Box.Frame);
            Assert.Equal(new RectangleF(80, 80, 10, 10), view2Box.Frame);
        }
    }
}