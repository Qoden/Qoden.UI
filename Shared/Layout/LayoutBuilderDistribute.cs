using System.Drawing;
using System.Linq;

namespace Qoden.UI
{
#if __IOS__
    using PlatformView = UIKit.UIView;
#endif
#if __ANDROID__
    using PlatformView = Android.Views.View;
#endif

    public enum DistributionDirection
    {
        Horizontal,
        Vertical
    }

    public static class LayoutBuilderDistribute
    {
        public static void Distribute(this LayoutBuilder layout, RectangleF bounds, DistributionDirection direction,
            params PlatformView[] views)
        {
            if (direction == DistributionDirection.Horizontal)
                layout.DistributeHorizontally(bounds, views);
            else
                layout.DistributeVertically(bounds, views);
        }

        public static void DistributeHorizontally(this LayoutBuilder layout, params PlatformView[] views)
        {
            layout.DistributeHorizontally(layout.PaddedOuterBounds, views);
        }

        public static void DistributeHorizontally(this LayoutBuilder layout, RectangleF bounds,
            params PlatformView[] views)
        {
            var viewBoxes = views.Select(x => layout.View(x)).ToArray();
            var totalWidth = viewBoxes.Sum(x => x.Width);
            if (totalWidth > bounds.Width || views.Length < 2)
            {
                return;
            }
            var dx = (bounds.Width - totalWidth) / (views.Length - 1);
            var xCoord = 0f;
            foreach (var v in viewBoxes)
            {
                v.Left(xCoord).Width(v.Width);
                xCoord += (dx + v.Width);
            }
        }

        public static void DistributeVertically(this LayoutBuilder layout, params PlatformView[] views)
        {
            layout.DistributeVertically(layout.PaddedOuterBounds, views);
        }

        public static void DistributeVertically(this LayoutBuilder layout, RectangleF bounds,
            params PlatformView[] views)
        {
            var viewBoxes = views.Select(x => layout.View(x)).ToArray();
            var totalWidth = viewBoxes.Sum(x => x.Height);
            if (totalWidth > bounds.Height || views.Length < 2)
            {
                return;
            }
            var dx = (bounds.Height - totalWidth) / (views.Length - 1);
            var xCoord = 0f;
            foreach (var v in viewBoxes)
            {
                v.Top(xCoord).Height(v.Height);
                xCoord += (dx + v.Height);
            }
        }
    }
}