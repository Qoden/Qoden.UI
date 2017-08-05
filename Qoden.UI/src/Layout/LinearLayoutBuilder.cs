using System;
using System.Collections.Generic;
using System.Drawing;
using Qoden.Util;
using Qoden.Validation;

namespace Qoden.UI
{
    public enum LinearLayoutDirection
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

        float _maxSize;
        Matrix2d _layoutToView, _viewToLayout;
        LayoutBuilder _layoutBuilder;
        List<IViewLayoutBox> _views = new List<IViewLayoutBox>();

        public bool Flow { get; set; }

        public LinearLayoutBuilder(LayoutBuilder layoutBuilder, LinearLayoutDirection layoutDirection, LinearLayoutDirection? flowDirection = null, RectangleF? bounds = null)
        {
            Assert.Argument(layoutBuilder, nameof(layoutBuilder)).NotNull();
            flowDirection = flowDirection ?? DefaultFlowDirection(layoutDirection);
            _layoutBuilder = layoutBuilder;
            _maxSize = 0;
            _viewBounds = bounds ?? _layoutBuilder.PaddedOuterBounds;
            _layoutToView = LayoutTransform(_viewBounds, layoutDirection, flowDirection.Value);
            _viewToLayout = _layoutToView.Inverted();
            _layoutBounds = _viewToLayout.Transform(_viewBounds);
            _layoutOrigin = _layoutBounds.Location;
        }

        public IViewLayoutBox Add(LayoutParams layoutParams)
        {
            Assert.State(_layoutBuilder).NotNull("Layout is not started. Did you call StartLayout?");

            var layoutResult = LayoutView(_layoutOrigin, ref layoutParams);
            var newLayoutOrigin = layoutResult.NewLayoutOrigin;
            if (Flow && newLayoutOrigin.X - LayoutStep > _layoutBounds.Right)
            {
                var nextLine = new PointF(_layoutBounds.X, _layoutOrigin.Y + _maxSize + FlowStep);
                layoutResult = LayoutView(nextLine, ref layoutParams);
                newLayoutOrigin = layoutResult.NewLayoutOrigin;
                if (newLayoutOrigin.X > _layoutBounds.Right)
                {
                    newLayoutOrigin.X = _layoutBounds.X;
                    newLayoutOrigin.Y = _layoutOrigin.Y + layoutResult.LayoutViewFrame.Height + FlowStep;
                }

            }
            _maxSize = Math.Max(_maxSize, layoutResult.LayoutViewFrame.Height);
            _layoutOrigin = newLayoutOrigin;

            return layoutResult.ViewLayoutBox;
        }

        public void AddOverflow()
        {
            _layoutOrigin = new PointF(_layoutBounds.X, _layoutOrigin.Y + _maxSize + FlowStep);
            _maxSize = 0;
        }

        public float LayoutStep { get; set; } = 0;
        public float FlowStep { get; set; } = 0;
        public IEnumerable<IViewLayoutBox> Views => _views;

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
            //Area which view wants to occupy in view coordinates
            var viewBox = _layoutBuilder.View(layoutParams.View, viewFreeSpace, EdgeInsets.Zero);
            _views.Add(viewBox);
            layoutParams.Layout(viewBox);
            //Area which view wants to occupy in layout coordinates
            var layoutFrame = _viewToLayout.Transform(viewBox.LayoutBounds);
            //Space required for view starting from layout origin in layout coordinates
            var viewFrame = new RectangleF(layoutOrigin, new SizeF(layoutFrame.Right - layoutOrigin.X, layoutFrame.Bottom - layoutOrigin.Y));
            var newLayoutOrigin = new PointF(viewFrame.Right + LayoutStep, viewFrame.Top);

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
        private static Matrix2d LayoutTransform(RectangleF bounds, LinearLayoutDirection layoutDirection, LinearLayoutDirection flowDirection)
        {
            switch (layoutDirection)
            {
                case LinearLayoutDirection.LeftRight:
                    switch (flowDirection)
                    {
                        case LinearLayoutDirection.TopBottom:
                            return new Matrix2d();
                        case LinearLayoutDirection.BottomTop:
                            return Matrix2d.Translation(0, bounds.Height) * Matrix2d.Stretch(1, -1);
                    }
                    break;
                case LinearLayoutDirection.RightLeft:
                    switch (flowDirection)
                    {
                        case LinearLayoutDirection.TopBottom:
                            return Matrix2d.Translation(bounds.Width, 0) * Matrix2d.Stretch(-1, 1);
                        case LinearLayoutDirection.BottomTop:
                            return Matrix2d.Translation(bounds.Width, bounds.Height) * Matrix2d.Stretch(-1, -1);
                    }
                    break;
                case LinearLayoutDirection.TopBottom:
                    switch (flowDirection)
                    {
                        case LinearLayoutDirection.LeftRight:
                            return Matrix2d.Rotation(90) * Matrix2d.Stretch(1, -1);
                        case LinearLayoutDirection.RightLeft:
                            return Matrix2d.Translation(bounds.Width, 0) * Matrix2d.Rotation(90);
                    }
                    break;
                case LinearLayoutDirection.BottomTop:
                    switch (flowDirection)
                    {
                        case LinearLayoutDirection.LeftRight:
                            return Matrix2d.Translation(0, bounds.Height) * Matrix2d.Rotation(-90);
                        case LinearLayoutDirection.RightLeft:
                            return Matrix2d.Translation(bounds.Width, bounds.Height) * Matrix2d.Stretch(-1, 1) * Matrix2d.Rotation(-90);
                    }
                    break;
            }
            var message = $"Layout direction {layoutDirection} is not compatible with flow direction {flowDirection}";
            throw new ArgumentException(message);
        }

        private static LinearLayoutDirection DefaultFlowDirection(LinearLayoutDirection layoutDirection)
        {
            switch (layoutDirection)
            {
                case LinearLayoutDirection.LeftRight:
                    return LinearLayoutDirection.TopBottom;
                case LinearLayoutDirection.TopBottom:
                    return LinearLayoutDirection.LeftRight;
                case LinearLayoutDirection.RightLeft:
                    return LinearLayoutDirection.TopBottom;
                case LinearLayoutDirection.BottomTop:
                    return LinearLayoutDirection.LeftRight;
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

    public static class LinearLayoutBuilder_LayoutBuilder_Extensions
    {
        public static LinearLayoutBuilder LinearLayout(this LayoutBuilder layoutBuilder, LinearLayoutDirection layoutDirection, LinearLayoutDirection? flowDirection = null, RectangleF? bounds = null)
        {
            return new LinearLayoutBuilder(layoutBuilder, layoutDirection, flowDirection, bounds);
        }
    }
}
