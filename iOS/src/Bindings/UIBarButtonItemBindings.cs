using System;
using Qoden.Binding;
using Qoden.Util;
using UIKit;

namespace Qoden.UI
{
    public static class UIBarButtonItemBindings
    {
        static readonly RuntimeEvent _ClickedEvent = new RuntimeEvent(typeof(UIBarButtonItem), "Clicked");
        public static RuntimeEvent ClickedEvent
        {
            get
            {
                if (LinkerTrick.False)
                {
                    new UIBarButtonItem().Clicked += (o, a) => { };
                }
                return _ClickedEvent;
            }
        }

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
}
