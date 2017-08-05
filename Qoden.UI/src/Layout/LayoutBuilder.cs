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
        public IViewLayoutBox View(IViewGeometry v, RectangleF? outerBounds = null, IUnit units = null)
        {
            var layoutBounds = outerBounds ?? OuterBounds;
            var box = v.MakeViewLayoutBox(layoutBounds, units ?? Units);
            _boxes.Add(box);
            return box;
        }

        /// <summary>
        /// Build <see cref="IViewLayoutBox"/>. 
        /// </summary>
        /// <param name="outerBounds">Optional outer bounds to override this builder <see cref="OuterBounds"/>.</param>
        /// <param name="units">Optional units to override this builder <see cref="Units"/>.</param>
        public IViewLayoutBox Spacer(RectangleF? outerBounds = null, IUnit units = null)
        {
            var layoutBounds = outerBounds ?? OuterBounds;
            var box = new SpacerLayoutBox(layoutBounds, units);
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

        public SizeF PreferredSize
        {
            get
            {
                int r = int.MinValue, b = int.MinValue;
                foreach (var v in Views)
                {
                    var size = v.PreferredBoundingSize;
                    r = (int)Math.Round(Math.Max(r, v.OuterBounds.Left + size.Width));
                    b = (int)Math.Round(Math.Max(b, v.OuterBounds.Top + size.Height));
                }
                return new SizeF(r, b);
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
