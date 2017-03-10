using System;
using System.Drawing;
using CoreGraphics;

namespace Qoden.UI.iOS
{
	public static class LayoutBoxExtensions
	{
		public static LayoutBox LayoutBox (this UIKit.UIView view)
		{
			return new LayoutBox ((RectangleF)view.Bounds);
		}

		public static CGRect AsCGRect(this LayoutBox box)
		{
			return box.Bounds;
		}

		public static LayoutBox LayoutFrame (this UIKit.UIView view)
		{
			return new LayoutBox ((RectangleF)view.Frame);
		}

		public static LayoutBox LayoutBox (this UIKit.UIScrollView view)
		{
			//TODO maybe ContentSize instead of Bounds.Size?
			return new LayoutBox (new RectangleF(Point.Empty, (SizeF)view.Bounds.Size));
		}

		public static LayoutBox Left (this LayoutBox box, nfloat size)
		{
			return box.Left ((float)size);
		}

		public static LayoutBox Left(this LayoutBox box, UIKit.UIView inside, nfloat size)
		{
			return box.Left((RectangleF)inside.Frame, (float)size);
		}

		public static LayoutBox Right (this LayoutBox box, nfloat size)
		{
			return box.Right ((float)size);
		}

		public static LayoutBox Right(this LayoutBox box, UIKit.UIView inside, nfloat size)
		{
			return box.Right((RectangleF)inside.Frame, (float)size);
		}

		public static LayoutBox Top (this LayoutBox box, nfloat size)
		{
			return box.Top ((float)size);
		}

		public static LayoutBox Top(this LayoutBox box, UIKit.UIView inside, nfloat size)
		{
			return box.Top((RectangleF)inside.Frame, (float)size);
		}

		public static LayoutBox Bottom (this LayoutBox box, nfloat size)
		{
			return box.Bottom ((float)size);
		}

		public static LayoutBox Bottom(this LayoutBox box, UIKit.UIView inside, nfloat size)
		{
			return box.Bottom((RectangleF)inside.Frame, (float)size);
		}

		public static LayoutBox Width (this LayoutBox box, nfloat size)
		{
			return box.Width ((float)size);
		}

		public static LayoutBox Width (this LayoutBox box, UIKit.UIView view)
		{
			return box.Width ((float)view.Bounds.Width);
		}

		public static LayoutBox Height (this LayoutBox box, nfloat size)
		{
			return box.Height ((float)size);
		}

		public static LayoutBox Height (this LayoutBox box, UIKit.UIView view)
		{
			return box.Height ((float)view.Bounds.Height);
		}

		public static LayoutBox Before (this LayoutBox box, UIKit.UIView reference, nfloat size)
		{
			return box.Before ((RectangleF)reference.Frame, (float)size);
		}
			
		public static LayoutBox After (this LayoutBox box, UIKit.UIView reference, float size)
		{
			return box.After ((RectangleF)reference.Frame, size);
		}

		public static LayoutBox After (this LayoutBox box, UIKit.UIView reference, nfloat size)
		{
			return box.After ((RectangleF)reference.Frame, (float)size);
		}

		public static LayoutBox Below (this LayoutBox box, UIKit.UIView reference, float size)
		{
			return box.Below ((RectangleF)reference.Frame, size);
		}

		public static LayoutBox Below (this LayoutBox box, UIKit.UIView reference, nfloat size)
		{
			return box.Below ((RectangleF)reference.Frame, (float)size);
		}

		public static LayoutBox Above (this LayoutBox box, UIKit.UIView reference, float size)
		{
			return box.Above ((RectangleF)reference.Frame, size);
		}

		public static LayoutBox Above (this LayoutBox box, UIKit.UIView reference, nfloat size)
		{
			return box.Above ((RectangleF)reference.Frame, (float)size);
		}

		public static LayoutBox CenterHorizontally (this LayoutBox box, UIKit.UIView view)
		{
			return box.CenterHorizontally ((RectangleF)view.Frame);
		}

		public static LayoutBox CenterVertically (this LayoutBox box, UIKit.UIView view)
		{
			return box.CenterVertically ((RectangleF)view.Frame);
		}
		public static LayoutBox CenterHorizontally (this LayoutBox box, nfloat dx)
		{
			return box.CenterHorizontally ((float)dx);
		}
		public static LayoutBox CenterVertically (this LayoutBox box, nfloat dy)
		{
			return box.CenterVertically ((float)dy);
		}
	}
}
