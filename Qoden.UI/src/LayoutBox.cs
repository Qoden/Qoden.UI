using System;
using System.Drawing;
#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
	public class LayoutBox
	{
		RectangleF bounds;

		float left, right, top, bottom, width, height, centerX, centerY;
		const float NOT_SET = -1;

		public LayoutBox (RectangleF bounds)
		{
			this.bounds = bounds;
			left = right = top = bottom = width = height = centerX = centerY = NOT_SET;
		}

		public LayoutBox(int left, int top, int right, int bottom) : this(new Rectangle(left, top, left - right, bottom - top))
		{
		}

		static bool IsSet (float val)
		{
			return Math.Abs (val - NOT_SET) > float.Epsilon;
		}

		void SetLeft (float l)
		{
			if (IsSet (centerX) && IsSet (width))
				centerX = NOT_SET;
			if (IsSet (right) && IsSet (width))
				width = NOT_SET;
			left = l;
		}

		public LayoutBox Left (float l)
		{
			SetLeft (l);
			return this;
		}

		public LayoutBox Left(RectangleF reference, float l)
		{
			SetLeft(reference.Left + l);
			return this;
		}

		void SetRight (float r)
		{
			if (IsSet (centerX) && IsSet (width))
				centerX = NOT_SET;
			if (IsSet (left) && IsSet (width))
				width = NOT_SET;
			right = r;
		}

		public LayoutBox Right (float r)
		{
			SetRight (r);
			return this;
		}

		public LayoutBox Right(RectangleF reference, float r)
		{
			SetRight(bounds.Width - reference.Right + r);
			return this;
		}

		void SetTop (float t)
		{
			if (IsSet (centerY) && IsSet (height))
				centerY = NOT_SET;
			if (IsSet (bottom) && IsSet (height))
				height = NOT_SET;
			top = t;
		}

		public LayoutBox Top (float t)
		{
			SetTop (t);
			return this;
		}

		public LayoutBox Top(RectangleF reference, float t)
		{
			SetTop(reference.Top + t);
			return this;
		}

		void SetBottom (float b)
		{
			if (IsSet (centerY) && IsSet (height))
				centerY = NOT_SET;
			if (IsSet (top) && IsSet (height))
				height = NOT_SET;
			bottom = b;
		}

		public LayoutBox Bottom (float b)
		{
			SetBottom (b);
			return this;
		}

		public LayoutBox Bottom(RectangleF reference, float b)
		{
			SetBottom(bounds.Height - reference.Bottom + b);
			return this;
		}

		public LayoutBox Before (RectangleF reference, float size)
		{
			SetRight (bounds.Width - reference.Left + size);
			return this;
		}

		public LayoutBox After (RectangleF reference, float size)
		{
			SetLeft (reference.Right + size);
			return this;
		}

		public LayoutBox Below (RectangleF reference, float size)
		{
			SetTop (reference.Bottom + size);
			return this;
		}

		public LayoutBox Above (RectangleF reference, float size)
		{
			SetBottom ((bounds.Height - reference.Top) + size);
			return this;
		}

		void SetWidth (float w)
		{
			if (IsSet (left) && IsSet (right)) {
				left = NOT_SET;
			}
			if (IsSet (centerX) && IsSet (left))
				centerX = NOT_SET;
			if (IsSet (centerX) && IsSet (right))
				centerX = NOT_SET;
			width = w;
		}

		public LayoutBox Width (float w)
		{
			SetWidth (w);
			return this;
		}

		void SetHeight (float h)
		{
			if (IsSet (top) && IsSet (bottom)) {
				top = NOT_SET;
			}
			if (IsSet (centerY) && IsSet (top))
				centerY = NOT_SET;
			if (IsSet (centerY) && IsSet (bottom))
				centerY = NOT_SET;
			height = h;
		}

		public LayoutBox Height (float h)
		{
			SetHeight (h);
			return this;
		}

		public LayoutBox Width (RectangleF view)
		{
			return Width (view.Width);
		}

		public LayoutBox Width ()
		{
			return Width (bounds);
		}

		public LayoutBox Height (RectangleF view)
		{
			return Height (view.Height);
		}

		public LayoutBox Height ()
		{
			return Height (bounds);
		}

		void SetCenterX (float cx)
		{
			if (IsSet (left) && IsSet (right)) {
				width = right - left;
				left = right = NOT_SET;
			}
			if (IsSet (left) && IsSet (width))
				width = NOT_SET;
			if (IsSet (right) && IsSet (width))
				width = NOT_SET;
			centerX = cx;
		}

		public LayoutBox CenterHorizontally (RectangleF view)
		{
			SetCenterX (view.Left + view.Width / 2);
			return this;
		}

		public LayoutBox CenterHorizontally ()
		{
			return CenterHorizontally (bounds);
		}

		public LayoutBox CenterHorizontally (float dx)
		{
			SetCenterX (bounds.Width / 2 + dx);
			return this;
		}

		void SetCenterY (float cy)
		{
			if (IsSet (top) && IsSet (bottom)) {
				height = bounds.Height - bottom - top;
				top = bottom = NOT_SET;
			}
			if (IsSet (top) && IsSet (height))
				height = NOT_SET;
			if (IsSet (bottom) && IsSet (height))
				height = NOT_SET;

			centerY = cy;
		}

		public LayoutBox CenterVertically (RectangleF view)
		{
			SetCenterY (view.Y + view.Height / 2);
			return this;
		}

		public LayoutBox CenterVertically (float dy)
		{
			SetCenterY (bounds.Height / 2 + dy);
			return this;
		}

		public LayoutBox CenterVertically ()
		{
			return CenterVertically (bounds);
		}

		public float LayoutWidth {
			get { 
				if (IsSet (width))
					return width;
				if (IsSet (left) && IsSet (right))
					return bounds.Width - right - left;
				if (IsSet (centerX) && IsSet (left))
					return (centerX - left) * 2;
				if (IsSet (centerX) && IsSet (right))
					return (centerX - right) * 2;
				return 0;
			}
		}

		public float LayoutHeight {
			get { 
				if (IsSet (height))
					return height;
				if (IsSet (top) && IsSet (bottom))
					return bounds.Height - bottom - top;
				if (IsSet (centerY) && IsSet (top))
					return (centerY - top) * 2;
				if (IsSet (centerY) && IsSet (bottom))
					return (centerY - bottom) * 2;
				return 0;
			}
		}

		public float LayoutLeft {
			get { 
				if (IsSet (left))
					return bounds.Left + left;
				if (IsSet (centerX) && IsSet (width))
					return bounds.Left + centerX - width / 2;
				if (IsSet (right) && IsSet (width))
					return bounds.Right - right - width;
				return 0;
			}
		}

		public float LayoutRight {
			get { 
				if (IsSet (right))
					return bounds.Right - right;
				if (IsSet (centerX) && IsSet (width))
					return bounds.Left + centerX + width / 2;
				if (IsSet (left) && IsSet (width))
					return bounds.Left + left + width;
				return 0;
			}
		}

		public float LayoutTop {
			get { 
				if (IsSet (top))
					return bounds.Top + top;
				if (IsSet (centerY) && IsSet (height))
					return bounds.Top + centerY - height / 2;
				if (IsSet (bottom) && IsSet (height))
					return bounds.Bottom - bottom - height;
				return 0;
			}
		}

		public float LayoutBottom {
			get { 
				if (IsSet (bottom))
					return bounds.Bottom - bottom;
				if (IsSet (centerY) && IsSet (height))
					return bounds.Top + centerY + height / 2;
				if (IsSet (top) && IsSet (height)) {
					return bounds.Top + top + height;
				}
				return NOT_SET;
			}
		}

		public PointF LayoutCenter { 
			get { return new PointF (LayoutLeft + LayoutWidth / 2, LayoutTop + LayoutHeight / 2); }
		}

		public SizeF LayoutSize {
			get { return new SizeF (LayoutWidth, LayoutHeight); }
		}

		public RectangleF Bounds { 
			get {
				return new RectangleF (LayoutLeft, LayoutTop, LayoutWidth, LayoutHeight);
			}
		}
	}
}

