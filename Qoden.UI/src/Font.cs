using System;

namespace Qoden.View
{
	public struct Font
	{
		public Font(object font)
		{
			if (font == null) throw new ArgumentNullException(nameof(font));
			Native = font;
		}

		public Font(string name, float size)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));
			Native = Operations.FontWithSize(name, size);
		}

		public object Native { get; }

		public static IFontOperations Operations { get; private set; }

		static Font()
		{
			var t = Type.GetType("Qoden.View.FontOperations", true);
			Operations = (IFontOperations)Activator.CreateInstance(t);
		}
	}

	public interface IFontOperations
	{
		void Dispose(object nativeFont);
		object FontWithSize(string name, float size);
	}
}

