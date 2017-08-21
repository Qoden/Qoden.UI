using System;
using System.Collections.Generic;
using System.Drawing;
using Qoden.Binding;

namespace Qoden.UI
{
#if __IOS__
    using UIKit;
    using PlatformViewGroup = UIKit.UIView;
#endif
#if __ANDROID__
    using Android.OS;
    using Android.Util;
    using PlatformViewGroup = Android.Views.ViewGroup;
#endif

    /// <summary>
    /// Base view using above infrastructure.
    /// </summary>
    public class QodenView : PlatformViewGroup, ILayoutable
    {
        public ViewBuilder Builder { get; private set; }
        public BindingList Bindings => _bindings.Value;
        BindingListHolder _bindings;

#if __IOS__
        public QodenView(System.IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public QodenView()
        {
            Initialize();
        }

        public QodenView(Foundation.NSCoder coder) : base(coder)
        {
            Initialize();
        }

        public QodenView(Foundation.NSObjectFlag t) : base(t)
        {
            Initialize();
        }

        public QodenView(CoreGraphics.CGRect frame) : base(frame)
        {
            Initialize();
        }

        void Initialize()
        {
            LayoutMargins = new UIEdgeInsets(0, 0, 0, 0);
            Builder = new ViewBuilder(this);
        }

        public sealed override void LayoutSubviews()
        {
            var layoutBuilder = new LayoutBuilder((RectangleF)Bounds);
            layoutBuilder.Padding = new EdgeInsets(Padding.Left,
                                                   Padding.Top,
                                                   Padding.Right,
                                                   Padding.Bottom);
            OnLayout(layoutBuilder);
            foreach (var box in layoutBuilder.Views)
            {
                box.Layout();
            }
        }

        EdgeInsets _padding;
        public EdgeInsets Padding 
        {
            get => _padding;
            set 
            {
                _padding = value;
                SetNeedsLayout();
            }
        }

        void ILayoutable.OnLayout(LayoutBuilder layout)
        {
            OnLayout(layout);
        }

        protected virtual void OnLayout(LayoutBuilder layout)
        {
        }

        public sealed override CoreGraphics.CGSize SizeThatFits(CoreGraphics.CGSize size)
        {
            return PreferredSize((SizeF)size);
        }

        public virtual SizeF PreferredSize(SizeF bounds)
        {
            return ViewLayoutUtil.PreferredSize(this, bounds);
        }

        IEventSource _clickTarget;

        public IEventSource ClickTarget()
        {
            if (_clickTarget == null)
            {
                _clickTarget = new UIViewClickEventSource(this);
            }
            return _clickTarget;
        }

#endif
#if __ANDROID__
        public QodenView(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
        }

        public QodenView(Android.Content.Context context) :
            base(context)
        {
            Initialize();
        }

        public QodenView(Android.Content.Context context, Android.Util.IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public QodenView(Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            Builder = new ViewBuilder(this, Context);
        }

        protected sealed override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            //var rect = new RectangleF(PaddingLeft, PaddingTop, r - l - PaddingRight, b - t - PaddingBottom);
            var rect = new RectangleF(0, 0, r - l, b - t);
            var layoutBuilder = new LayoutBuilder(rect);
            OnLayout(layoutBuilder);
            foreach (var box in layoutBuilder.Views)
            {
                box.Layout();
            }
        }

        protected virtual void OnLayout(LayoutBuilder layout)
        {
        }

        void ILayoutable.OnLayout(LayoutBuilder layout)
        {
            OnLayout(layout);
        }

        protected sealed override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var width = MeasureSpec.GetSize(widthMeasureSpec);
            var height = MeasureSpec.GetSize(heightMeasureSpec);
            var widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            var heightMode = MeasureSpec.GetMode(heightMeasureSpec);

            var size = PreferredSize(new System.Drawing.SizeF(width, height));
            width = (int)Math.Round(size.Width);
            height = (int)Math.Round(size.Height);

            width = ResolveSizeAndState(width, widthMeasureSpec, 0);
            height = ResolveSizeAndState(height, heightMeasureSpec, 0);
            SetMeasuredDimension(width, height);
        }

        public virtual System.Drawing.SizeF PreferredSize(System.Drawing.SizeF bounds)
        {
            return ViewLayoutUtil.PreferredSize(this, bounds);
        }

        protected override void DispatchSaveInstanceState(Android.Util.SparseArray container)
        {
            if (Id > 0)
            {
                container.Append(Id, (Java.Lang.Object)OnSaveInstanceState());
            }
        }

        protected override void DispatchRestoreInstanceState(SparseArray container)
        {
            if (Id > 0)
            {
                var bundle = container.Get(Id) as IParcelable;
                if (bundle != null)
                {
                    OnRestoreInstanceState(bundle);
                }
            }
        }

        protected override IParcelable OnSaveInstanceState()
        {
            var bundle = new Bundle();
            var childrenContainer = new SparseArray();
            var count = ChildCount;
            for (int i = 0; i < ChildCount; i++)
            {
                var c = GetChildAt(i);
                if (c.SaveEnabled)
                {
                    c.SaveHierarchyState(childrenContainer);
                    if (c.IsFocused)
                    {
                        bundle.PutInt("qoden:focused_view", c.Id);
                    }
                }
            }
            bundle.PutSparseParcelableArray("qoden:views", childrenContainer);
            return bundle;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            var bundle = state as Bundle;
            if (bundle != null)
            {
                var childrenContainer = bundle.GetSparseParcelableArray("qoden:views");
                var count = ChildCount;
                for (int i = 0; i < ChildCount; i++)
                {
                    var c = GetChildAt(i);
                    if (c.SaveEnabled)
                    {
                        c.RestoreHierarchyState(childrenContainer);
                    }
                }

                var focusedChildId = bundle.GetInt("qoden:focused_view");
                if (focusedChildId > 0)
                {
                    var focusedChild = FindViewById(focusedChildId);
                    if (focusedChild != null)
                    {
                        focusedChild.RequestFocus();
                    }
                }
            }
            else
            {
                base.OnRestoreInstanceState(state);
            }
        }
#endif

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _bindings.Unbind();
                Builder.Dispose();
            }
        }
    }
}

