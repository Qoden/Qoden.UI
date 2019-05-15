using UIKit;
using System;
using System.Threading.Tasks;
using System.Threading;
using Qoden.UI.Wrappers;

namespace Qoden.UI
{
    public class RemoteImageView : QodenView, IPlatformRemoteImageView
    {
        RemoteImageViewModel _model;
        public UIImageView ImageView { get; private set; }
        public UIActivityIndicatorView Indicator { get; private set; }

        public RemoteImageView()
        {
            ImageView = Builder.ImageView();
            _model = new RemoteImageViewModel(this);
        }

        protected override void OnLayout(LayoutBuilder layout)
        {
            layout.View(ImageView)
                     .Top(0).Bottom(0).Left(0).Right(0);
            if (Indicator != null)
            {
                layout.View(Indicator)
                         .CenterVertically()
                         .CenterHorizontally()
                         .AutoSize();
            }
        }

        public bool UseIndicator
        {
            get { return Indicator != null; }
            set
            {
                if (value != UseIndicator)
                {
                    if (value)
                    {
                        Indicator = new UIActivityIndicatorView();
                        AddSubview(Indicator);
                    }
                    else
                    {
                        Indicator.RemoveFromSuperview();
                        Indicator.Dispose();
                        Indicator = null;
                    }
                }
            }
        }

        public event EventHandler ImageChanged;


        public RemoteImage Image
        {
            get { return _model.Image; }
            set
            {
                _model.Image = value;
            }
        }

        public UIImage Placeholder
        {
            get { return _model.Placeholder; }
            set
            {
                _model.Placeholder = value;
            }
        }

        public async Task SetRemoteImage(RemoteImage image, CancellationToken token)
        {
            await _model.SetRemoteImage(image, token);
        }

        void IPlatformRemoteImageView.SetImage(UIImage image)
        {
            InvokeOnMainThread(() => ImageView.Image = image);
        }

        void IPlatformRemoteImageView.OnFireImageChanged()
        {
            ImageChanged?.Invoke(this, EventArgs.Empty);
        }

        void IPlatformRemoteImageView.OnLoadingStarted()
        {
            if (UseIndicator)
                Indicator.StartAnimating();
        }

        void IPlatformRemoteImageView.OnLoadingFinished()
        {
            if (UseIndicator)
                Indicator.StopAnimating();
        }
    }
}

