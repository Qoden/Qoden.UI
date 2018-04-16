using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Qoden.Binding;
#if __IOS__
using UIKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using PlatformView = UIKit.UIView;
using PlatformViewGroup = UIKit.UIView;
#endif
#if __ANDROID__
using Android.Views;
using PlatformView = Android.Views.View;
using PlatformViewGroup = Android.Views.ViewGroup;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Support.V4.Graphics.Drawable;
using Android.Content;
#endif

namespace Qoden.UI.Wrappers
{
    public struct View
    {
        public static implicit operator PlatformView(View area) { return area.PlatformView; }
        public PlatformView PlatformView { get; set; }

        public View(object view)
        {
            PlatformView = (PlatformView)view;
        }

        //You can also use View and other wrapper structs here since all of them 
        //implicitly cnverted to PlatformView subclasses
        public View AddSubview(PlatformView child)
        {
#if __IOS__
            PlatformView.AddSubview(child);
#elif __ANDROID__
            var view = PlatformView as PlatformViewGroup;
            if (view == null) throw new InvalidOperationException("PlatformView is not ViewGroup");
            if (child.Parent != null) child.AsView().RemoveFromSuperview();
            view.AddView(child);
#endif
            return this;
        }

        public View RemoveFromSuperview()
        {
#if __IOS__
            PlatformView.RemoveFromSuperview();
#elif __ANDROID__
            var view = PlatformView.Parent as PlatformViewGroup;
            if (view == null) throw new InvalidOperationException("PlatformView.Parent is not ViewGroup");
            view.RemoveView(PlatformView);
#endif
            return this;
        }

        public IEnumerable<View> Subviews()
        {
#if __IOS__
            foreach (var v in PlatformView.Subviews)
            {
                yield return new View() { PlatformView = v };
            }
#elif __ANDROID__
            var view = PlatformView as PlatformViewGroup;
            if (view != null)
            {
                var count = view.ChildCount;
                for (int i = 0; i < count; ++i)
                {
                    yield return new View() { PlatformView = view.GetChildAt(i) };
                }
            }
#endif
        }


        public RectangleF Frame
        {
#if __IOS__
            get => (RectangleF)PlatformView.Frame;
#elif __ANDROID__
            get => new RectangleF(PlatformView.Left, PlatformView.Top, PlatformView.Width, PlatformView.Height);
#endif
        }

        public void SetFrame(RectangleF value)
        {
#if __IOS__
            PlatformView.Frame = value;
#elif __ANDROID__
            PlatformView.Layout((int)Math.Round(value.Left),
                    (int)Math.Round(value.Top),
                    (int)Math.Round(value.Right),
                    (int)Math.Round(value.Bottom));
#endif
        }


        public EdgeInsets Padding
        {
#if __IOS__
            get
            {
                if (PlatformView is QodenView) return ((QodenView)PlatformView).Padding;
                return new EdgeInsets((float)PlatformView.LayoutMargins.Left,
                                  (float)PlatformView.LayoutMargins.Top,
                                  (float)PlatformView.LayoutMargins.Right,
                                  (float)PlatformView.LayoutMargins.Bottom);
            }
            set
            {
                if (PlatformView is QodenView)
                {
                    ((QodenView)PlatformView).Padding = value;
                }
                else
                {
                    PlatformView.LayoutMargins = new UIKit.UIEdgeInsets(value.Left,
                                                                        value.Top,
                                                                        value.Right,
                                                                        value.Bottom);
                }
            }
#elif __ANDROID__
            get => new EdgeInsets(PlatformView.PaddingLeft,
                                  PlatformView.PaddingTop,
                                  PlatformView.PaddingRight,
                                  PlatformView.PaddingBottom);
            set
            {
                PlatformView.SetPadding((int)Math.Round(value.Left),
                                        (int)Math.Round(value.Top),
                                        (int)Math.Round(value.Right),
                                        (int)Math.Round(value.Bottom));
            }
#endif
        }


        public bool Visible
        {
#if __IOS__
            get => !PlatformView.Hidden;
#elif __ANDROID__
            get => PlatformView.Visibility == ViewStates.Visible;
#endif
        }

        public void SetVisible(bool value)
        {
#if __IOS__
            PlatformView.Hidden = !value;
#elif __ANDROID__
            PlatformView.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
#endif
        }

        public View SetBackgroundColor(RGB color)
        {
#if __IOS__
            PlatformView.BackgroundColor = color.ToColor();
#elif __ANDROID__
            PlatformView.Background = GetColoredDrawable(PlatformView.Background, color.ToColor());
            PlatformView.Invalidate();
#endif
            return this;
        }

        public View SetCornerRadius(float radius)
        {
#if __IOS__
            PlatformView.Layer.CornerRadius = radius;
#elif __ANDROID__
            PlatformView.Background = GetRoundedDrawable(PlatformView.Context, PlatformView.Background, radius);
            PlatformView.Invalidate();
#endif
            return this;
        }

#if __ANDROID__
        Drawable GetRoundedDrawable(Context context, Drawable drawable, float radius)
        {
            // ShapeDrawable crashes when Mutate() is executed if there is no Shape set (tries to clone() null)
            if (!(drawable is ShapeDrawable shapeDrawable) || shapeDrawable.Shape != null)
                drawable = drawable?.Mutate();

            switch (drawable)
            {
                case PaintDrawable paintDrawable:
                    paintDrawable.SetCornerRadius(radius);
                    break;
                case GradientDrawable gradientDrawable:
                    gradientDrawable.SetCornerRadius(radius);
                    break;
                case RippleDrawable rippleDrawable:
                    {
                        var contentDrawable = rippleDrawable.GetDrawable(0);
                        contentDrawable = GetRoundedDrawable(context, contentDrawable, radius);

                        rippleDrawable.SetDrawable(0, contentDrawable);

                        var radii = new float[8];
                        Array.Fill(radii, radius);
                        var maskShape = new RoundRectShape(radii, null, null);
                        var maskDrawable = new ShapeDrawable(maskShape);
                        rippleDrawable.SetDrawableByLayerId(Android.Resource.Id.Mask, maskDrawable);
                    }
                    break;
                case DrawableWrapper drawableWrapper:
                    drawableWrapper.Drawable = GetRoundedDrawable(context, drawableWrapper.Drawable, radius);
                    break;
                case ColorDrawable colorDrawable:
                    {
                        var contentDrawable = new PaintDrawable(colorDrawable.Color);
                        contentDrawable.SetCornerRadius(radius);
                        drawable = contentDrawable;
                    }
                    break;
                case BitmapDrawable bitmapDrawable:
                    var roundedBitmap = RoundedBitmapDrawableFactory.Create(context.Resources, bitmapDrawable.Bitmap);
                    roundedBitmap.CornerRadius = radius;
                    drawable = roundedBitmap;
                    break;
                case DrawableContainer drawableContainer:
                    var drawables = (drawableContainer.GetConstantState() as DrawableContainer.DrawableContainerState).GetChildren();
                    for (var i = 0; i < drawables.Length; i++)
                        drawables[i] = GetRoundedDrawable(context, drawables[i], radius);
                    break;
                default:
                    {
                        var contentDrawable = new PaintDrawable();
                        contentDrawable.SetCornerRadius(radius);
                        drawable = contentDrawable;
                    }
                    break;
            }
            return drawable;
        }

        Drawable GetColoredDrawable(Drawable drawable, Android.Graphics.Color color)
        {
            if (drawable == null)
                return new ColorDrawable(color);

            drawable = drawable?.Mutate();

            switch (drawable)
            {
                case PaintDrawable paintDrawable:
                    paintDrawable.Paint.Color = color;
                    break;
                case GradientDrawable gradientDrawable:
                    gradientDrawable.SetColor(color);
                    break;
                case ShapeDrawable shapeDrawable:
                    var shape = shapeDrawable.Shape;
                    var newPaintDrawable = new PaintDrawable(color) { Shape = shape };
                    PlatformView.Background = newPaintDrawable;
                    break;
                case RippleDrawable rippleDrawable:
                    rippleDrawable.SetTint(color.ToArgb());
                    break;
                case DrawableWrapper drawableWrapper:
                    drawableWrapper.Drawable = GetColoredDrawable(drawableWrapper.Drawable, color);
                    break;
                case ColorDrawable colorDrawable:
                    colorDrawable.Color = color;
                    break;
                default:
                    var compatDrawable = DrawableCompat.Wrap(drawable);
                    DrawableCompat.SetTint(compatDrawable, color.ToArgb());
                    break;
            }
            return drawable;
        }
#endif

        public void SetupGradient(
            RGB[] gradientColors,
            PointF startPoint,
            PointF endPoint,
            float[] locations)
        {
#if __IOS__
            var gradientLayer = new CAGradientLayer();

            gradientLayer.Frame = PlatformView.Layer.Frame;

            gradientLayer.Colors = gradientColors.Select(color => color.ToColor().CGColor).ToArray();
            gradientLayer.StartPoint = new CGPoint(startPoint.X, startPoint.Y);
            gradientLayer.EndPoint = new CGPoint(endPoint.X, endPoint.Y);
            gradientLayer.Locations = locations.Select(number => new NSNumber(number)).ToArray();

            var existingGradientLayer = PlatformView.Layer.Sublayers?.FirstOrDefault(layer => layer is CAGradientLayer);
            if(existingGradientLayer == null)
            {
                PlatformView.Layer.InsertSublayer(gradientLayer, 0);   
            }
            else 
            {
                PlatformView.Layer.ReplaceSublayer(existingGradientLayer, gradientLayer);
            }
            var superlayer = PlatformView.Layer;
            var disposable = superlayer.AddObserver("Bounds", (NSKeyValueObservingOptions)0, (@event) =>
            {
                gradientLayer.Frame = superlayer.Bounds;
            });

            gradientLayer.Delegate = new ObserverDisposingDelegate(disposable);
#elif __ANDROID__
            var shaderFactory = new GradientShaderFactory(
                locations,
                startPoint,
                endPoint,
                gradientColors);

            var paintDrawable = new PaintDrawable();
            paintDrawable.Shape = new RectShape();
            paintDrawable.SetShaderFactory(shaderFactory);

            PlatformView.Background = paintDrawable;
#endif
        }


        public bool Enabled
        {
#if __IOS__
            get => PlatformView is UIControl ? ((UIControl)PlatformView).Enabled : true;
#elif __ANDROID__
            get => PlatformView.Enabled;
#endif
        }

        public void SetEnabled(bool enabled)
        {
#if __IOS__
            if (PlatformView is UIControl) 
                ((UIControl)PlatformView).Enabled = enabled;
#elif __ANDROID__
            PlatformView.Enabled = enabled;
#endif
        }

        public SizeF PreferredSize(SizeF size)
        {
            return PlatformView.PreferredSize(size);
        }

        public void SetNeedsLayout()
        {
#if __IOS__
            PlatformView.SetNeedsLayout();
#elif __ANDROID__
            PlatformView.RequestLayout();
#endif
        }

        public EventCommandTrigger ClickTarget()
        {
#if __IOS__
            var control = PlatformView as UIControl;
            if (control == null) throw new InvalidOperationException("PlatformView is not UIControl");
            return control.ClickTarget();
#elif __ANDROID__
            return PlatformView.ClickTarget();
#endif
        }

        public override string ToString()
        {
            return PlatformView != null ? PlatformView.ToString() : "Empty QView";
        }
    }

    public static class ViewExtensions
    {
        /// <summary>
        /// Create new empty view and add it to parent
        /// </summary>
        public static View View(this ViewBuilder b, bool addSubview = true)
        {
#if __IOS__
            var view = new View() { PlatformView = new PlatformView() };
#elif __ANDROID__
            //View in android cannot have children while in iOS this is ok.
            //Sometimes iOS code creates view and manage it by itself.
            //To make this case work in Android as well return not view, but something
            //which can contain children. First thing which comes in mind is QodenView, 
            //since ViewGroup is abstract.
            var view = new View() { PlatformView = new QodenView(b.Context) };
#endif
            if (addSubview) b.AddSubview(view.PlatformView);
            return view;
        }

        /// <summary>
        /// Wrap convert platform view with cross platform wrapper
        /// </summary>
        public static View AsView(this PlatformView view)
        {
            return new View() { PlatformView = view };
        }
    }
}

