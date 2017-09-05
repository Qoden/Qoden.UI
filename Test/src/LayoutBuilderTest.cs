using System.Drawing;
using Xunit;

namespace Qoden.UI.Test
{
    public class LayoutBuilderTest
    {
        readonly LayoutBuilder _layout = new LayoutBuilder(new RectangleF(0, 0, 100, 100));
        
        [Fact]
        public void SameBoxAllTheTime()
        {
            var view = new PlatformView();
            var vv1 = _layout.View(view);
            var vv2 = _layout.View(view);
            Assert.Same(vv1, vv2);
        }

        [Fact]
        public void PreferredSize()
        {
            Assert.Equal(0, _layout.PreferredHeight);
            Assert.Equal(0, _layout.PreferredWidth);
            
            var view1 = new PlatformView();
            var view2 = new PlatformView();
            _layout.Padding = new EdgeInsets(10, 10, 10, 10);
            _layout.View(view1).Left(10).Top(10).Width(20).Height(20);
            _layout.View(view2).Left(40).Top(30).Width(10).Height(10);
            
            Assert.Equal(60, _layout.PreferredHeight);
            Assert.Equal(70, _layout.PreferredWidth);
            
            Assert.Equal(new RectangleF(20, 20, 40, 30), _layout.BoundingFrame(withPadding: false));
            Assert.Equal(new RectangleF(10, 10, 60, 50), _layout.BoundingFrame());
        }

        [Fact]
        public void PreferredSizeOverriden()
        {
            _layout.Padding = new EdgeInsets(10, 10, 10, 10);
            _layout.PreferredHeight = 10;
            _layout.PreferredWidth = 20;
            Assert.Equal(10, _layout.PreferredHeight);
            Assert.Equal(20, _layout.PreferredWidth);

            _layout.SetPreferredHeight(10, addPadding:true);
            _layout.SetPreferredWidth(20, addPadding:true);
            Assert.Equal(30, _layout.PreferredHeight);
            Assert.Equal(40, _layout.PreferredWidth);
        }

        [Fact]
        public void BoundingFrameForViews()
        {
            var view1 = new PlatformView();
            var view2 = new PlatformView();
            var view1Frame = _layout.View(view1).Left(10).Top(10).Width(20).Height(20);
            var view2Frame = _layout.View(view2).Left(40).Top(30).Width(10).Height(10);
            
            Assert.Equal(view1Frame.Frame, _layout.BoundingFrame(view1));
            Assert.Equal(
                RectangleF.Union(view1Frame.Frame, view2Frame.Frame), 
                _layout.BoundingFrame(view1, view2));
        }

        [Fact]
        public void PaddingsAndBounds()
        {
            Assert.Equal(new RectangleF(0, 0, 100, 100), _layout.PaddedOuterBounds);
            Assert.Equal(new RectangleF(0, 0, 100, 100), _layout.OuterBounds);
            
            _layout.Padding = new EdgeInsets(10, 10, 10, 10);
            Assert.Equal(new RectangleF(10, 10, 80, 80), _layout.PaddedOuterBounds);
            Assert.Equal(new RectangleF(0, 0, 100, 100), _layout.OuterBounds);
            
            //large paddings which are more than available bounds
            _layout.Padding = new EdgeInsets(50, 50, 60, 60);
            Assert.Equal(new RectangleF(50, 50, 0, 0), _layout.PaddedOuterBounds);
            Assert.Equal(new RectangleF(0, 0, 110, 110), _layout.OuterBounds);
        }

        [Fact]
        public void PaddingsAppliedToViewFrame()
        {
            _layout.Padding = new EdgeInsets(10, 10, 10, 10);
            var view1 = new PlatformView();
            var view1Frame = _layout.View(view1).Left(10).Top(10).Width(20).Height(20).Frame;
            Assert.Equal(new RectangleF(20, 20, 20, 20), view1Frame);
        }
    }
}