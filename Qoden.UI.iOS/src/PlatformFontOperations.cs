using CoreGraphics;
using CoreText;
using UIKit;

namespace Qoden.UI.iOS
{
	public class PlatformFontOperations : IPlatformFontOperations
	{
		public PlatformFont? FontWithSize(string name, float size)
		{
			var font = UIFont.FromName(name, size);
			if (font == null) return null;
			return new PlatformFont(font);
		}

		public bool IsFont(object font)
		{
			return font is UIFont;
		}
	}

	public static class PlatformFontExtensions
	{
		public static UIFont UIFont(this PlatformFont icon)
		{
			return (UIFont)icon.Native;
		}

		public static CTFont CTFont(this PlatformFont icon)
		{
			var uiFont = icon.UIFont();
			using (var cgFont = CGFont.CreateWithFontName(uiFont.FamilyName))
			{
				return new CTFont(cgFont, uiFont.PointSize, CGAffineTransform.MakeIdentity());
			}
		}
	}
}
