using System;
using Foundation;
using Qoden.Binding;
using UIKit;

namespace Qoden.UI
{
    public static class UISwitchBindings
    {
        public static IProperty<bool> CheckedProperty(this UISwitch label)
        {
            return label.GetProperty(_ => _.On, SwitchCheckedBindingStrategy.Instance);
        }
    }
    
    public class SwitchCheckedBindingStrategy : IPropertyBindingStrategy
    {
        public static readonly SwitchCheckedBindingStrategy Instance = new SwitchCheckedBindingStrategy();
        public IDisposable SubscribeToPropertyChange(IProperty property, Action<IProperty> action)
        {
            var tf = property.Owner as UISwitch;
            tf.ValueChanged += (sender, eventArgs) => action(property);
            return tf.AddObserver("on", NSKeyValueObservingOptions.OldNew, change =>
            {
                action(property);
            });
        }
    }
}