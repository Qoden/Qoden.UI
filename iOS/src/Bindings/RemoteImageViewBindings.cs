using System;
using Qoden.Binding;
using Qoden.Util;

namespace Qoden.UI
{
    public static class RemoteImageViewBindings
    {
        static readonly RuntimeEvent _ImageChangedEvent = new RuntimeEvent(typeof(RemoteImageView), "ImageChanged");
        public static RuntimeEvent ImageChangedEvent
        {
            get
            {
                if (LinkerTrick.False)
                {
                    new RemoteImageView().ImageChanged += (sender, e) => { };
                }
                return _ImageChangedEvent;
            }
        }

        public static readonly IPropertyBindingStrategy ImageChangedBinding = new EventHandlerBindingStrategy(ImageChangedEvent);

        public static IProperty<RemoteImage> ImageProperty(this RemoteImageView imageView)
        {
            return imageView.GetProperty(_ => _.Image, ImageChangedBinding);
        }
    }
}
