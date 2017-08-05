using System;
using System.Collections.Generic;
using System.Drawing;

namespace Qoden.UI
{
    /// <summary>
    /// Root object to build set of view rectangles (layout).
    /// </summary>
    public class LayoutBuilder
    {
        private List<IViewLayoutBox> _boxes = new List<IViewLayoutBox>();

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
        /// Default layout padding.
        /// </summary>
        public EdgeInsets Padding { get; set; }

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
        /// <param name="padding">Optional padding to override this builder <see cref="Padding"/>.</param>
        /// <param name="units">Optional units to override this builder <see cref="Units"/>.</param>
        public IViewLayoutBox View(IViewGeometry v, RectangleF? outerBounds = null, EdgeInsets? padding = null, IUnit units = null)
        {
            var layoutBounds = MakeLayoutBounds(outerBounds, padding);
            var box = v.MakeViewLayoutBox(layoutBounds, units ?? Units);
            _boxes.Add(box);
            return box;
        }

        /// <summary>
        /// Builds box with specified outerBounds, padding and units.
        /// </summary>
        /// <returns>The box.</returns>
        /// <param name="outerBounds">Outer bounds.</param>
        /// <param name="padding">Padding.</param>
        /// <param name="units">Units.</param>
        public ILayoutBox Box(RectangleF? outerBounds = null, EdgeInsets? padding = null, IUnit units = null)
        {
            var layoutBounds = MakeLayoutBounds(outerBounds, padding);
            return new LayoutBox(layoutBounds, units ?? Units);
        }

        private RectangleF MakeLayoutBounds(RectangleF? outerBounds, EdgeInsets? padding)
        {
            var b = outerBounds ?? OuterBounds;
            var p = padding ?? Padding;
            var layoutBounds = new RectangleF(b.X + p.Left,
                                              b.Y + p.Top,
                                              Math.Max(b.Width - p.Left - p.Right, 0),
                                              Math.Max(b.Height - p.Top - p.Bottom, 0));
            return layoutBounds;
        }

        public RectangleF PaddedOuterBounds
        {
            get 
            {
                return MakeLayoutBounds(OuterBounds, Padding);
            }
        }

        public virtual void Layout()
        {
            foreach (var v in Views)
            {
                v.Layout();
            }
        }
    }
}
