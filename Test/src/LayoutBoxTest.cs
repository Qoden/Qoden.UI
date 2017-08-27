using System;
using System.Drawing;
using Xunit;

namespace Qoden.UI.Test
{
    public class LayoutBoxTest
    {
        [Fact]
        public void CenterVertically()
        {
            var lb = new LayoutBox(new RectangleF(0, 0, 375, 44));
            lb.Left(20).CenterVertically().Height(18).Right(0);
            Assert.Equal(new RectangleF(20, 13, 355, 18), lb.Frame());
        }

        [Fact]
        public void CenterHorizontaly()
        {
            var lb = new LayoutBox(new RectangleF(20, 20, 100, 100));
            lb.CenterHorizontally().Height(20).Width(20).Top(0);
            Assert.Equal(new RectangleF(60, 20, 20, 20), lb.Frame());
        }
    }
}
