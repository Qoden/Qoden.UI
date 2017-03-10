using System;
namespace Qoden.UI
{
	/// <summary>
	/// Generate image from font glyph.
	/// </summary>
	public interface IFontIconGenerator : IDisposable
	{
		PlatformImage CreateIcon(FontGlyph icon);
	}
}
