using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qoden.UI.Test
{
    [TestClass]
    public class LayoutBoxTest
    {
        [TestMethod]
        public void CenterVertically()
        {
            var lb = new LayoutBox(new RectangleF(0, 0, 375, 44));
            lb.Left(20).CenterVertically().Height(18).Right(0);
            Assert.AreEqual(new RectangleF(20, 13, 355, 18), lb.Frame());
        }

        [TestMethod]
        public void CenterHorizontaly()
        {
            var lb = new LayoutBox(new RectangleF(20, 20, 100, 100));
            lb.CenterHorizontally().Height(20).Width(20).Top(0);
            Assert.AreEqual(new RectangleF(60, 20, 20, 20), lb.Frame());
        }
    }
}
