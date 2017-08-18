using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qoden.UI
{

#if __IOS__
using PlatformImage = UIKit.UIImage;
#endif
#if __ANDROID__
    using PlatformImage = Android.Graphics.Drawables.Drawable;
#endif

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
        IPlatformRemoteImageView _view;

        public RemoteImageViewModel(IPlatformRemoteImageView owner)
        {
            _view = owner ?? throw new ArgumentNullException();
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
                                _view.SetImage(_placeholder);
                            }
                            _view.OnFireImageChanged();
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
                    _view.SetImage(_placeholder);
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
                    _view.SetImage(_placeholder);
                    _view.OnLoadingStarted();
                    await image.Load(token);
                }
                catch (Exception e)
                {                    
                    _view.SetImage(_placeholder);
                    if (e is OperationCanceledException) return;
                    throw;
                }
                finally 
                {
                    _view.OnLoadingFinished();    
                }
            }
            if (image.Bitmap == null)
            {
                _view.SetImage(_placeholder);
            }
            else
            {
                _view.SetImage(image.Bitmap);
            }
        }
    }
}
