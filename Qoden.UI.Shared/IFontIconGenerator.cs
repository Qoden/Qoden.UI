using System;
namespace Qoden.UI
{
	/// <summary>
	/// Generate image from font glyph.
	/// </summary>
	public interface IFontIconGenerator
	{
        Image CreateIcon(char icon);
	}
}
