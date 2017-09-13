﻿using System;
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

    /// <summary>
    /// Root object to build set of view rectangles (layout).
    /// </summary>
    public class LayoutBuilder
    {
        float? _preferredHeight;
        float? _preferredWidth;
        private EdgeInsets _padding;
        readonly Dictionary<PlatformView, ViewLayoutBox> _boxes = new Dictionary<PlatformView, ViewLayoutBox>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Qoden.UI.LayoutBuilder"/> class.
        /// </summary>
        /// <param name="bounds">Layout outer bounds.</param>
        public LayoutBuilder(RectangleF bounds)
        {
            OuterBounds = bounds;
        }

        /// <summary>
        /// Default layout outer bounds.
        /// </summary>
        public RectangleF OuterBounds { get; private set; }

        public RectangleF PaddedOuterBounds =>
            new RectangleF(Math.Min(OuterBounds.Right, OuterBounds.Left + Padding.Left),
                Math.Min(OuterBounds.Bottom, OuterBounds.Top + Padding.Top),
                Math.Max(0, OuterBounds.Width - Padding.Left - Padding.Right),
                Math.Max(0, OuterBounds.Height - Padding.Top - Padding.Bottom));

        /// <summary>
        /// Gets the views.
        /// </summary>
        /// <value>The views.</value>
        public IEnumerable<IViewLayoutBox> Views => _boxes.Values;

        /// <summary>
        /// Build <see cref="IViewLayoutBox"/>. 
        /// </summary>
        /// <param name="v">View geometry (usually cross platform View wrapper)</param>
        /// <param name="outerBounds">Optional outer bounds to override this builder <see cref="OuterBounds"/>.</param>
        /// <param name="units">Optional units to override this builder <see cref="Units"/>.</param>
        public IViewLayoutBox View(PlatformView v, RectangleF? outerBounds = null, IUnit units = null)
        {
            var layoutBounds = outerBounds ?? PaddedOuterBounds;
            ViewLayoutBox box;
            if (_boxes.TryGetValue(v, out box))
            {
                if (box.OuterBounds != layoutBounds)
                {
                    _boxes.Remove(v);
                    box = null;
                }
            }
            if (box == null)
            {
                box = new ViewLayoutBox(v, layoutBounds);
            }
            _boxes[v] = box;
            return box;
        }

        /// <summary>
        /// Build <see cref="IViewLayoutBox"/>. 
        /// </summary>
        /// <param name="v">View geometry (usually cross platform View wrapper)</param>
        /// <param name="outerBounds">Optional outer bounds to override this builder <see cref="OuterBounds"/>.</param>
        public IViewLayoutBox View(PlatformView v, ILayoutBox outerBounds)
        {
            return View(v, outerBounds.Frame());
        }

        /// <summary>
        /// Builds box with specified outerBounds, padding and units.
        /// </summary>
        /// <returns>The box.</returns>
        /// <param name="outerBounds">Outer bounds.</param>
        public ILayoutBox Box(RectangleF? outerBounds = null)
        {
            var layoutBounds = outerBounds ?? PaddedOuterBounds;
            return new LayoutBox(layoutBounds);
        }

        /// <summary>
        /// Builds box with specified outerBounds, padding and units.
        /// </summary>
        /// <returns>The box.</returns>
        /// <param name="outerBounds">Outer bounds.</param>
        public ILayoutBox Box(ILayoutBox outerBounds)
        {
            return Box(outerBounds.Frame());
        }

        /// <summary>
        /// Calculate bounding frame in view coordinate system in pixels
        /// </summary>
        public RectangleF BoundingFrame(bool withPadding = true)
        {
            if (_boxes.Count == 0) return new RectangleF(OuterBounds.Location, SizeF.Empty	);
            
            var padding = withPadding ? Padding : EdgeInsets.Zero;
            var frames = _boxes.Select(x => x.Value.Frame());
            var combinedFrame = frames.Aggregate(RectangleF.Union);
            return new RectangleF(combinedFrame.Left - padding.Left,
                combinedFrame.Top - padding.Top,
                combinedFrame.Width + padding.Left + padding.Right,
                combinedFrame.Height + padding.Top + padding.Bottom);
        }

        /// <summary>
        /// Returns bounding frame for views. Frame coordinates is in view coordinates.
        /// </summary>
        public RectangleF BoundingFrame(params PlatformView[] views)
        {
            return BoundingFrame(views.AsEnumerable());
        }
        /// <summary>
        /// Returns bounding frame for views. Frame coordinates is in view coordinates.
        /// </summary>        
        public RectangleF BoundingFrame(IEnumerable<PlatformView> views)
        {
            var frames = _boxes.Where(x => views.Any(y => ReferenceEquals(y, x.Key))).Select(x => x.Value.Frame);
            return frames.Aggregate(RectangleF.Union);
        }
        
        /// <summary>
        /// Get or set Preferred layout width in pixels
        /// </summary>
        public float PreferredWidth
        {
            get
            {
                if (_preferredWidth.HasValue) return _preferredWidth.Value;
                return BoundingFrame().Right - OuterBounds.Left;
            }
            set => _preferredWidth = value;
        }

        public LayoutBuilder SetPreferredWidth(float width, bool addPadding = false)
        {
            PreferredWidth = width + (addPadding ? Padding.Left + Padding.Right : 0);
            return this;
        }
        
        public LayoutBuilder SetPreferredHeight(float height, bool addPadding = false)
        {
            PreferredHeight = height + (addPadding ? Padding.Top + Padding.Bottom: 0);
            return this;
        }

        /// <summary>
        /// Get or set Preferred layout height in pixels
        /// </summary>
        public float PreferredHeight
        {
            get
            {
                if (_preferredHeight.HasValue) return _preferredHeight.Value;
                return BoundingFrame().Bottom - OuterBounds.Top;
            }
            set => _preferredHeight = value;
        }

        /// <summary>
        /// Preferred layout size in pixels
        /// </summary>
        public SizeF PreferredSize => new SizeF(PreferredWidth, PreferredHeight);

        public EdgeInsets Padding
        {
            get => _padding;
            set
            {
                if (!value.Equals(_padding))
                {
                    _padding = value;
                    var minWidth = Math.Max(OuterBounds.Width, _padding.Left + _padding.Right);
                    var minHeight = Math.Max(OuterBounds.Height, _padding.Bottom + _padding.Top);
                    OuterBounds = new RectangleF(OuterBounds.Location, new SizeF(minWidth, minHeight));
                }
            }
        }

        public virtual void Layout()
        {
            foreach (var v in Views)
            {
                v.Layout();
            }
        }

        public override string ToString()
        {
            return $"{OuterBounds}, {PreferredSize}";
        }
    }
}