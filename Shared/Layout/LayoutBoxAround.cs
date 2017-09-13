using System.Collections.Generic;
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

    public static class LayoutBoxBoxArdound
    {
        /// <summary>
        /// Wrap <see cref="IViewLayoutBox"/> around <see cref="bounds"/> with provided <see cref="padding"/>.
        /// </summary>
        /// <param name="box">layout box</param>
        /// <param name="bounds">bounds to wrap around</param>
        /// <param name="padding">padding to add to bounds</param>
        public static T Around<T>(this T box, RectangleF bounds, EdgeInsets? padding = null) where T : ILayoutBox
        {
            var p = padding ?? EdgeInsets.Zero;
            var l = bounds.Left - p.Left;
            var r = bounds.Right + p.Right;
            var t = bounds.Top - p.Top;
            var b = bounds.Bottom + p.Bottom;
            var outer = box.OuterBounds;

            return box.Left(l - outer.Left)
                .Top(t - outer.Top)
                .Right(outer.Right - r)
                .Bottom(outer.Bottom - b);
        }

        /// <summary>
        /// Wrap <see cref="IViewLayoutBox"/> around <see cref="boxes"/> with provided <see cref="padding"/>.
        /// </summary>
        /// <param name="box">layout box</param>
        /// <param name="boxes">boxes to wrap around</param>
        /// <param name="padding">padding to add to bounds</param>
        public static T Around<T>(this T box, EdgeInsets padding, params ILayoutBox[] boxes) where T : ILayoutBox
        {
            return box.Around(boxes.AsEnumerable(), padding);
        }

        /// <summary>
        /// Wrap <see cref="IViewLayoutBox"/> around <see cref="boxes"/>.
        /// </summary>
        /// <param name="box">layout box</param>
        /// <param name="boxes">boxes to wrap around</param>
        public static T Around<T>(this T box, params ILayoutBox[] boxes) where T : ILayoutBox
        {
            return box.Around(boxes.AsEnumerable());
        }

        /// <summary>
        /// Wrap <see cref="IViewLayoutBox"/> around <see cref="boxes"/> with provided <see cref="padding"/>.
        /// </summary>
        /// <param name="box">layout box</param>
        /// <param name="boxes">boxes to wrap around</param>
        /// <param name="padding">padding to add to bounds</param>
        public static T Around<T>(this T box, IEnumerable<ILayoutBox> boxes, EdgeInsets? padding = null)
            where T : ILayoutBox
        {
            return box.Around(boxes.Select(x => x.Frame).Aggregate(RectangleF.Union), padding);
        }

        
        public static T Around<T>(this T box, IEnumerable<RectangleF> boxes, EdgeInsets? padding = null)
            where T : ILayoutBox
        {
            return box.Around(boxes.Aggregate(RectangleF.Union), padding);
        }
    }
}