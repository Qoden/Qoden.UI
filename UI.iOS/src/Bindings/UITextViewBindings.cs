using System;
using CoreGraphics;
using Foundation;
using Qoden.Binding;
using Qoden.Reflection;
using Qoden.Validation;
using UIKit;

namespace Qoden.UI
{
    public static class UITextViewBindings
    {
        class TextChangedBindingStrategy : NSObject, IPropertyBindingStrategy
        {
            internal class TextViewTextSubscription : IDisposable
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

            public IDisposable SubscribeToPropertyChange(IProperty property, Action<IProperty> action)
            {
                Assert.Argument(property, "property").NotNull();
                Assert.Argument(action, "action").NotNull();

                var tf = property.Owner as UITextView;
                var kvc = tf.AddObserver("text", NSKeyValueObservingOptions.OldNew, change =>
                {
                    if (!change.NewValue.IsEqual(change.OldValue))
                    {
                        action(property);
                    }
                });
                var center = NSNotificationCenter.DefaultCenter;
                var token = center.AddObserver(
                                UITextView.TextDidChangeNotification,
                                _ => action(property),
                                (NSObject)property.Owner);
                var notification = new NSNotificationCenterSubscription(NSNotificationCenter.DefaultCenter, token);
                return new TextViewTextSubscription
                {
                    NotificationSubscription = notification,
                    KVCObserver = kvc
                };
            }
        }

        static readonly RuntimeEvent _EditingEndedEvent = new RuntimeEvent(typeof(UITextView), "Ended");
        public static RuntimeEvent EditingEndedEvent
        {
            get
            {
                if (LinkerTrick.False)
                {
                    new UITextView().Ended += (sender, e) => { };
                }
                return _EditingEndedEvent;
            }
        }

        public static readonly IPropertyBindingStrategy EditingDidEndBinding = new EventHandlerBindingStrategy(EditingEndedEvent);
        public static readonly IPropertyBindingStrategy TextChangedBinding = new TextChangedBindingStrategy();

        public static IProperty<string> TextProperty(this UITextView textView, bool immediate = true)
        {
            if (immediate)
            {
                return textView.GetProperty(_ => _.Text, TextChangedBinding);
            }
            else
            {
                return textView.GetProperty(_ => _.Text, EditingDidEndBinding);
            }
        }

        public static IProperty<CGSize> ContentSizeProperty(this UITextView textView)
        {
            return textView.GetProperty(_ => _.ContentSize, KVCBindingStrategy.Instance);
        }
    }
}
