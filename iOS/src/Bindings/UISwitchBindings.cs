using System;
using Qoden.Binding;
using UIKit;

namespace Qoden.UI
{
    public static class UISwitchBindings
    {
        public static IProperty<bool> CheckedProperty(this UISwitch label)
        {
            return label.GetProperty(_ => _.On, KVCBindingStrategy.Instance);
        }
    }
}
