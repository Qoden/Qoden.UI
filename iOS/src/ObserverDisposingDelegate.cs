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

        public void DisposeObserver()
        {
            DisposableObserver?.Dispose();
            DisposableObserver = null;
        }

        [Export("actionForLayer:forKey:")]
        public override Foundation.NSObject ActionForLayer(CALayer layer, string eventKey)
        {
            if(eventKey.Equals("onOrderOut")) 
            {
                DisposeObserver();
            }
            return null;
        }
    }
}
