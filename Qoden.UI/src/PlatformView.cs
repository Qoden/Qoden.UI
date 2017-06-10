using System;
using System.Drawing;
using System.Reflection;
using Qoden.Util;

#pragma warning disable CS1701 // Assuming assembly reference matches identity
namespace Qoden.UI
{
	/// <summary>
	/// Light weight wrapper for platform view. Platform specific assemblies add expension method to manipulate this view.
	/// </summary>
	/// <remarks>
	/// This struct is intended to either be allocated on stack and short lived or embedded as a field into 
	/// other classes. Make sure not to cast PlatformView to IDisposable too often since this will trigger boxing/unboxing.
	/// </remarks>
	public struct PlatformView : IDisposable
	{
		public static IPlatformViewOperations Operations { get; private set; }

		public PlatformView(object view)
		{
			if (view == null) throw new ArgumentNullException(nameof(view));
			if (!Operations.IsView(view)) throw new ArgumentException();
			Native = view;
		}

		public object Native { get; private set; }

		static PlatformView()
		{
			Operations = Util.Plugin.Load<IPlatformViewOperations>("Qoden.UI", "PlatformViewOperations");
		}

		public void Dispose()
		{
			if (Native is IDisposable)
			{
				((IDisposable)Native).Dispose();
			}
		}

		public void AddSubview(PlatformView child)
		{
			Operations.AddSubview(this, child);
		}

		public void RemoveSubview(PlatformView child)
		{
			Operations.RemoveSubview(this, child);
		}

		public SizeF Bounds
		{
			get { return Operations.Bounds(this); }
			set { Operations.SetBounds(this, value); }
		}

		public RectangleF Frame
		{
			get { return Operations.Frame(this); }
			set { Operations.SetFrame(this, value); }
		}

		public Padding LayoutMargins
		{
			get { return Operations.LayoutMargins(this); }
			set { Operations.SetLayoutMargins(this, value); }
		}

		public PlatformView CreateChild<T>()
		{
			return CreateChild(typeof(T));
		}

		public PlatformView CreateChild(Type t)
		{
			return Operations.CreateView(this, t);
		}
	}
}
