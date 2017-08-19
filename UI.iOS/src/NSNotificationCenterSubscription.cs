using System;
using UIKit;
using Foundation;
using Qoden.Validation;

namespace Qoden.UI
{
	public class NSNotificationCenterSubscription : IDisposable
	{
		NSObject subscription;
		readonly NSNotificationCenter center;

		public NSNotificationCenterSubscription (NSNotificationCenter center, NSObject subscription)
		{
			Assert.Argument (center, "center").NotNull ();
			Assert.Argument (subscription, "subscription").NotNull ();
			this.subscription = subscription;
			this.center = center;
		}

		public void Dispose ()
		{
			if (subscription != null) {
				center.RemoveObserver (subscription);
				subscription = null;
			}
		}
	}
	
}
