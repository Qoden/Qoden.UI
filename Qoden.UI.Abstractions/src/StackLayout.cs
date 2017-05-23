using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
#pragma warning disable CS1701 // Assuming assembly reference matches identity
namespace Qoden.UI
{
	public enum LayoutDirection
	{
		TopBottom,
		BottomTop,
		LeftRight,
		RightLeft,
	}

	public struct ViewFramePair
	{
		public PlatformView View;
		public RectangleF Frame;
	}

	public class StackLayout : IEnumerable
	{
		readonly List<StackLayoutInfo> views = new List<StackLayoutInfo>();

		public StackLayout()
		{
			Spacing = 6;
			Direction = LayoutDirection.TopBottom;
			Size = float.MinValue;
		}

		public LayoutDirection Direction { get; set; }

		public float Spacing { get; set; }

		public bool Flow { get; set; }

		public float Size { get; set; }

		public IEnumerator GetEnumerator()
		{
			return views.GetEnumerator();
		}

		public void Add(PlatformView view)
		{
			Add(view, float.MinValue);
		}

		public void Add(PlatformView view, Func<LayoutBox, LayoutBox> custom)
		{
			Add(view, float.MinValue, custom);
		}

		public void Add(PlatformView view, float size)
		{
			Add(view, size, Padding.Zero);
		}

		public void Add(PlatformView view, float size, Padding margins)
		{
			Add(new StackLayoutInfo
			{
				View = view,
				Size = size,
				Margins = margins
			});
		}

		public void Add(PlatformView view, float size, Func<LayoutBox, LayoutBox> custom)
		{
			Add(new StackLayoutInfo
			{
				View = view,
				Size = size,
				Custom = custom
			});
		}

		public void Add(StackLayoutInfo info)
		{
			var idx = views.FindIndex(li => li.View.Native == info.View.Native);
			if (idx >= 0)
			{
				views.RemoveAt(idx);
			}
			views.Add(info);
		}

		public void Remove(PlatformView view)
		{
			var idx = views.FindIndex(li => li.View.Native == view.Native);
			if (idx >= 0)
			{
				views.RemoveAt(idx);
			}
		}

		public void ChangeSize(PlatformView view, float size)
		{
			var idx = views.FindIndex(li => li.View.Native == view.Native);
			ChangeSize(idx, size);
		}

		public void ChangeSize(int idx, float size)
		{
			var li = views[idx];
			li.Size = size;
			views[idx] = li;
		}

		public IEnumerable<ViewFramePair> Measure(PlatformView parent)
		{
			var bounds = parent.Bounds;
			var margin = parent.LayoutMargins;
			var layoutRect = new RectangleF(
								 margin.Left,
								 margin.Top,
								 bounds.Width - margin.Right - margin.Left,
								 bounds.Height - margin.Bottom - margin.Top);
			return Measure(layoutRect);
		}

		public void Layout(PlatformView parent)
		{
			var bounds = parent.Bounds;
			var margin = parent.LayoutMargins;
			var layoutRect = new RectangleF(
								 margin.Left,
								 margin.Top,
								 bounds.Width - margin.Right - margin.Left,
								 bounds.Height - margin.Bottom - margin.Top);
			Layout(layoutRect);
		}

		public void Layout(RectangleF bounds)
		{
			foreach (var kv in Measure(bounds))
			{
				var view = kv.View;
				view.Frame = kv.Frame;				
			}
		}

		float ViewSize(StackLayoutInfo li)
		{
			if (Math.Abs(li.Size - float.MinValue) > float.Epsilon)
			{
				return li.Size;
			}
			else if (Math.Abs(Size - float.MinValue) > float.Epsilon)
			{
				return Size;
			}
			else
			{
				var bounds = li.View.Bounds;
				switch (Direction)
				{
					case LayoutDirection.LeftRight:
					case LayoutDirection.RightLeft:
						return bounds.Width;
					case LayoutDirection.TopBottom:
					case LayoutDirection.BottomTop:
						return bounds.Height;
					default:
						return 0;
				}
			}
		}

		public IEnumerable<ViewFramePair> Measure(RectangleF bounds)
		{
			PointF origin;
			switch (Direction)
			{
				case LayoutDirection.LeftRight:
				case LayoutDirection.TopBottom:
					origin = bounds.Location;
					break;
				case LayoutDirection.BottomTop:
					origin = new PointF(bounds.Left, bounds.Bottom);
					break;
				case LayoutDirection.RightLeft:
					origin = new PointF(bounds.Right, bounds.Top);
					break;
				default:
					throw new InvalidOperationException("Unknown layout direction");
			}
			float maxHeight = 0, maxWidth = 0;
			foreach (var li in views)
			{
				RectangleF boundingBox;
				switch (Direction)
				{
					case LayoutDirection.LeftRight:
						{
							var width = ViewSize(li);
							width = width + li.Margins.Left + li.Margins.Right;
							var height = bounds.Size.Height;
							boundingBox = new RectangleF(origin.X, origin.Y, width, height);

							origin = new PointF(boundingBox.Right + Spacing, boundingBox.Top);
							maxHeight = Math.Max(maxHeight, height);
							if (Flow && origin.X >= bounds.Right)
							{
								origin.X = bounds.X;
								origin.Y = origin.Y + maxHeight;
								maxHeight = 0;
							}
						}
						break;
					case LayoutDirection.RightLeft:
						{
							var width = ViewSize(li);
							width = width + li.Margins.Left + li.Margins.Right;
							var height = bounds.Size.Height;
							boundingBox = new RectangleF(origin.X - width, origin.Y, width, height);

							origin = new PointF(boundingBox.Left - Spacing, boundingBox.Top);
							maxHeight = Math.Max(maxHeight, height);
							if (Flow && origin.X <= 0)
							{
								origin.X = bounds.Right;
								origin.Y = origin.Y + maxHeight;
								maxHeight = 0;
							}
						}
						break;
					case LayoutDirection.TopBottom:
						{
							var width = bounds.Size.Width;
							var height = ViewSize(li);
							height = height + li.Margins.Top + li.Margins.Bottom;
							boundingBox = new RectangleF(origin.X, origin.Y, width, height);

							origin = new PointF(boundingBox.Left, boundingBox.Bottom + Spacing);
							maxWidth = Math.Max(maxWidth, width);
							if (Flow && origin.Y >= bounds.Bottom)
							{
								origin.X = bounds.X + maxWidth;
								origin.Y = bounds.Y;
								maxWidth = 0;
							}
						}
						break;
					case LayoutDirection.BottomTop:
						{
							var width = bounds.Size.Width;
							var height = ViewSize(li);
							height = height + li.Margins.Top + li.Margins.Bottom;
							boundingBox = new RectangleF(origin.X, origin.Y - height, width, height);

							origin = new PointF(boundingBox.Left, boundingBox.Top - Spacing);
							maxWidth = Math.Max(maxWidth, width);
							if (Flow && origin.Y >= bounds.Top)
							{
								origin.X = bounds.X + maxWidth;
								origin.Y = bounds.Bottom;
								maxWidth = 0;
							}
						}
						break;
					default:
						throw new NotImplementedException("Layout for this direction is not implemented");
				}

				var box = new LayoutBox(boundingBox)
					.Top(li.Margins.Top)
					.Left(li.Margins.Left)
					.Bottom(li.Margins.Bottom)
					.Right(li.Margins.Right);
				if (li.Custom != null)
				{
					box = li.Custom(box);
				}
				yield return new ViewFramePair
				{
					View = li.View,
					Frame = box.Bounds
				};
			}
		}

		public void Clear()
		{
			views.Clear();
		}
	}
}

