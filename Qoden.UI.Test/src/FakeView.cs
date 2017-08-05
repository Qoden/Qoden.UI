using System;
using System.Collections.Generic;
using System.Drawing;

namespace Qoden.UI.Test
{
    public class FakeView
    {
        public List<FakeView> Subviews { get; private set; } = new List<FakeView>();
        public RectangleF Frame { get; set; }
        public Type ViewType { get; set; }
        public EdgeInsets LayoutMargins { get; set; }
        public FakeView Parent { get; set; }
        public Dictionary<string, string> Info { get; } = new Dictionary<string, string>();

        public FakeView() { }

        public FakeView(float l, float t, float w, float h)
        {
            Frame = new RectangleF(l, t, w, h);
        }
    }
}
