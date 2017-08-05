using System;
using System.Drawing;
using CoreGraphics;

namespace Qoden.UI
{
    public static class LayoutBoxExtensions
    {
        public static CGRect AsCGRect<T>(this T box) where T : LayoutBox
        {
            return box.Frame;
        }

        public static T Left<T>(this T box, nfloat size) where T : LayoutBox
        {
            return box.Left((float)size);
        }

        public static T Right<T>(this T box, nfloat size) where T : LayoutBox
        {
            return box.Right((float)size);
        }

        public static T Top<T>(this T box, nfloat size) where T : LayoutBox
        {
            return box.Top((float)size);
        }

        public static T Bottom<T>(this T box, nfloat size) where T : LayoutBox
        {
            return box.Bottom((float)size);
        }

        public static T Width<T>(this T box, nfloat size) where T : LayoutBox
        {
            return box.Width((float)size);
        }

        public static T Width<T>(this T box, UIKit.UIView view) where T : LayoutBox
        {
            return box.Width((float)view.Bounds.Width);
        }

        public static T Height<T>(this T box, nfloat size) where T : LayoutBox
        {
            return box.Height((float)size);
        }

        public static T Height<T>(this T box, UIKit.UIView view) where T : LayoutBox
        {
            return box.Height((float)view.Bounds.Height);
        }

        public static T Before<T>(this T box, UIKit.UIView reference, nfloat size) where T : LayoutBox
        {
            return box.Before((RectangleF)reference.Frame, (float)size);
        }

        public static T After<T>(this T box, UIKit.UIView reference, float size) where T : LayoutBox
        {
            return box.After((RectangleF)reference.Frame, size);
        }

        public static T After<T>(this T box, UIKit.UIView reference, nfloat size) where T : LayoutBox
        {
            return box.After((RectangleF)reference.Frame, (float)size);
        }

        public static T Below<T>(this T box, UIKit.UIView reference, float size) where T : LayoutBox
        {
            return box.Below((RectangleF)reference.Frame, size);
        }

        public static T Below<T>(this T box, UIKit.UIView reference, nfloat size) where T : LayoutBox
        {
            return box.Below((RectangleF)reference.Frame, (float)size);
        }

        public static T Above<T>(this T box, UIKit.UIView reference, float size) where T : LayoutBox
        {
            return box.Above((RectangleF)reference.Frame, size);
        }

        public static T Above<T>(this T box, UIKit.UIView reference, nfloat size) where T : LayoutBox
        {
            return box.Above((RectangleF)reference.Frame, (float)size);
        }

        public static T CenterHorizontally<T>(this T box, nfloat dx) where T : LayoutBox
        {
            return box.CenterHorizontally((float)dx);
        }

        public static T CenterVertically<T>(this T box, nfloat dy) where T : LayoutBox
        {
            return box.CenterVertically((float)dy);
        }
    }
}
