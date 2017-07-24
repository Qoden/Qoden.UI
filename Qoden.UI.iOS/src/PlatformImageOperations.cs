using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Qoden.UI
{
	public class PlatformImageOperations : IPlatformImageOperations
	{
		public Size Size(PlatformImage image) { return (Size)((UIImage)image.Native).Size; }

		public Task<PlatformImage> LoadFromStream(Stream stream, CancellationToken token)
		{
			return Task.Run(() =>
			{
				var data = NSData.FromStream(stream);
				token.ThrowIfCancellationRequested();
				return new PlatformImage(new UIImage(data));
			});
		}

		public Task<PlatformImage> Resize(PlatformImage image, float sx, float sy)
		{
			var impl = (UIImage)image.Native;
			return Task.Run(() => Task.FromResult(new PlatformImage(impl.Scale(new CGSize(sx, sy)))));
		}

		public async Task Write(PlatformImage image, Stream stream, CancellationToken token)
		{
			var impl = (UIImage)image.Native;
			await Task.Run(async () =>
			{
				await impl.AsPNG().AsStream().CopyToAsync(stream);
				token.ThrowIfCancellationRequested();
			});
		}

		public bool IsImage(object image)
		{
			return image is UIImage;
		}
	}

	public static class PlatformImageExtensions 
	{
		public static UIImage UIImage(this PlatformImage image)
		{
			return (UIImage)image.Native;
		}
	}
}
