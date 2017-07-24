using UIKit;
using Qoden.Binding;

namespace Qoden.UI
{
    public static class UIActivityIndicatorViewBindings
    {
        public static IProperty<bool> IsAnimatingProperty(this IQView<UIActivityIndicatorView> indicator)
        {
            return indicator.PlatformView.IsAnimatingProperty();
        }

        public static IProperty<bool> IsAnimatingProperty(this UIActivityIndicatorView indicator)
        {
            return indicator.GetProperty(_ => _.IsAnimating, KVCBindingStrategy.Instance);
        }
    }

}

