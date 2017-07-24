using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using Android.Widget;
using System.Threading;
using Android.Runtime;
using Android.Graphics.Drawables;

namespace Qoden.UI
{
    public class RemoteImageView : ImageView, IPlatformRemoteImageView
    {
        RemoteImageViewModel _model;

        public RemoteImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
        }

        public RemoteImageView(Context context) :
            base(context)
        {
            Initialize();
        }

        public RemoteImageView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public RemoteImageView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        protected void OnImageChanged(EventArgs e)
        {
            ImageChanged?.Invoke(this, e);
        }

        void Initialize()
        {
            _model = new RemoteImageViewModel(this);
        }

        public event EventHandler ImageChanged;

        public RemoteImage Image
        {
            get { return _model.Image; }
            set { _model.Image = value; }
        }

        public Drawable Placeholder
        {
            get { return _model.Placeholder.Drawable(); }
            set { _model.Placeholder = new PlatformImage(value); }
        }

        public async Task SetRemoteImage(RemoteImage image, CancellationToken token)
        {
            await _model.SetRemoteImage(image, token);
        }

        void IPlatformRemoteImageView.SetImage(PlatformImage image)
        {
            SetImageDrawable(image.Drawable());
        }

        void IPlatformRemoteImageView.OnFireImageChanged()
        {
            ImageChanged?.Invoke(this, EventArgs.Empty);
        }

        void IPlatformRemoteImageView.OnLoadingStarted()
        {
        }

        void IPlatformRemoteImageView.OnLoadingFinished()
        {
        }
    }
}
