using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

#if __IOS__
using UIKit;
using Foundation;
using CoreGraphics;
using PlatformImage = UIKit.UIImage;
#endif
#if __ANDROID__
using Android.Graphics;
using Android.Graphics.Drawables;
using PlatformImage = Android.Graphics.Drawables.Drawable;
#endif

namespace Qoden.UI
{
    public struct Image
    {
        public static implicit operator PlatformImage(Image image) { return image.PlatformImage; }

        public PlatformImage PlatformImage { get; set; }

        public SizeF Size()
        {
#if __IOS__
            return (SizeF)PlatformImage.Size;
#elif __ANDROID__
            return new Size(PlatformImage.IntrinsicWidth, PlatformImage.IntrinsicHeight);
#else
            return SizeF.Empty;
#endif
        }

        public static Task<Image> LoadFromStream(System.IO.Stream stream, CancellationToken token)
        {
#if __IOS__
            return Task.Run(() =>
            {
                var data = NSData.FromStream(stream);
                token.ThrowIfCancellationRequested();
                return new Image() { PlatformImage = new UIImage(data) };
            });
#elif __ANDROID__
            return Task.Run(() =>
            {
                var impl = Drawable.CreateFromStream(stream, null);
                token.ThrowIfCancellationRequested();
                return new Image() { PlatformImage = impl };
            });
#endif
        }

        public Task<Image> Resize(float sx, float sy)
        {
#if __IOS__
            var impl = PlatformImage;
            return Task.Run(() => Task.FromResult(new Image() { PlatformImage = impl.Scale(new CGSize(sx, sy)) }));
#endif

#if __ANDROID__
            var impl = PlatformImage as BitmapDrawable;
            if (impl == null) throw new ImageOperationException();
            return Task.Run(() =>
            {

                var b = impl.Bitmap;
                var sizeX = Math.Round(impl.IntrinsicWidth * sx);
                var sizeY = Math.Round(impl.IntrinsicHeight * sy);
                var resized = Bitmap.CreateScaledBitmap(b, (int)sizeX, (int)sizeY, false);
                return new Image() { PlatformImage = new BitmapDrawable(resized) };
            });
#endif
        }

        public async Task Write(System.IO.Stream stream, CancellationToken token)
        {
#if __IOS__
            var impl = PlatformImage;
            await Task.Run(async () =>
            {
                await impl.AsPNG().AsStream().CopyToAsync(stream);
                token.ThrowIfCancellationRequested();
            });
#endif

#if __ANDROID__
            var impl = PlatformImage as BitmapDrawable;
            if (impl == null) throw new ImageOperationException();
            await Task.Run(async () =>
            {
                var bitmap = impl.Bitmap;
                await bitmap.CompressAsync(Bitmap.CompressFormat.Png, 100, stream);
                token.ThrowIfCancellationRequested();
            });
#endif
        }

        public static Image GetByName(string name)
        {
#if __ANDROID__
            var drawable = ImageFactory.GetDrawable(name);
            return new Image() { PlatformImage = drawable };
#elif __IOS__
            return new Image { PlatformImage = UIImage.FromBundle(name) };
#endif
        }
    }

    public static class ImageExtensions
    {
        public static Image AsImage(this PlatformImage image)
        {
            return new Image() { PlatformImage = image };
        }
    }

    public class ImageOperationException : Exception
    {
    }
}
