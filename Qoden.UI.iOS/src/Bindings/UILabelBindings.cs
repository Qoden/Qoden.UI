using UIKit;
using Qoden.Binding;

namespace Qoden.UI
{
    public static class UILabelBindings
    {
        public static IProperty<string> TextProperty(this IQView<UILabel> label)
        {
            return label.PlatformView.TextProperty();
        }

        public static IProperty<string> TextProperty(this UILabel label)
        {
            return label.GetProperty(_ => _.Text, KVCBindingStrategy.Instance);
        }
    }
}

