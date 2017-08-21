using System;
using Qoden.Binding;
using Qoden.Util;
using UIKit;

namespace Qoden.UI
{
    public static class UIControlBindings
    {
        static readonly RuntimeEvent _ValueChangedEvent = new RuntimeEvent(typeof(UIControl), "ValueChanged");
        public static RuntimeEvent ValueChangedEvent
        {
            get
            {
                if (LinkerTrick.False)
                {
                    new UIControl().ValueChanged += (o, a) => { };
                }
                return _ValueChangedEvent;
            }
        }

        static readonly RuntimeEvent _TouchUpInsideEvent = new RuntimeEvent(typeof(UIControl), "TouchUpInside");
        public static RuntimeEvent TouchUpInsideEvent
        {
            get
            {
                if (LinkerTrick.False)
                {
                    new UIControl().TouchUpInside += (o, a) => { };
                }
                return _TouchUpInsideEvent;
            }
        }

        public static readonly IPropertyBindingStrategy ValueBinding = new EventHandlerBindingStrategy(ValueChangedEvent);

        public static IProperty<bool> EnabledProperty(this UIControl control)
        {
            return control.GetProperty(_ => _.Enabled, KVCBindingStrategy.Instance);
        }

        public static EventHandlerSource ClickTarget(this UIControl control)
        {
            return new EventHandlerSource(TouchUpInsideEvent, control)
            {
                SetEnabledAction = SetControlEnabled
            };
        }

        static void SetControlEnabled(object control, bool enabled)
        {
            ((UIControl)control).Enabled = enabled;
        }
    }
}
