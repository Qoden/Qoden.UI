using System;
namespace Qoden.UI
{
	public struct PlatformColor
	{
		public PlatformColor(object color)
		{
			if (color == null) throw new ArgumentNullException(nameof(color));
			if (!Operations.IsColor(color)) throw new ArgumentException();
			Native = color;
		}

		public static IPlatformColorOperations Operations { get; private set; }

		public object Native { get; private set; }

		public void Dispose()
		{
			if (Native is IDisposable)
			{
				((IDisposable)Native).Dispose();
			}
		}

		static PlatformColor()
		{
			Operations = Plugin.Load<IPlatformColorOperations>("PlatformColorOperations");
		}
	}
}
