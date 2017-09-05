using System.Drawing;
using Xunit;

namespace Qoden.UI.Test
{
    public class LayoutBuilderTest
    {
        private LayoutBuilder Layout { get; } = new LayoutBuilder(new RectangleF(0, 0, 100, 100));

        [Fact]
        public void SameBoxAllTheTime()
        {
            var view = new PlatformView();
            var vv1 = Layout.View(view);
            var vv2 = Layout.View(view);
            Assert.Same(vv1, vv2);
        }

        [Fact]
        public void PreferredSize()
        {
            Assert.Equal(0, Layout.PreferredHeight);
            Assert.Equal(0, Layout.PreferredWidth);
            
            var view1 = new PlatformView();
            var view2 = new PlatformView();
            Layout.Padding = new EdgeInsets(10, 10, 10, 10);
            Layout.View(view1).Left(10).Top(10).Width(20).Height(20);
            Layout.View(view2).Left(40).Top(30).Width(10).Height(10);
            
            Assert.Equal(60, Layout.PreferredHeight);
            Assert.Equal(70, Layout.PreferredWidth);
            
            Assert.Equal(new RectangleF(20, 20, 40, 30), Layout.BoundingFrame(withPadding: false));
            Assert.Equal(new RectangleF(10, 10, 60, 50), Layout.BoundingFrame());
        }

        [Fact]
        public void PreferredSizeOverriden()
        {
            Layout.Padding = new EdgeInsets(10, 10, 10, 10);
            Layout.PreferredHeight = 10;
            Layout.PreferredWidth = 20;
            Assert.Equal(10, Layout.PreferredHeight);
            Assert.Equal(20, Layout.PreferredWidth);

            Layout.SetPreferredHeight(10, addPadding:true);
            Layout.SetPreferredWidth(20, addPadding:true);
            Assert.Equal(30, Layout.PreferredHeight);
            Assert.Equal(40, Layout.PreferredWidth);
        }

        [Fact]
        public void BoundingFrameForViews()
        {
            var view1 = new PlatformView();
            var view2 = new PlatformView();
            var view1Frame = Layout.View(view1).Left(10).Top(10).Width(20).Height(20);
            var view2Frame = Layout.View(view2).Left(40).Top(30).Width(10).Height(10);
            
            Assert.Equal(view1Frame.Frame, Layout.BoundingFrame(view1));
            Assert.Equal(
                RectangleF.Union(view1Frame.Frame, view2Frame.Frame), 
                Layout.BoundingFrame(view1, view2));
        }

        [Fact]
        public void PaddingsAndBounds()
        {
            Assert.Equal(new RectangleF(0, 0, 100, 100), Layout.PaddedOuterBounds);
            Assert.Equal(new RectangleF(0, 0, 100, 100), Layout.OuterBounds);
            
            Layout.Padding = new EdgeInsets(10, 10, 10, 10);
            Assert.Equal(new RectangleF(10, 10, 80, 80), Layout.PaddedOuterBounds);
            Assert.Equal(new RectangleF(0, 0, 100, 100), Layout.OuterBounds);
            
            //large paddings which are more than available bounds
            Layout.Padding = new EdgeInsets(50, 50, 60, 60);
            Assert.Equal(new RectangleF(50, 50, 0, 0), Layout.PaddedOuterBounds);
            Assert.Equal(new RectangleF(0, 0, 110, 110), Layout.OuterBounds);
        }

        [Fact]
        public void PaddingsAppliedToViewFrame()
        {
            Layout.Padding = new EdgeInsets(10, 10, 10, 10);
            var view1 = new PlatformView();
            var view1Frame = Layout.View(view1).Left(10).Top(10).Width(20).Height(20).Frame;
            Assert.Equal(new RectangleF(20, 20, 20, 20), view1Frame);
        }
    }
}