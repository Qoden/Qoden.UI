using System;
using CoreAnimation;
using Foundation;

namespace Qoden.UI
{
    public class CALayerAutoResizeSublayersDelegate : CALayerDelegate
    {
        // [Export("actionForLayer:forKey:")]
        public override NSObject ActionForLayer(CALayer layer, string eventKey)
        {
            // hack / N.Shalin 11.04.18
            // Need to find out how to automatically resize layer to its superlayer size
            // this method is not stable. Possible side-effects!
            if (eventKey.Equals("onOrderIn"))
            {
                layer.Frame = layer.SuperLayer.Frame;
            }
            return null;
        }
	}
}
