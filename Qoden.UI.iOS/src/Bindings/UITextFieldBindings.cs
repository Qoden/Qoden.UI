using System;
using Foundation;
using Qoden.Binding;
using Qoden.Reflection;
using Qoden.Validation;
using UIKit;

namespace Qoden.UI
{
    public static class UITextFieldBindings
    {
        class TextChangedBindingStrategy : NSObject, IPropertyBindingStrategy
        {
            internal class TextFieldTextSubscription : IDisposable
            {
                public NSNotificationCenterSubscription NotificationSubscription;
                public IDisposable KVCObserver;

                public void Dispose()
                {
                    if (KVCObserver != null)
                    {
                        NotificationSubscription.Dispose();
                        KVCObserver.Dispose();
                        KVCObserver = null;
                    }
                }
            }
            //IMPORTANT NOTE:
            //UITextField.TextFieldTextDidChangeNotification is fired when user change text AND if UITextField.ShouldChangeCharacters
            //return true.
            //KVC observer is fired when 'text' property is changed programmaticaly.
            //To handle all cases you have to subscribe to both notification and KVC.
            public IDisposable SubscribeToPropertyChange(IProperty property, Action<IProperty> action)
            {
                Assert.Argument(property, "property").NotNull();
                Assert.Argument(action, "action").NotNull();

                var tf = property.Owner as UITextField;
                var kvc = tf.AddObserver("text", NSKeyValueObservingOptions.OldNew, change =>
                {
                    if (!change.NewValue.IsEqual(change.OldValue))
                    {
                        action(property);
                    }
                });
                var center = NSNotificationCenter.DefaultCenter;
                var token = center.AddObserver(
                                UITextField.TextFieldTextDidChangeNotification,
                                _ => action(property),
                                (NSObject)property.Owner);
                var notification = new NSNotificationCenterSubscription(NSNotificationCenter.DefaultCenter, token);
                return new TextFieldTextSubscription
                {
                    NotificationSubscription = notification,
                    KVCObserver = kvc
                };
            }
        }

        static readonly RuntimeEvent _EditingDidEndEvent = new RuntimeEvent(typeof(UITextField), "EditingDidEnd");
        public static RuntimeEvent EditingDidEndEvent
        {
            get
            {
                if (LinkerTrick.False)
                {
                    new UITextField().EditingDidEnd += (sender, e) => {};
                }
                return _EditingDidEndEvent;
            }
        }

        public static readonly IPropertyBindingStrategy EditingDidEndBinding = new EventHandlerBindingStrategy(EditingDidEndEvent);
        public static readonly IPropertyBindingStrategy TextChangedBinding = new TextChangedBindingStrategy();

        public static IProperty<string> TextProperty(this IQView<UITextField> textField, bool immediate = true)
        {
            return textField.PlatformView.TextProperty(immediate);
        }

        public static IProperty<string> TextProperty(this UITextField textField, bool immediate = true)
        {
            if (immediate)
            {
                return textField.GetProperty(_ => _.Text, TextChangedBinding);
            }
            else
            {
                return textField.GetProperty(_ => _.Text, EditingDidEndBinding);
            }
        }
    }
}
