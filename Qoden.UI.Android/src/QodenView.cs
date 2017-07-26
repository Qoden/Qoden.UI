using System;
using System.Drawing;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Qoden.UI
{
    public class QodenView : ViewGroup
    {
        private ViewHierarchy hierarchy;

        public ViewHierarchyBuilder Builder { get; private set; }

        public QodenView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
        }

        public QodenView(Context context) :
            base(context)
        {
            Initialize();
        }

        public QodenView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public QodenView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            CreateView();
        }

        protected virtual void CreateView()
        {
            Builder = new ViewHierarchyBuilder(Context);
            hierarchy = new ViewHierarchy(this, Builder);
            LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
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

        protected sealed override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var width = MeasureSpec.GetSize(widthMeasureSpec);
            var height = MeasureSpec.GetSize(heightMeasureSpec);
            var widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            var heightMode = MeasureSpec.GetMode(heightMeasureSpec);

            var size = SizeThatFits(new System.Drawing.SizeF(width, height));
            width = (int)Math.Round(size.Width);
            height = (int)Math.Round(size.Height);

            width = ResolveSizeAndState(width, widthMeasureSpec, 0);
            height = ResolveSizeAndState(height, heightMeasureSpec, 0);
            SetMeasuredDimension(width, height);
        }

        public virtual System.Drawing.SizeF SizeThatFits(System.Drawing.SizeF bounds)
        {
            var layoutBuilder = new LayoutBuilder(new RectangleF(PointF.Empty, bounds));
            OnLayout(layoutBuilder);
            int l = int.MaxValue, t = int.MaxValue, r = int.MinValue, b = int.MinValue;
            foreach (var v in layoutBuilder.Views)
            {
                l = (int)Math.Round(Math.Min(v.LayoutLeft, l));
                r = (int)Math.Round(Math.Max(v.LayoutRight, r));
                t = (int)Math.Round(Math.Min(v.LayoutTop, t));
                b = (int)Math.Round(Math.Max(v.LayoutBottom, b));
            }
            return new System.Drawing.SizeF(Math.Abs(r - l), Math.Abs(b - t));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                hierarchy?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void DispatchSaveInstanceState(SparseArray container)
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

        //public View PlatformView => this;

        //RectangleF IViewGeometry.Frame { get => this.Frame(); set => this.SetFrame(value); }

        //public IViewLayoutBox LayoutInBounds(RectangleF bounds, IUnit unit = null)
        //{
        //    return new PlatformViewLayoutBox(this, bounds, unit);
        //}
    }
}
