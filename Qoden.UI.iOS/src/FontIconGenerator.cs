using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using CoreText;
using ObjCRuntime;
using Qoden.UI.Platform.iOS;
using UIKit;

namespace Qoden.UI.iOS
{
	public class IconGenerator : IFontIconGenerator
	{
		public static IFontIconGenerator Create(FontIconAppearance appearance)
		{
			return new IconGenerator(appearance);
		}

		public static IFontIconGenerator Create()
		{
			return new IconGenerator(new FontIconAppearance());
		}

		FontIconAppearance iconAppearance;

		private IconGenerator(FontIconAppearance appearance)
		{
			if (appearance == null) throw new ArgumentNullException(nameof(appearance));
			iconAppearance = new FontIconAppearance();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				iconAppearance?.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		~IconGenerator()
		{
			Dispose(false);
		}

		public PlatformImage CreateIcon(FontGlyph icon)
		{
			using (var path = CreatePath(icon.Font.CTFont(), icon.IconIndex))
			{
				var image = CreateIconFromPath(path, iconAppearance);
				return new PlatformImage(image);
			}
		}

		public CGPath CreatePath(CTFont ctfont, char icon)
		{
			var transform = CGAffineTransform.MakeIdentity();
			return ctfont.GetPathForGlyph(GlyphForIcon(ctfont, icon), ref transform);
		}

		readonly char[] getGlyphCharBuffer = new char[1];
		readonly ushort[] getGlyphGlyphBuffer = new ushort[1];

		public ushort GlyphForIcon(CTFont ctfont, char icon)
		{
			getGlyphCharBuffer[0] = icon;
			getGlyphGlyphBuffer[0] = 0;
			CTFontGetGlyphsForCharacters(ctfont.Handle, getGlyphCharBuffer, getGlyphGlyphBuffer, 1);
			return getGlyphGlyphBuffer[0];
		}

		[DllImport(Constants.CoreTextLibrary)]
		static extern bool CTFontGetGlyphsForCharacters(
			IntPtr font,
			[In, MarshalAs(UnmanagedType.LPWStr)] char[] characters,
			[Out] ushort[] glyphs,
			nint count);

		//float strokeWidth, RGB strokeColor
		static public UIImage CreateIconFromPath(CGPath path, FontIconAppearance appearance)
		{
			//NOTE: glyph bounds are has negative Y origin which absolute value is equal to baseline.
			//this is why traslate and scale is used to convert context coordinates into glyph coordinates.
			var bounds = path.BoundingBox;
			var baseLineY = bounds.Y;
			var imageSize = new CGSize(bounds.Width + appearance.StrokeWidth * 2, bounds.Height + appearance.StrokeWidth * 2);
			UIGraphics.BeginImageContextWithOptions(imageSize, false, 0f);
			try
			{
				using (var context = UIGraphics.GetCurrentContext())
				{
					context.TranslateCTM(0, imageSize.Height + baseLineY);
					context.ScaleCTM(1, -1);
					path.RenderInContext(context, appearance.Colors, appearance.StrokeColor, appearance.StrokeWidth);
					var image = UIGraphics.GetImageFromCurrentImageContext();
					UIGraphics.EndImageContext();
					if (image.RenderingMode != appearance.RenderingMode)
					{
						image = image.ImageWithRenderingMode(appearance.RenderingMode);
					}
					return image;
				}
			}
			finally
			{
				UIGraphics.EndImageContext();
			}
		}
	}
}
