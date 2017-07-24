using System;
using System.Collections.Generic;
using System.Drawing;

namespace Qoden.UI
{
#if __IOS__
    using View = UIKit.UIView;
#endif
#if __ANDROID__
    using View = Android.Views.View;
#endif
    public class LayoutBuilder
    {
        private List<PlatformViewLayoutBox> _boxes = new List<PlatformViewLayoutBox>();

        public LayoutBuilder(RectangleF bounds)
        {
            Bounds = bounds;
        }

        public RectangleF Bounds { get; private set; }

        public PlatformViewLayoutBox View(IPlatformView<View> v, IUnit units = null)
        {
            var box = new PlatformViewLayoutBox(v, Bounds, units);
            _boxes.Add(box);
            return box;
        }

        public PlatformViewLayoutBox View(View v, IUnit units = null)
        {
            return View(new QView(v), units);
        }

        public IEnumerable<PlatformViewLayoutBox> Views => _boxes;
    }
}
