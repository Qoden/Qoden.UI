using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qoden.UI.Test
{
    [TestClass]
    public class LinearLayoutBuilderTest : QodenUITest
    {
        LinearLayoutBuilder layout;
        QView view1, view2, view3;
        LayoutBuilder builder;

        public LinearLayoutBuilderTest()
        {
            builder = new LayoutBuilder(new RectangleF(0, 0, 100, 100));
            layout = new LinearLayoutBuilder();
            layout.Flow = true;

            view1 = new QView(new FakeView(0, 0, 20, 30));
            view2 = new QView(new FakeView(0, 0, 20, 20));
            view3 = new QView(new FakeView(0, 0, 100, 100));
        }

        [TestMethod]
        public void LeftRightTopBottom()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Top(5).Left(5);
            layout.StartLayout(builder, LayoutDirection.LeftRight, LayoutDirection.TopBottom);
            layout.Add(new LayoutParams(view1, margins))
                  .Add(new LayoutParams(view2, margins))
                  .Add(new LayoutParams(view3, margins));
            builder.Layout();
            Assert.AreEqual(new RectangleF(5, 5, 20, 30), view1.Frame);
            Assert.AreEqual(new RectangleF(30, 5, 20, 20), view2.Frame);
            Assert.AreEqual(new RectangleF(5, 40, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void LeftRightBottomTop()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Bottom(5).Left(5);
            layout.StartLayout(builder, LayoutDirection.LeftRight, LayoutDirection.BottomTop);
            layout.Add(new LayoutParams(view1, margins))
                  .Add(new LayoutParams(view2, margins))
                  .Add(new LayoutParams(view3, margins));
            builder.Layout();
            Assert.AreEqual(new RectangleF(5, 65, 20, 30), view1.Frame);
            Assert.AreEqual(new RectangleF(30, 75, 20, 20), view2.Frame);
            Assert.AreEqual(new RectangleF(5, -40, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void TopBottomLeftRight()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Top(5).Left(5);
            layout.StartLayout(builder, LayoutDirection.TopBottom);
            layout.Add(new LayoutParams(view1, margins))
                  .Add(new LayoutParams(view2, margins))
                  .Add(new LayoutParams(view3, margins));
            builder.Layout();
            Assert.AreEqual(new RectangleF(5, 5, 20, 30), view1.Frame);
            Assert.AreEqual(new RectangleF(5, 40, 20, 20), view2.Frame);
            Assert.AreEqual(new RectangleF(30, 5, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void TopBottomRightLeft()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Top(5).Right(5);
            layout.StartLayout(builder, LayoutDirection.TopBottom, LayoutDirection.RightLeft);
            layout.Add(new LayoutParams(view1, margins))
                  .Add(new LayoutParams(view2, margins))
                  .Add(new LayoutParams(view3, margins));
            builder.Layout();                  
            Assert.AreEqual(new RectangleF(75, 5, 20, 30), view1.Frame);
            Assert.AreEqual(new RectangleF(75, 40, 20, 20), view2.Frame);
            Assert.AreEqual(new RectangleF(-30, 5, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void BottomTopLeftRight()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Bottom(5).Left(5);
            layout.StartLayout(builder, LayoutDirection.BottomTop, LayoutDirection.LeftRight);
            layout.Add(new LayoutParams(view1, margins))
                  .Add(new LayoutParams(view2, margins))
                  .Add(new LayoutParams(view3, margins));
            builder.Layout();                  
            Assert.AreEqual(new RectangleF(5, 65, 20, 30), view1.Frame);
            Assert.AreEqual(new RectangleF(5, 40, 20, 20), view2.Frame);
            Assert.AreEqual(new RectangleF(30, -5, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void BottomTopRightLeft()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Bottom(5).Right(5);
            layout.StartLayout(builder, LayoutDirection.BottomTop, LayoutDirection.RightLeft);
            layout.Add(new LayoutParams(view1, margins))
                  .Add(new LayoutParams(view2, margins))
                  .Add(new LayoutParams(view3, margins));
            builder.Layout();                  
            Assert.AreEqual(new RectangleF(75, 65, 20, 30), view1.Frame);
            Assert.AreEqual(new RectangleF(75, 40, 20, 20), view2.Frame);
            Assert.AreEqual(new RectangleF(-30, -5, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void RightLeftTopBottom()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Top(5).Right(5);
            layout.StartLayout(builder, LayoutDirection.RightLeft, LayoutDirection.TopBottom);
            layout.Add(new LayoutParams(view1, margins))
                  .Add(new LayoutParams(view2, margins))
                  .Add(new LayoutParams(view3, margins));
            builder.Layout();                            
            Assert.AreEqual(new RectangleF(75, 5, 20, 30), view1.Frame);
            Assert.AreEqual(new RectangleF(50, 5, 20, 20), view2.Frame);
            Assert.AreEqual(new RectangleF(-5, 40, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void RightLeftBottomTop()
        {
            Action<IViewLayoutBox> margins = v => v.AutoSize().Bottom(5).Right(5);
            layout.StartLayout(builder, LayoutDirection.RightLeft, LayoutDirection.BottomTop);
            layout.Add(new LayoutParams(view1, margins))
                  .Add(new LayoutParams(view2, margins))
                  .Add(new LayoutParams(view3, margins));
            builder.Layout();                  
            Assert.AreEqual(new RectangleF(75, 65, 20, 30), view1.Frame);
            Assert.AreEqual(new RectangleF(50, 75, 20, 20), view2.Frame);
            Assert.AreEqual(new RectangleF(-5, -40, 100, 100), view3.Frame);
        }

        [TestMethod]
        public void LayoutBounds()
        {
            var nonZeroBounds = new RectangleF(20, 30, 100, 100);
            builder = new LayoutBuilder(nonZeroBounds);
            layout.StartLayout(builder, LayoutDirection.TopBottom);
            layout.Add(new LayoutParams(view1))
                  .Add(new LayoutParams(view2));
            builder.Layout();                  
            Assert.AreEqual(new RectangleF(20, 30, 20, 30), view1.Frame);
            Assert.AreEqual(new RectangleF(20, 60, 20, 20), view2.Frame);
        }
    }
}
