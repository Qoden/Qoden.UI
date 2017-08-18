using UIKit;
using Qoden.Binding;

namespace Qoden.UI
{
    public static class UIViewBindings
    {
        public static IProperty<bool> HiddenProperty(this UIView view)
        {
            return view.GetProperty(_ => _.Hidden, KVCBindingStrategy.Instance);
        }
    }
}

