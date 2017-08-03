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
        RectangleF _layoutBounds;
        PointF _layoutOrigin;
        float _maxSize;
        Matrix2d _layoutToView, _viewToLayout;
        LayoutBuilder _layoutBuilder;

        public bool Flow { get; set; }

        public void StartLayout(LayoutBuilder layoutBuilder, LayoutDirection layoutDirection)
        {
            StartLayout(layoutBuilder, layoutDirection, DefaultFlowDirection(layoutDirection));
        }

        public void StartLayout(LayoutBuilder layoutBuilder, LayoutDirection layoutDirection, LayoutDirection flowDirection)
        {
            Assert.Argument(layoutBuilder, nameof(layoutBuilder)).NotNull();

            _layoutBuilder = layoutBuilder;
            _maxSize = 0;
            _layoutToView = LayoutTransform(_layoutBuilder.Bounds, layoutDirection, flowDirection);
            _viewToLayout = _layoutToView.Inverted();
            _layoutBounds = _viewToLayout.Transform(_layoutBuilder.Bounds);
            _layoutOrigin = _layoutBounds.Location;
        }

        public LinearLayoutBuilder Add(LayoutParams layoutParams)
        {
            Assert.State(_layoutBuilder).NotNull("Layout is not started. Did you call StartLayout?");

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
            var viewBox = _layoutBuilder.View(layoutParams.View, viewFreeSpace);
            layoutParams.Layout(viewBox);
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

        static void DefaultLayout(IViewLayoutBox obj)
        {
            obj.AutoSize().Left(0).Top(0);
        }
    }
}
