using System;
using System.Collections.Generic;
using System.Drawing;
using Qoden.Util;
using Qoden.Validation;

namespace Qoden.UI
{
    public enum LayoutDirection
    {
        TopBottom, BottomTop, LeftRight, RightLeft
    }
    /**
     * Layout builder performs layout in a coordinate system where 
     * primary layout direction is positive X axis and overflow layout direction
     * is Y axis. Once done it performs reverse transform to get coordinates in 
     * coordinate system associated with view.
     * 
     **/
    /// <summary>
    /// Linear layout builder.
    /// </summary>
    public class LinearLayoutBuilder
    {
        RectangleF _layoutBounds, _viewBounds;
        PointF _layoutOrigin;
        List<IViewLayoutBox> _views;
        float _maxSize;
        Matrix2d _layoutToView, _viewToLayout;

        public bool Flow { get; set; }
        public IEnumerable<IViewLayoutBox> Views => _views;
        public RectangleF Bounds => _viewBounds;

        public void StartLayout(RectangleF bounds, LayoutDirection layoutDirection)
        {
            StartLayout(bounds, layoutDirection, DefaultFlowDirection(layoutDirection));
        }

        public void StartLayout(RectangleF bounds, LayoutDirection layoutDirection, LayoutDirection flowDirection)
        {
            _maxSize = 0;
            _views = new List<IViewLayoutBox>();
            _viewBounds = bounds;
            _layoutToView = LayoutTransform(_viewBounds, layoutDirection, flowDirection);
            _viewToLayout = _layoutToView.Inverted();
            _layoutBounds = _viewToLayout.Transform(bounds);
            _layoutOrigin = _layoutBounds.Location;
        }

        public void Layout()
        {
            foreach (var v in _views)
            {
                v.Layout();
            }
            _views = null;
        }

        public LinearLayoutBuilder Add(LayoutParams layoutParams)
        {
            Assert.State(_views).NotNull("Layout is not started. Did you call StartLayout?");

            var layoutResult = LayoutView(_layoutOrigin, ref layoutParams);
            var newLayoutOrigin = layoutResult.NewLayoutOrigin;
            if (Flow && newLayoutOrigin.X > _layoutBounds.Right)
            {
                var nextLine = new PointF(_layoutBounds.X, _layoutOrigin.Y + _maxSize);
                layoutResult = LayoutView(nextLine, ref layoutParams);
                newLayoutOrigin = layoutResult.NewLayoutOrigin;
                if (newLayoutOrigin.X > _layoutBounds.Right)
                {
                    newLayoutOrigin.X = _layoutBounds.X;
                    newLayoutOrigin.Y = _layoutOrigin.Y + layoutResult.LayoutViewFrame.Height;
                }
            }
            _maxSize = Math.Max(_maxSize, layoutResult.LayoutViewFrame.Height);
            _layoutOrigin = newLayoutOrigin;
            _views.Add(layoutResult.ViewLayoutBox);

            return this;
        }

        struct LayoutViewResult
        {
            public IViewLayoutBox ViewLayoutBox;
            public RectangleF LayoutViewFrame;
            public PointF NewLayoutOrigin;
        }

        private LayoutViewResult LayoutView(PointF layoutOrigin, ref LayoutParams layoutParams)
        {
            var freeSpaceSize = new SizeF(_layoutBounds.Right - layoutOrigin.X, _layoutBounds.Bottom - layoutOrigin.Y);
            //Free space available a view in layout coordinates 
            var freeSpace = new RectangleF(layoutOrigin, freeSpaceSize);
            //Free space available for a view in view coordinates
            var viewFreeSpace = _layoutToView.Transform(freeSpace);
            //Postion which view wants to occupy in view coordinates
            var viewBox = layoutParams.LayoutView(viewFreeSpace);
            //Postion which view wants to occupy in layout coordinates
            var layoutFrame = _viewToLayout.Transform(viewBox.LayoutBounds);
            //Space required for view starting from layout origin in layout coordinates
            var viewFrame = new RectangleF(layoutOrigin, new SizeF(layoutFrame.Right - layoutOrigin.X, layoutFrame.Bottom - layoutOrigin.Y));
            var newLayoutOrigin = new PointF(viewFrame.Right, viewFrame.Top);

            return new LayoutViewResult()
            {
                ViewLayoutBox = viewBox,
                NewLayoutOrigin = newLayoutOrigin,
                LayoutViewFrame = viewFrame
            };
        }

        public SizeF GetPreferredSize()
        {
            var max = new PointF(float.MinValue, float.MinValue);
            var min = new PointF(float.MaxValue, float.MaxValue);
            foreach (var v in _views)
            {
                max.X = Math.Max(max.X, v.LayoutRight);
                max.Y = Math.Max(max.Y, v.LayoutBottom);

                min.X = Math.Max(min.X, v.LayoutLeft);
                min.Y = Math.Max(min.Y, v.LayoutTop);
            }
            return new SizeF(Math.Abs(max.X - min.X), Math.Abs(max.Y - min.Y));
        }


        /*
         * Calculates transform matrix from layout coordinate system to view coordinate system.
         */
        private static Matrix2d LayoutTransform(RectangleF bounds, LayoutDirection layoutDirection, LayoutDirection flowDirection)
        {
            switch (layoutDirection)
            {
                case LayoutDirection.LeftRight:
                    switch (flowDirection)
                    {
                        case LayoutDirection.TopBottom:
                            return new Matrix2d();
                        case LayoutDirection.BottomTop:
                            return Matrix2d.Translation(0, bounds.Height) * Matrix2d.Stretch(1, -1);
                    }
                    break;
                case LayoutDirection.RightLeft:
                    switch (flowDirection)
                    {
                        case LayoutDirection.TopBottom:
                            return Matrix2d.Translation(bounds.Width, 0) * Matrix2d.Stretch(-1, 1);
                        case LayoutDirection.BottomTop:
                            return Matrix2d.Translation(bounds.Width, bounds.Height) * Matrix2d.Stretch(-1, -1);
                    }
                    break;
                case LayoutDirection.TopBottom:
                    switch (flowDirection)
                    {
                        case LayoutDirection.LeftRight:
                            return Matrix2d.Rotation(90) * Matrix2d.Stretch(1, -1);
                        case LayoutDirection.RightLeft:
                            return Matrix2d.Translation(bounds.Width, 0) * Matrix2d.Rotation(90);
                    }
                    break;
                case LayoutDirection.BottomTop:
                    switch (flowDirection)
                    {
                        case LayoutDirection.LeftRight:
                            return Matrix2d.Translation(0, bounds.Height) * Matrix2d.Rotation(-90);
                        case LayoutDirection.RightLeft:
                            return Matrix2d.Translation(bounds.Width, bounds.Height) * Matrix2d.Stretch(-1, 1) * Matrix2d.Rotation(-90);
                    }
                    break;
            }
            var message = $"Layout direction {layoutDirection} is not compatible with flow direction {flowDirection}";
            throw new ArgumentException(message);
        }

        private static LayoutDirection DefaultFlowDirection(LayoutDirection layoutDirection)
        {
            switch (layoutDirection)
            {
                case LayoutDirection.LeftRight:
                    return LayoutDirection.TopBottom;
                case LayoutDirection.TopBottom:
                    return LayoutDirection.LeftRight;
                case LayoutDirection.RightLeft:
                    return LayoutDirection.TopBottom;
                case LayoutDirection.BottomTop:
                    return LayoutDirection.LeftRight;
                default:
                    throw new ArgumentException("Unknown layout direction");
            }
        }
    }

    public struct LayoutParams
    {
        IViewGeometry _view;
        Action<IViewLayoutBox> _layout;

        public LayoutParams(IViewGeometry view) : this(view, DefaultLayout)
        {
        }

        public LayoutParams(IViewGeometry view, Action<IViewLayoutBox> layoutAction)
        {
            Assert.Argument(view, nameof(view)).NotNull();
            Assert.Argument(layoutAction, nameof(layoutAction)).NotNull();
            _view = view;
            _layout = layoutAction ?? DefaultLayout;
        }

        public IViewGeometry View
        {
            get => _view;
            set
            {
                _view = Assert.Property(value).NotNull().Value;
            }
        }

        public Action<IViewLayoutBox> Layout
        {
            get => _layout;
            set
            {
                Assert.Property(value).NotNull();
                _layout = value;
            }
        }

        public IViewLayoutBox LayoutView(RectangleF bounds)
        {
            var box = View.LayoutInBounds(bounds);
            Layout(box);
            return box;
        }

        static void DefaultLayout(IViewLayoutBox obj)
        {
            obj.AutoSize().Left(0).Top(0);
        }
    }
}
