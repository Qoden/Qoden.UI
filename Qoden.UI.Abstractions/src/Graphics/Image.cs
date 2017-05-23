using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Qoden.View
{
	public class Image
	{
		public static IImageOperations Operations { get; private set;}

		static Image()
		{
			var t = Type.GetType("Qoden.View.ImageOperations", true);
			Operations = (IImageOperations)Activator.CreateInstance(t);
		}
	}

	public interface IImageOperations 
	{
		Size Size(object image);

		Task<object> LoadFromStream(Stream stream, CancellationToken token);
		Task Write(object image, Stream stream, CancellationToken token);

		Task<object> Resize(object image, float sx, float sy);

		void Dispose(object image);
	}

	public static class ImageOperations 
	{
		public static Task<object> LoadFromStream(this IImageOperations ops, Stream stream)
		{
			return ops.LoadFromStream(stream, CancellationToken.None);
		}

		public static Task Write(this IImageOperations ops, object image, Stream stream)
		{
			return ops.Write(image, stream, CancellationToken.None);
		}

		public static Task<object> AspectFitInSize(this IImageOperations ops, object image, float sx, float sy)
		{
			var size = ops.Size(image);
			var maxRatio = Math.Min(sx / size.Width, sy / size.Height);
			if (maxRatio >= 1) return Task.FromResult(image);
			return ops.Resize(image, (float)(size.Width * maxRatio), (float)(size.Height * maxRatio));
		}
	}
}
