using System;
using UIKit;
using Foundation;
using CoreGraphics;
using Qoden.Reflection;
using Qoden.Validation;
using Qoden.Binding;

namespace Qoden.UI.iOS
{
    public static class UIViewProperties
    {
        public static IProperty<bool> HiddenProperty(this UIView view)
        {
            return view.GetProperty(_ => _.Hidden, KVCBindingStrategy.Instance);
        }
    }

    public static class UIControlEvents
    {
        public static readonly RuntimeEvent ValueChangedEvent = new RuntimeEvent(typeof(UIControl), "ValueChanged");
        public static readonly RuntimeEvent TouchUpInsideEvent = new RuntimeEvent(typeof(UIControl), "TouchUpInside");

        [Preserve]
        static void LinkerTrick()
        {
            new UIControl().ValueChanged += (o, a) => { };
            new UIControl().TouchUpInside += (o, a) => { };
        }
    }

    public static class UIControlProperties
    {
        public static readonly IPropertyBindingStrategy ValueBinding = new EventHandlerBindingStrategy(UIControlEvents.ValueChangedEvent);

        public static IProperty<bool> EnabledProperty(this UIControl control)
        {
            return control.GetProperty(_ => _.Enabled, KVCBindingStrategy.Instance);
        }

        public static EventHandlerSource<T> ClickTarget<T>(this T control)
            where T : UIControl
        {
            return new EventHandlerSource<T>(UIControlEvents.TouchUpInsideEvent, control)
            {
                SetEnabledAction = SetControlEnabled,
                ParameterExtractor = (object sender, EventArgs eventArgs) => sender
            };
        }

        static void SetControlEnabled(UIControl control, bool enabled)
        {
            control.Enabled = enabled;
        }
    }

    public static class UIBarButtonItemProperties
    {
        public static readonly RuntimeEvent ClickedEvent = new RuntimeEvent(typeof(UIBarButtonItem), "Clicked");

        public static EventHandlerSource<T> ClickedTarget<T>(this T control)
            where T : UIBarButtonItem
        {
            return new EventHandlerSource<T>(ClickedEvent, control)
            {
                SetEnabledAction = SetControlEnabled,
                ParameterExtractor = (object sender, EventArgs eventArgs) => sender
            };
        }

        static void SetControlEnabled(UIBarButtonItem control, bool enabled)
        {
            control.Enabled = enabled;
        }
    }

    public static class UILabelProperties
    {
        public static IProperty<string> TextProperty(this UILabel label)
        {
            return label.GetProperty(_ => _.Text, KVCBindingStrategy.Instance);
        }
    }

    public static class UITextFieldProperties
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

        public static readonly RuntimeEvent EditingDidEndEvent = new RuntimeEvent(typeof(UITextField), "EditingDidEnd");
        public static readonly IPropertyBindingStrategy EditingDidEndBinding = new EventHandlerBindingStrategy(EditingDidEndEvent);
        public static readonly IPropertyBindingStrategy TextChangedBinding = new TextChangedBindingStrategy();

        public static IProperty<string> TextProperty(this UITextField textField, bool immediate = true)
        {
            if (immediate)
            {
                return textField.GetProperty(_ => _.Text, TextChangedBinding);
            }
            else {
                return textField.GetProperty(_ => _.Text, EditingDidEndBinding);
            }
        }

        [Preserve]
        static void LinkerTrick()
        {
            new UITextField().EditingDidEnd += (o, a) => { };
        }
    }

    public static class UISegmentedControlProperties
    {
        class SelectedSegment : Property<int>
        {
            public SelectedSegment(UISegmentedControl owner)
                : base(owner, "SelectedSegment", UIControlProperties.ValueBinding)
            {
            }

            protected override void SetValue(int value)
            {
                ((UISegmentedControl)Owner).SelectedSegment = value;
            }

            protected override int GetValue()
            {
                return (int)((UISegmentedControl)Owner).SelectedSegment;
            }
        }

        public static IProperty<int> SelectedSegmentProperty(this UISegmentedControl segments)
        {
            return new SelectedSegment(segments);
        }
    }

    public static class UITextViewProperties
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

        public static readonly RuntimeEvent EditingEndedEvent = new RuntimeEvent(typeof(UITextView), "Ended");
        public static readonly IPropertyBindingStrategy EditingDidEndBinding = new EventHandlerBindingStrategy(EditingEndedEvent);
        public static readonly IPropertyBindingStrategy TextChangedBinding = new TextChangedBindingStrategy();

        public static IProperty<string> TextProperty(this UITextView textView, bool immediate = true)
        {
            if (immediate)
            {
                return textView.GetProperty(_ => _.Text, TextChangedBinding);
            }
            else {
                return textView.GetProperty(_ => _.Text, EditingDidEndBinding);
            }
        }

        public static IProperty<CGSize> ContentSizeProperty(this UITextView textView)
        {
            return textView.GetProperty(_ => _.ContentSize, KVCBindingStrategy.Instance);
        }

        [Preserve]
        static void LinkerTrick()
        {
            new UITextView().Ended += (sender, e) => { };
        }
    }

    public static class RemoteImageViewProperties
    {
        public static readonly RuntimeEvent ImageChangedEvent = new RuntimeEvent(typeof(RemoteImageView), "ImageChanged");
        public static readonly IPropertyBindingStrategy ImageChangedBinding = new EventHandlerBindingStrategy(ImageChangedEvent);

        public static IProperty<RemoteImage> ImageProperty(this RemoteImageView imageView)
        {
            return imageView.GetProperty(_ => _.Image, ImageChangedBinding);
        }

        [Preserve]
        static void LinkerTrick()
        {
            new RemoteImageView().ImageChanged += (o, a) => { };
        }
    }

    public static class UIActivityIndicatorViewProperties
    {
        public static IProperty<bool> IsAnimatingProperty(this UIActivityIndicatorView indicator)
        {
            return indicator.GetProperty(_ => _.IsAnimating, KVCBindingStrategy.Instance);
        }
    }

}

