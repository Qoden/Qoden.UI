using System;
using CoreGraphics;
using CoreText;
using UIKit;

namespace Qoden.UI
{
    public static class FontExtensions
    {
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
    }


}
