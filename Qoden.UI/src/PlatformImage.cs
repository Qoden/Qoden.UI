using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable CS1701 // Assuming assembly reference matches identity
namespace Qoden.UI
{
	public struct PlatformImage : IDisposable
	{
		public static IPlatformImageOperations Operations { get; private set; }

		public PlatformImage(object image)
		{
			if (image == null) throw new ArgumentNullException(nameof(image));
			if (!Operations.IsImage(image)) throw new ArgumentException();
			Native = image;
		}

		public object Native { get; private set; }

		public void Dispose()
		{
			if (Native is IDisposable)
			{
				((IDisposable)Native).Dispose();
			}
		}

		static PlatformImage()
		{
			Operations = Plugin.Load<IPlatformImageOperations>("PlatformImageOperations");
		}

		public Size Size { get { return Operations.Size(this); } }

		public static Task<PlatformImage> LoadFromStream(Stream stream, CancellationToken token)
		{
			return Operations.LoadFromStream(stream, token);
		}

		public Task<PlatformImage> Resize(float sx, float sy)
		{
			return Operations.Resize(this, sx, sy);
		}

		public Task Write(Stream stream, CancellationToken token)
		{
			return Operations.Write(this, stream, token);
		}
	}
}
