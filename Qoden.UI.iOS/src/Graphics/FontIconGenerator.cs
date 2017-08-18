using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using CoreText;
using ObjCRuntime;
using UIKit;

namespace Qoden.UI
{
	public class IconGenerator : IFontIconGenerator
	{
        FontIconAppearance _iconAppearance;

        public IconGenerator(FontIconAppearance appearance)
		{
            _iconAppearance = appearance ?? throw new ArgumentNullException(nameof(appearance));
		}

        public Image CreateIcon(char icon)
        {
            return CreateIcon(icon, UIEdgeInsets.Zero);   
        }

        public Image CreateIcon(char icon, UIEdgeInsets insets)
        {
            getGlyphCharBuffer[0] = icon;
            getGlyphGlyphBuffer[0] = 0;
            var ctfont = _iconAppearance.Font;
            CTFontGetGlyphsForCharacters(ctfont.Handle, getGlyphCharBuffer, getGlyphGlyphBuffer, 1);

            using (var path = ctfont.GetPathForGlyph(getGlyphGlyphBuffer[0]))
            {
                var glyphBoundingBox = ctfont.BoundingBox;
                var width = _iconAppearance.Font.GetAdvancesForGlyphs(CTFontOrientation.Default, getGlyphGlyphBuffer);
                var height = glyphBoundingBox.Height;
                var imageSize = new CGSize(width + insets.Left + insets.Right, height + insets.Top + insets.Bottom);

                //NOTE: glyph bounds has negative Y origin which absolute value is equal to baseline.
                //this is why traslate and scale is used to convert context coordinates into glyph coordinates.
                var baseLineY = path.BoundingBox.Y;
                UIGraphics.BeginImageContextWithOptions(imageSize, false, 0f);
                try
                {
                    using (var context = UIGraphics.GetCurrentContext())
                    {
                        context.TranslateCTM(insets.Left, imageSize.Height + baseLineY - insets.Bottom);
                        context.ScaleCTM(1, -1);
                        path.RenderInContext(context, _iconAppearance.Colors, _iconAppearance.StrokeColor, _iconAppearance.StrokeWidth);
                        var image = UIGraphics.GetImageFromCurrentImageContext();
                        UIGraphics.EndImageContext();
                        if (image.RenderingMode != _iconAppearance.RenderingMode)
                        {
                            image = image.ImageWithRenderingMode(_iconAppearance.RenderingMode);
                        }
                        return image.AsImage();
                    }
                }
                finally
                {
                    UIGraphics.EndImageContext();
                }
            }
        }

		readonly char[] getGlyphCharBuffer = new char[1];
		readonly ushort[] getGlyphGlyphBuffer = new ushort[1];

		[DllImport(Constants.CoreTextLibrary)]
		static extern bool CTFontGetGlyphsForCharacters(
			IntPtr font,
			[In, MarshalAs(UnmanagedType.LPWStr)] char[] characters,
			[Out] ushort[] glyphs,
			nint count);
    }
}
