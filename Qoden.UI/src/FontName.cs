using System;

namespace Qoden.UI
{
	/// <summary>
	/// Font with well known name which can be loaded by just supplying font name. 
	/// Usually such fonts are bundled with app or provide by system (example - Arial).
	/// </summary>
	public class FontName
	{
		public FontName(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException();
			Name = name;
		}

		public string Name { get; private set; }

		public PlatformFont FontWithSize(float size)
		{
			return new PlatformFont(Name, size);
		}

		public FontGlyph FontIcon(char icon, float size)
		{
			return new FontGlyph
			{
				IconIndex = icon,
				Font = FontWithSize(size)
			};
		}
	}

}
