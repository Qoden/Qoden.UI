using System;
using CoreAnimation;
using Foundation;

namespace Qoden.UI
{
    public class ObserverDisposingDelegate : CALayerDelegate
    {
        IDisposable DisposableObserver { get; set; }

        public ObserverDisposingDelegate(IDisposable disposableObserver)
        {
            DisposableObserver = disposableObserver;
        }

        [Export("actionForLayer:forKey:")]
        public override Foundation.NSObject ActionForLayer(CALayer layer, string eventKey)
        {
            if(eventKey.Equals("onOrderOut")) 
            {
                DisposableObserver?.Dispose();
            }
            return null;
        }
    }
}
