using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qoden.UI
{
    public interface IPlatformRemoteImageView
    {
        void SetImage(PlatformImage image);
        void OnFireImageChanged();
        void OnLoadingStarted();
        void OnLoadingFinished();
    }

    public class RemoteImageViewModel
    {
        RemoteImage _image;
        CancellationTokenSource _cts = new CancellationTokenSource();
        PlatformImage _placeholder;
        IPlatformRemoteImageView _platform;

        public RemoteImageViewModel(IPlatformRemoteImageView owner)
        {
            _platform = owner ?? throw new ArgumentNullException();
        }

        public RemoteImage Image
        {
            get { return _image; }
            set
            {
                if (value != _image)
                {
                    _image = value;
                    _cts.Cancel();
                    _cts = new CancellationTokenSource();
                    SetRemoteImage(value, _cts.Token)
                        .ContinueWith(t =>
                        {
                            if (t.Exception != null)
                            {
                                _platform.SetImage(_placeholder);
                            }
                            _platform.OnFireImageChanged();
                        });
                }
            }
        }

        public PlatformImage Placeholder
        {
            get { return _placeholder; }
            set
            {
                _placeholder = value;
                if (_image == null || !_image.IsLoaded)
                {
                    _platform.SetImage(_placeholder);
                }
            }
        }

        public async Task SetRemoteImage(RemoteImage image, CancellationToken token)
        {
            if (image == null)
                throw new ArgumentNullException();
            if (this._image == image)
            {
                return;
            }
            this._image = image;
            if (!this._image.IsLoaded)
            {
                try
                {
                    _platform.SetImage(_placeholder);
                    _platform.OnLoadingStarted();
                    await image.Load(token);
                }
                catch (Exception e)
                {                    
                    _platform.SetImage(_placeholder);
                    if (e is OperationCanceledException) return;
                    throw;
                }
                finally 
                {
                    _platform.OnLoadingFinished();    
                }
            }
            if (image.Bitmap.Native == null)
            {
                _platform.SetImage(_placeholder);
            }
            else
            {
                _platform.SetImage(image.Bitmap);
            }
        }
    }
}
