using System;
using UIKit;

namespace Qoden.UI
{
    public static class FontExtensions
    {
        public static UIFont ToFont(this Font font)
        {
            var nsFont = UIFont.FromName(font.Name, font.Size);
            UIFontDescriptorSymbolicTraits trait;
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
