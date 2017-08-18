using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net.Http;
#if __IOS__
using PlatformImage = UIKit.UIImage;
#endif
#if __ANDROID__
using PlatformImage = Android.Graphics.Drawables.Drawable;
#endif
#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
	public class RemoteImage 
	{
		public RemoteImage()
		{
			IsLoaded = false;
		}

		public RemoteImage(Uri uri) : this()
		{
            Uri = uri ?? throw new ArgumentNullException();
		}

		public RemoteImage(Uri uri, PlatformImage img) : this(uri)
		{
			IsLoaded = true;
			Bitmap = img;
		}

		public Uri Uri { get; set; }
		public bool IsLoaded { get; private set; }

		public PlatformImage Bitmap { get; private set; }

		public Task<bool> LoadTask { get; private set; }

		public Exception LoadError { get; private set; }

		public async Task<bool> Load()
		{			
			return await Load(CancellationToken.None);		
		}

		public async Task<bool> Load(CancellationToken token)
		{
			if (Uri == null) throw new InvalidOperationException();
			if (LoadTask != null) return await LoadTask;
			try 
			{
				LoadTask = MakeLoadTask(token);
				return await LoadTask;
			}
			finally 
			{
				LoadTask = null;
			}
		}

		private async Task<bool> MakeLoadTask(CancellationToken token)
		{
			try 
			{
				Stream stream = null;
				var httpClient = MakeHttpClient();
				using (var response = await httpClient.GetAsync(Uri, token))
				{
					if (response.IsSuccessStatusCode)
					{
						stream = await response.Content.ReadAsStreamAsync();
					}
					else 
					{
						throw new HttpRequestException(response.ReasonPhrase);
					}
				}

				using (var buffer = new MemoryStream())
				{
					await stream.CopyToAsync(buffer, 4096, token);
					buffer.Position = 0;
					Bitmap = await Image.LoadFromStream(buffer, token);
				}

				IsLoaded = true;
				return true;	
			} 
			catch (HttpRequestException e)
			{
				LoadError = e;
			}
			catch (IOException e)
			{
				LoadError = e;
			}
			return false;
		}

		public static Func<HttpClient> MakeHttpClient = () => new HttpClient();
	}
}
