using System;
using System.Reflection;
#pragma warning disable CS1701 // Assuming assembly reference matches identity
namespace Qoden.UI
{
	public struct PlatformFont : IDisposable
	{
		public object Native { get; private set; }

		public static IPlatformFontOperations Operations { get; private set; }

		public PlatformFont(string name, float size)
		{
			var font = Operations.FontWithSize(name, size);
			if (font == null) throw new ArgumentException();
			Native = font;
		}

		public PlatformFont(object font)
		{
			if (font == null) throw new ArgumentNullException(nameof(font));
			if (!Operations.IsFont(font)) throw new ArgumentException();
			Native = font;
		}

		public void Dispose()
		{
			if (Native is IDisposable)
			{
				((IDisposable)Native).Dispose();
			}
		}

		static PlatformFont()
		{
			Operations = Plugin.Load<IPlatformFontOperations>("PlatformFontOperations");
		}
	}
}
