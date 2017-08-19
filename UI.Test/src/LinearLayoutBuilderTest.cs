using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qoden.UI.Test
{
    [TestClass]
    public class LinearLayoutBuilderTest
    {
        PlatformView view1, view2, view3;
        LayoutBuilder builder;

        public LinearLayoutBuilderTest()
        {
            builder = new LayoutBuilder(new RectangleF(0, 0, 100, 100));

            view1 = new PlatformView(0, 0, 20, 30);
            view2 = new PlatformView(0, 0, 20, 20);
            view3 = new PlatformView(0, 0, 100, 100);
        }

        [TestMethod]
        public void LayoutStep()
        {
            var layout = StartFlowLayout(LinearLayoutDirection.TopBottom, LinearLayoutDirection.LeftRight);
            layout.LayoutStep = 10;
            layout.FlowStep = 5;
            layout.Add(new LayoutParams(view1));
            layout.Add(new LayoutParams(view2));
            layout.Add(new LayoutParams(view3));
            builder.Layout();
            AreEqualRectangles(new RectangleF(0, 0, 20, 30), view1.Frame);
            AreEqualRectangles(new RectangleF(0, 40, 20, 20), view2.Frame);
            AreEqualRectangles(new RectangleF(25, 0, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void AddOverflow()
        {
            var layout = StartFlowLayout(LinearLayoutDirection.LeftRight, LinearLayoutDirection.TopBottom);
            layout.Add(new LayoutParams(view1));
            layout.AddOverflow();
            layout.Add(new LayoutParams(view2));
            layout.AddOverflow();
            layout.Add(new LayoutParams(view3));
            builder.Layout();
            AreEqualRectangles(new RectangleF(0, 0, 20, 30), view1.Frame);
            AreEqualRectangles(new RectangleF(0, 30, 20, 20), view2.Frame);
            AreEqualRectangles(new RectangleF(0, 50, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void LeftRightTopBottom()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Top(5).Left(5);
            var layout = StartFlowLayout(LinearLayoutDirection.LeftRight, LinearLayoutDirection.TopBottom);
            layout.Add(new LayoutParams(view1, margins));
            layout.Add(new LayoutParams(view2, margins));
            layout.Add(new LayoutParams(view3, margins));
            builder.Layout();
            AreEqualRectangles(new RectangleF(5, 5, 20, 30), view1.Frame);
            AreEqualRectangles(new RectangleF(30, 5, 20, 20), view2.Frame);
            AreEqualRectangles(new RectangleF(5, 40, 100, 100), view3.Frame);
        }

        private LinearLayoutBuilder StartFlowLayout(LinearLayoutDirection layoutDirection, LinearLayoutDirection? flowDirection = null)
        {
            var layout = builder.LinearLayout(layoutDirection, flowDirection);
            layout.Flow = true;
            return layout;
        }

        [TestMethod]
        public void LeftRightBottomTop()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Bottom(5).Left(5);
            var layout = StartFlowLayout(LinearLayoutDirection.LeftRight, LinearLayoutDirection.BottomTop);
            layout.Add(new LayoutParams(view1, margins));
            layout.Add(new LayoutParams(view2, margins));
            layout.Add(new LayoutParams(view3, margins));
            builder.Layout();
            AreEqualRectangles(new RectangleF(5, 65, 20, 30), view1.Frame);
            AreEqualRectangles(new RectangleF(30, 75, 20, 20), view2.Frame);
            AreEqualRectangles(new RectangleF(5, -40, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void TopBottomLeftRight()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Top(5).Left(5);
            var layout = StartFlowLayout(LinearLayoutDirection.TopBottom);
            layout.Add(new LayoutParams(view1, margins));
            layout.Add(new LayoutParams(view2, margins));
            layout.Add(new LayoutParams(view3, margins));
            builder.Layout();
            AreEqualRectangles(new RectangleF(5, 5, 20, 30), view1.Frame);
            AreEqualRectangles(new RectangleF(5, 40, 20, 20), view2.Frame);
            AreEqualRectangles(new RectangleF(30, 5, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void TopBottomRightLeft()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Top(5).Right(5);
            var layout = StartFlowLayout(LinearLayoutDirection.TopBottom, LinearLayoutDirection.RightLeft);
            layout.Add(new LayoutParams(view1, margins));
            layout.Add(new LayoutParams(view2, margins));
            layout.Add(new LayoutParams(view3, margins));
            builder.Layout();
            AreEqualRectangles(new RectangleF(75, 5, 20, 30), view1.Frame);
            AreEqualRectangles(new RectangleF(75, 40, 20, 20), view2.Frame);
            AreEqualRectangles(new RectangleF(-30, 5, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void BottomTopLeftRight()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Bottom(5).Left(5);
            var layout = StartFlowLayout(LinearLayoutDirection.BottomTop, LinearLayoutDirection.LeftRight);
            layout.Add(new LayoutParams(view1, margins));
            layout.Add(new LayoutParams(view2, margins));
            layout.Add(new LayoutParams(view3, margins));
            builder.Layout();
            AreEqualRectangles(new RectangleF(5, 65, 20, 30), view1.Frame);
            AreEqualRectangles(new RectangleF(5, 40, 20, 20), view2.Frame);
            AreEqualRectangles(new RectangleF(30, -5, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void BottomTopRightLeft()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Bottom(5).Right(5);
            var layout = StartFlowLayout(LinearLayoutDirection.BottomTop, LinearLayoutDirection.RightLeft);
            layout.Add(new LayoutParams(view1, margins));
            layout.Add(new LayoutParams(view2, margins));
            layout.Add(new LayoutParams(view3, margins));
            builder.Layout();
            AreEqualRectangles(new RectangleF(75, 65, 20, 30), view1.Frame);
            AreEqualRectangles(new RectangleF(75, 40, 20, 20), view2.Frame);
            AreEqualRectangles(new RectangleF(-30, -5, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void RightLeftTopBottom()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Top(5).Right(5);
            var layout = StartFlowLayout(LinearLayoutDirection.RightLeft, LinearLayoutDirection.TopBottom);
            layout.Add(new LayoutParams(view1, margins));
            layout.Add(new LayoutParams(view2, margins));
            layout.Add(new LayoutParams(view3, margins));
            builder.Layout();
            AreEqualRectangles(new RectangleF(75, 5, 20, 30), view1.Frame);
            AreEqualRectangles(new RectangleF(50, 5, 20, 20), view2.Frame);
            AreEqualRectangles(new RectangleF(-5, 40, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void RightLeftBottomTop()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Bottom(5).Right(5);
            var layout = StartFlowLayout(LinearLayoutDirection.RightLeft, LinearLayoutDirection.BottomTop);
            layout.Add(new LayoutParams(view1, margins));
            layout.Add(new LayoutParams(view2, margins));
            layout.Add(new LayoutParams(view3, margins));
            builder.Layout();
            AreEqualRectangles(new RectangleF(75, 65, 20, 30), view1.Frame);
            AreEqualRectangles(new RectangleF(50, 75, 20, 20), view2.Frame);
            AreEqualRectangles(new RectangleF(-5, -40, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void LayoutBounds()
        {
            var nonZeroBounds = new RectangleF(20, 30, 100, 100);
            builder = new LayoutBuilder(nonZeroBounds);
            var layout = StartFlowLayout(LinearLayoutDirection.TopBottom);
            layout.Add(new LayoutParams(view1));
            layout.Add(new LayoutParams(view2));
            builder.Layout();
            AreEqualRectangles(new RectangleF(20, 30, 20, 30), view1.Frame);
            AreEqualRectangles(new RectangleF(20, 60, 20, 20), view2.Frame);
        }

        void AreEqualRectangles(RectangleF expected, RectangleF actual)
        {
            actual = new RectangleF((float)Math.Round(actual.Left),
                                    (float)Math.Round(actual.Top),
                                    (float)Math.Round(actual.Width),
                                    (float)Math.Round(actual.Height));
            Assert.AreEqual(expected, actual);
        }
    }
}
