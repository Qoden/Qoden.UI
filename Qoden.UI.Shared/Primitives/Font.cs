using System;
#if __IOS__
using CoreGraphics;
using CoreText;
using UIKit;
#endif

namespace Qoden.UI
{
    public struct Font
    {
        public Font(string name, float size = 14, FontStyle style = FontStyle.Unknown)
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
        Normal = 0,
        Unknown = -1
    }

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

    public static class FontExtensions
    {
#if __IOS__
        public static CTFont ToCTFont(this Font font)
        {
            using (var cgFont = CGFont.CreateWithFontName(font.Name))
            {
                return new CTFont(cgFont, font.Size, CGAffineTransform.MakeIdentity());
            }
        }

        public static UIFont ToFont(this Font font)
        {
            var nsFont = UIFont.FromName(font.Name, font.Size);
            if (nsFont == null) return null;

            if (font.Style == FontStyle.Unknown) return nsFont;

            UIFontDescriptorSymbolicTraits trait = UIFontDescriptorSymbolicTraits.ClassUnknown;
            switch (font.Style)
            {
                case FontStyle.Bold:
                    trait = UIFontDescriptorSymbolicTraits.Bold;
                    break;
                case FontStyle.BoldItalic:
                    trait = UIFontDescriptorSymbolicTraits.Bold | UIFontDescriptorSymbolicTraits.Italic;
                    break;
                case FontStyle.Italic:
                    trait = UIFontDescriptorSymbolicTraits.Italic;
                    break;
                default:
                    trait = 0;
                    break;
            }
            using (var desc = nsFont.FontDescriptor.CreateWithTraits(trait))
            {
                return UIFont.FromDescriptor(desc, font.Size);
            }
        }

        public static Font ToFont(this UIFont font)
        {
            FontStyle style;
            var trait = font.FontDescriptor.SymbolicTraits;
            if ((trait & (UIFontDescriptorSymbolicTraits.Bold | UIFontDescriptorSymbolicTraits.Italic)) != 0)
            {
                style = FontStyle.BoldItalic;
            }
            else if ((trait & UIFontDescriptorSymbolicTraits.Bold) != 0)
            {
                style = FontStyle.Bold;
            }
            else if ((trait & UIFontDescriptorSymbolicTraits.Italic) != 0)
            {
                style = FontStyle.Italic;
            }
            else
            {
                style = FontStyle.Normal;
            }

            return new Font()
            {
                Name = font.Name,
                Size = (float)font.PointSize,
                Style = style
            };
        }
#endif
    }
}
