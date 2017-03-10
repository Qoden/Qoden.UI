using System;
using CoreGraphics;
using UIKit;

namespace Qoden.UI.iOS
{
	public class FontIconAppearance : IDisposable
	{
		public FontIconAppearance()
		{
			Colors = new[] { UIColor.DarkGray.CGColor };
			StrokeColor = UIColor.Black.CGColor;
			StrokeWidth = 0;
		}

		private CGColor[] colors;
		public CGColor[] Colors
		{
			get { return colors; }
			set
			{
				if (value == null) throw new ArgumentNullException(nameof(value));
				if (value.Length == 0) throw new ArgumentException("Colors cannot be empty");
				foreach (var c in colors)
					c.Dispose();
				colors = value;
			}
		}

		public UIImageRenderingMode RenderingMode { get; set; } = UIImageRenderingMode.Automatic;

		private CGColor strokeColor;
		public CGColor StrokeColor
		{
			get { return strokeColor; }
			set
			{
				if (value == null) throw new ArgumentNullException(nameof(value));
				strokeColor = value;
			}
		}

		float strokeWidth = 1f;
		public float StrokeWidth
		{
			get { return strokeWidth; }
			set
			{
				if (value < 0) throw new ArgumentException("StrokeWidth cannot be negative");
				strokeWidth = value;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (var c in colors)
					c.Dispose();
				StrokeColor?.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		~FontIconAppearance()
		{
			Dispose(false);
		}
	}
}
