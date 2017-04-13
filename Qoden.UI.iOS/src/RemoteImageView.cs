using UIKit;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Qoden.UI.iOS
{
	public class RemoteImageView : QodenView
	{
		[View]
		public UIImageView ImageView { get; private set; }

		public UIActivityIndicatorView Indicator { get; private set; }

		public override void LayoutSubviews()
		{
			ImageView.Frame = this.LayoutBox()
				.Top(0).Bottom(0).Left(0).Right(0).AsCGRect();
			if (Indicator != null)
			{
				Indicator.SizeToFit();
				Indicator.Frame = this.LayoutBox()
				.CenterVertically().CenterHorizontally().Width(Indicator).Height(Indicator).AsCGRect();
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
					else {
						Indicator.RemoveFromSuperview();
						Indicator.Dispose();
						Indicator = null;
					}
				}
			}
		}

		public event EventHandler ImageChanged;

		RemoteImage image;
		public CancellationTokenSource CancelTokenSource = new CancellationTokenSource();

		public RemoteImage Image
		{
			get { return image; }
			set
			{
				if (value != image)
				{
					CancelTokenSource.Cancel();
					CancelTokenSource = new CancellationTokenSource();
					SetRemoteImage(value, CancelTokenSource.Token)
						.ContinueWith(t =>
						{
							if (t.Exception != null)
							{
								ImageView.Image = placeholder;
							}
							image = value;
							ImageChanged?.Invoke(this, EventArgs.Empty);
						});
				}
			}
		}

		UIImage placeholder;

		public UIImage Placeholder
		{
			get { return placeholder; }
			set
			{
				placeholder = value;
				if (image == null || !image.IsLoaded)
				{
					ImageView.Image = placeholder;
				}
			}
		}

		public async Task SetRemoteImage(RemoteImage image, CancellationToken token)
		{
			if (image == null)
				throw new ArgumentNullException();
			if (this.image == image)
			{
				return;
			}
			this.image = image;
			if (!this.image.IsLoaded)
			{
				try
				{
					ImageView.Image = placeholder;
					if (UseIndicator)
						Indicator.StartAnimating();
					await image.Load(token);
				}
				catch (Exception e)
				{
					ImageView.Image = placeholder;
					if (e is TaskCanceledException) return;
					throw;
				}
			}
			if (image.Bitmap.Native == null)
			{
				ImageView.Image = placeholder;
			}
			else
			{
				ImageView.Image = image.Bitmap.UIImage();
			}
			if (UseIndicator)
				Indicator.StopAnimating();
		}
	}
}

