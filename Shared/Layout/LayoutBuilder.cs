using System;
using System.Collections.Generic;
using System.Drawing;

namespace Qoden.UI
{
#if __IOS__
    using PlatformView = UIKit.UIView;
#endif
#if __ANDROID__
    using PlatformView = Android.Views.View;
#endif

    /// <summary>
    /// Root object to build set of view rectangles (layout).
    /// </summary>
    public class LayoutBuilder
    {
        List<IViewLayoutBox> _boxes = new List<IViewLayoutBox>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Qoden.UI.LayoutBuilder"/> class.
        /// </summary>
        /// <param name="bounds">Layout outer bounds.</param>
        /// <param name="units">Layout measurement units.</param>
        public LayoutBuilder(RectangleF bounds, IUnit units = null)
        {
            OuterBounds = bounds;
            Units = units;
        }

        /// <summary>
        /// Default layout outer bounds.
        /// </summary>
        public RectangleF OuterBounds { get; private set; }

        /// <summary>
        /// Default layout measurement units.
        /// </summary>
        /// <value>The units.</value>
        public IUnit Units { get; private set; }

        /// <summary>
        /// Gets the views.
        /// </summary>
        /// <value>The views.</value>
        public IEnumerable<IViewLayoutBox> Views => _boxes;

        /// <summary>
        /// Build <see cref="IViewLayoutBox"/>. 
        /// </summary>
        /// <param name="v">View geometry (usually cross platform View wrapper)</param>
        /// <param name="outerBounds">Optional outer bounds to override this builder <see cref="OuterBounds"/>.</param>
        /// <param name="units">Optional units to override this builder <see cref="Units"/>.</param>
        public IViewLayoutBox View(PlatformView v, RectangleF? outerBounds = null, IUnit units = null)
        {
            var layoutBounds = outerBounds ?? OuterBounds;
            layoutBounds = new RectangleF(Math.Min(layoutBounds.Right, layoutBounds.Left + Padding.Left),
                                          Math.Min(layoutBounds.Bottom, layoutBounds.Top + Padding.Top),
                                          Math.Max(0, layoutBounds.Width - Padding.Left - Padding.Right),
                                          Math.Max(0, layoutBounds.Height - Padding.Top - Padding.Bottom));
            var box = new ViewLayoutBox(v, layoutBounds, units ?? Units);
            _boxes.Add(box);
            return box;
        }

        /// <summary>
        /// Builds box with specified outerBounds, padding and units.
        /// </summary>
        /// <returns>The box.</returns>
        /// <param name="outerBounds">Outer bounds.</param>
        /// <param name="units">Units.</param>
        public ILayoutBox Box(RectangleF? outerBounds = null, IUnit units = null)
        {
            var layoutBounds = outerBounds ?? OuterBounds;
            return new LayoutBox(layoutBounds, units ?? Units);
        }

        /// <summary>
        /// Calculate bounding frame in view coordinate system in pixels
        /// </summary>
        public RectangleF BoundingFrame()
        {
            RectangleF combinedFrame = new RectangleF(Padding.Left, Padding.Top, 0, 0);
            foreach (var v in Views)
            {
                var frame = v.Frame();
                combinedFrame = RectangleF.Union(combinedFrame, frame);
            }
            return new RectangleF(combinedFrame.Left - Padding.Left,
                                  combinedFrame.Top - Padding.Top,
                                  combinedFrame.Width + Padding.Left + Padding.Right,
                                  combinedFrame.Height + Padding.Top + Padding.Bottom);
        }

        float? _preferredWidth;
        /// <summary>
        /// Get or set Preferred layout width in pixels
        /// </summary>
        public float PreferredWidth
        {
            get
            {
                if (_preferredWidth.HasValue) return _preferredWidth.Value;
                return BoundingFrame().Size.Width;
            }
            set { _preferredWidth = value; }
        }

        float? _preferredHeight;
        /// <summary>
        /// Get or set Preferred layout height in pixels
        /// </summary>
        public float PreferredHeight
        {
            get
            {
                if (_preferredHeight.HasValue) return _preferredHeight.Value;
                return BoundingFrame().Size.Height;
            }
            set { _preferredHeight = value; }
        }

        /// <summary>
        /// Preferred layout size in pixels
        /// </summary>
        public SizeF PreferredSize => new SizeF(PreferredWidth, PreferredHeight);

        public EdgeInsets Padding { get; set; }

        public virtual void Layout()
        {
            foreach (var v in Views)
            {
                v.Layout();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", OuterBounds, PreferredSize);
        }
    }
}
