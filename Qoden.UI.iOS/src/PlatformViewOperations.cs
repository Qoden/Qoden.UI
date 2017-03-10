using System;
using System.Drawing;
using CoreGraphics;
using UIKit;
using Qoden.UI;

namespace Qoden.UI.Platform.iOS
{
	public class PlatformViewOperations : IPlatformViewOperations
	{
		static readonly Type[] NoArguments = { };

		public bool IsView(object view)
		{
			return view is UIView;
		}

		public void AddSubview(PlatformView parent, PlatformView child)
		{
			((UIView)parent.Native).AddSubview((UIView)child.Native);
		}

#pragma warning disable RECS0154 // Parameter is never used
		public void RemoveSubview(PlatformView parent, PlatformView child)
		{
			((UIView)child.Native).RemoveFromSuperview();
		}

		public PlatformView CreateView(PlatformView? parent, Type outletType)
		{
			var constructor = outletType.GetConstructor(NoArguments);
			if (constructor == null)
			{
				//TODO
				//LOG.Error("Cannot find default constructor for {0}. Please check type has default constructor and is referenced somewhere in your code to ensure linker does not remove it. See LinkerHack class for example how to trick linker.", outletType);
			}
			var view = outletType.GetConstructor(NoArguments).Invoke(NoArguments) as UIView;
			if (parent != null)
			{
				((UIView)parent.Value.Native).AddSubview(view);
			}
			return new PlatformView(view);
		}
#pragma warning restore RECS0154 // Parameter is never used

		public SizeF Bounds(PlatformView view)
		{
			return (SizeF)((UIView)view.Native).Bounds.Size;
		}

		public void SetBounds(PlatformView view, SizeF bounds)
		{
			((UIView)view.Native).Bounds = new CGRect(CGPoint.Empty, bounds);
		}

		public RectangleF Frame(PlatformView view)
		{
			return (RectangleF)((UIView)view.Native).Frame;
		}

		public void SetFrame(PlatformView view, RectangleF frame)
		{
			((UIView)view.Native).Frame = frame;
		}

		public Padding LayoutMargins(PlatformView view)
		{
			var insets = ((UIView)view.Native).LayoutMargins;
			return insets.AsPadding();
		}

		public void SetLayoutMargins(PlatformView view, Padding margins)
		{
			((UIView)view.Native).LayoutMargins = margins.AsEdgeInsets();
		}
	}
}
