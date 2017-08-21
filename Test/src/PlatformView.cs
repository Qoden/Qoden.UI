using System;
using System.Collections.Generic;
using System.Drawing;

namespace Qoden.UI
{
    public class PlatformView
    {
        public List<PlatformView> Subviews { get; private set; } = new List<PlatformView>();
        public RectangleF Frame { get; set; }
        public Type ViewType { get; set; }
        public EdgeInsets LayoutMargins { get; set; }
        public PlatformView Parent { get; set; }
        public Dictionary<string, string> Info { get; } = new Dictionary<string, string>();

        public PlatformView()
        {
        }

        public PlatformView(float l, float t, float w, float h) : this()
        {
            Frame = new RectangleF(l, t, w, h);
        }
    }
    public class PlatformViewGroup : PlatformView { }

    public class PlatformLabel : PlatformView {}

    public class PlatformTextField : PlatformView 
    {
        public string Hint;
    }

    public class PlatformImageView : PlatformView
    {
        public PlatformImage Image;
    }

    public class PlatformProgressBar : PlatformView 
    {
        float Progress;
    }

    public class PlatformTableViewContent {}

    public class PlatformTableView : PlatformView 
    {
        PlatformTableViewContent Content;
    }

    public class PlatformGroupTableView : PlatformTableView {}

    public class PlatformImage {}
}