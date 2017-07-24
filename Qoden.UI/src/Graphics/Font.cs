using System;
namespace Qoden.UI
{
    public struct Font
    {
        public Font(string name, float size = 10, FontStyle style = FontStyle.Normal)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Size = size;
            Style = style;
        }

        public string Name;
        public float Size;
        public FontStyle Style;
    }

    public enum FontStyle
    {
        Bold = 1,
        BoldItalic = 3,
        Italic = 2,
        Normal = 0
    }
}
