namespace Qoden.UI
{

	/// <summary>
	/// Template for an icon image. Since icons produced from font glyph can be of different size there is 
	/// one to many relationships between glyp in a font and created images.
	/// FontIconTemplate can be used to generate as many icon images from a given font and glyph as neccecary.
	/// </summary>
	public struct FontIconTemplate
	{
		public FontIconTemplate(FontName font, char icon) : this()
		{
			Font = font;
			IconIndex = icon;
		}

		public FontName Font;
		public char IconIndex;

		public FontGlyph FontIcon(float size)
		{
			return Font.FontIcon(IconIndex, size);
		}
	}

}
