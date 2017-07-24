using System;
namespace Qoden.UI
{
    public struct FontIconTemplate
    {
        public string Name;
        public FontStyle Style;
        public char Icon;

        public FontIcon FontIcon(float pointSize)
        {
            return new FontIcon()
            {
                Name = this.Name,
                Style = this.Style,
                Icon = this.Icon,
                Size = pointSize
            };
        }
    }

    public struct FontIcon
    {
        public string Name;
        public FontStyle Style;
        public char Icon;
        public float Size;
    }
	/// <summary>
	/// Generate image from font glyph.
	/// </summary>
	public interface IFontIconGenerator
	{
        PlatformImage CreateIcon(char icon);
	}
}
