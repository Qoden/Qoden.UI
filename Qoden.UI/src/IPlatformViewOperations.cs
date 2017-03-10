using System;
using System.Drawing;

namespace Qoden.UI
{
	public interface IPlatformViewOperations
	{
		bool IsView(object nativeView);

		void AddSubview(PlatformView parent, PlatformView child);
		void RemoveSubview(PlatformView parent, PlatformView child);
		PlatformView CreateView(PlatformView? parent, Type type);

		SizeF Bounds(PlatformView view);
		void SetBounds(PlatformView view, SizeF bounds);

		RectangleF Frame(PlatformView view);
		void SetFrame(PlatformView view, RectangleF frame);

		void SetLayoutMargins(PlatformView native, Padding value);
		Padding LayoutMargins(PlatformView view);
	}
}
