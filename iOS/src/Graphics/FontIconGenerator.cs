using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using CoreText;
using Foundation;
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
            var ctfont = _iconAppearance.CoreTextFont;
            var uifont = _iconAppearance.UIKitFont;
            CTFontGetGlyphsForCharacters(ctfont.Handle, getGlyphCharBuffer, getGlyphGlyphBuffer, 1);

            using (var path = ctfont.GetPathForGlyph(getGlyphGlyphBuffer[0]))
            {
                var glyphBoundingBox = ctfont.BoundingBox;
                var width = _iconAppearance.CoreTextFont.GetAdvancesForGlyphs(CTFontOrientation.Default, getGlyphGlyphBuffer);
                var height = uifont.LineHeight;
                var imageSize = new CGSize(width + insets.Left + insets.Right, height + insets.Top + insets.Bottom);

                //Baseline can be calculated by subtracting Descender and Leading from Line Height.
                //Descender is already negative.
                var baseLineY = height - (-uifont.Descender) - uifont.Leading;
                UIGraphics.BeginImageContextWithOptions(imageSize, false, 0f);
                try
                {
                    using (var context = UIGraphics.GetCurrentContext())
                    {
                        context.TranslateCTM(insets.Left, baseLineY - insets.Bottom);
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
