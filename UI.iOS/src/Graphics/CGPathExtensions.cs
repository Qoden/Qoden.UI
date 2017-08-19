using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using ObjCRuntime;

namespace Qoden.UI
{
	public static class CGPathExtensions
	{
		public unsafe static CGPath CopyByTransformingPath(this CGPath path, CGAffineTransform transform)
		{
			var handle = CGPathCreateCopyByTransformingPath(path.Handle, &transform);
			return new CGPath(handle);
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		unsafe static extern IntPtr CGPathCreateCopyByTransformingPath(
			IntPtr path,
			CGAffineTransform* transform
		);


		// Prevent +1 on values that are slightly too big (e.g. 24.000001).
		const float EPSILON = 0.01f;

		static CGSize RoundImageSize(CGSize size)
		{
			return new CGSize(
				(float)Math.Ceiling(size.Width - EPSILON),
				(float)Math.Ceiling(size.Height - EPSILON));
		}

		public static void RenderInContext(this CGPath path,
											CGContext context,
											CGColor[] cgColors,
											CGColor strokeColor = null,
											float strokeWidth = -1)
		{
			context.AddPath(path);
			if (cgColors.Length > 1)
			{
				context.SaveState();
				context.Clip();
				var bounds = path.BoundingBox;
				context.RenderGradientInRect(bounds, cgColors);
				context.RestoreState();
			}
			else
			{
				context.SetFillColor(cgColors[0]);
				context.FillPath();
			}

			context.AddPath(path);
			if (strokeColor != null && strokeWidth > 0.0f)
			{
				context.SetStrokeColor(strokeColor);
				context.SetLineWidth(strokeWidth);
				context.StrokePath();
			}
		}

		public static void RenderGradientInRect(this CGContext context, CGRect bounds, CGColor[] colors)
		{
			var n = colors.Length;
			nfloat[] locations = new nfloat[n];
			for (var i = 0; i < n; i++)
			{
				locations[i] = (nfloat)i / (n - 1);
			}
			using (var gradient = new CGGradient(null, colors, locations))
			{
				var topLeft = new CGPoint(bounds.GetMinX(), bounds.GetMinY());
				var bottomLeft = new CGPoint(bounds.GetMinX(), bounds.GetMaxY());
				context.DrawLinearGradient(gradient, topLeft, bottomLeft, 0);
			}
		}
	}
}
