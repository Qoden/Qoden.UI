using System;
#pragma warning disable CS1701 // Assuming assembly reference matches identity
namespace Qoden.UI
{
	public struct StackLayoutInfo
	{
		public PlatformView View;
		public float Size;
		public Padding Margins;
		public Func<LayoutBox, LayoutBox> Custom;

		public StackLayoutInfo(PlatformView view) : this(view, float.MinValue)
		{
		}

		public StackLayoutInfo(PlatformView view, float size) : this(view, size, Padding.Zero)
		{
		}

		public StackLayoutInfo(PlatformView view, float size, Padding margins)
			: this(view, size, margins, null)
		{
		}

		public StackLayoutInfo(PlatformView view, float size, Func<LayoutBox, LayoutBox> custom)
			: this(view, size, Padding.Zero, custom)
		{
		}

		StackLayoutInfo(PlatformView view, float size, Padding margins, Func<LayoutBox, LayoutBox> custom)
		{
			View = view;
			Size = size;
			Custom = custom;
			Margins = margins;
		}
	}
}