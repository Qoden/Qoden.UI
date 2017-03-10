using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Qoden.UI
{
	public interface IPlatformImageOperations
	{
		bool IsImage(object image);
		Size Size(PlatformImage image);
		Task<PlatformImage> LoadFromStream(Stream stream, CancellationToken token);
		Task<PlatformImage> Resize(PlatformImage image, float sx, float sy);
		Task Write(PlatformImage image, Stream stream, CancellationToken token);
	}
}
