using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace Qoden.UI
{
    public class PlatformImageOperations : IPlatformImageOperations
    {
        public Size Size(PlatformImage image)
        {
            var nativeImage = image.Native as Drawable;
            return new Size(nativeImage.IntrinsicWidth, nativeImage.IntrinsicHeight);
        }

        public Task<PlatformImage> LoadFromStream(System.IO.Stream stream, CancellationToken token)
        {
            return Task.Run(() =>
            {
                var impl = Drawable.CreateFromStream(stream, null);
                token.ThrowIfCancellationRequested();
                return new PlatformImage(impl);
            });
        }

        public Task<PlatformImage> Resize(PlatformImage image, float sx, float sy)
        {            
            return Task.Run(() => 
            {
                var impl = image.Native as BitmapDrawable;
                if (impl == null) throw new ImageOperationException();
                var b = impl.Bitmap;
                var sizeX = Math.Round(impl.IntrinsicWidth * sx);
                var sizeY = Math.Round(impl.IntrinsicHeight * sy);
                var resized = Bitmap.CreateScaledBitmap(b, (int)sizeX, (int)sizeY, false);
                return new PlatformImage(new BitmapDrawable(resized));
            });
        }

        public async Task Write(PlatformImage image, System.IO.Stream stream, CancellationToken token)
        {
            await Task.Run(async () =>
            {
                var impl = image.Native as BitmapDrawable;
                if (impl == null) throw new ImageOperationException();
                var bitmap = impl.Bitmap;
                await bitmap.CompressAsync(Bitmap.CompressFormat.Png, 100, stream);
                token.ThrowIfCancellationRequested();
            });
        }

        public bool IsImage(object image)
        {
            return image is Drawable;
        }
    }

    public static class PlatformImageExtensions
    {
        public static Drawable Drawable(this PlatformImage image)
        {
            return image.Native as Drawable;
        }
    }
}
