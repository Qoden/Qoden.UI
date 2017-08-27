using System;
using Qoden.Binding;
using Qoden.Util;
using UIKit;

namespace Qoden.UI
{
    public static class UIBarButtonItemBindings
    {
        private static readonly RuntimeEvent _clickedEvent = new RuntimeEvent(
            typeof(UIBarButtonItem), 
            nameof(UIBarButtonItem.Clicked));
        public static RuntimeEvent ClickedEvent
        {
            get
            {
                if (LinkerTrick.False)
                {
                    new UIBarButtonItem().Clicked += (o, a) => { };
                }
                return _clickedEvent;
            }
        }

        public static EventCommandTrigger ClickedTarget<T>(this T control)
            where T : UIBarButtonItem
        {
            return new EventCommandTrigger(ClickedEvent, control)
            {
                SetEnabledAction = SetControlEnabled,
                ParameterExtractor = (object sender, EventArgs eventArgs) => sender
            };
        }

        static void SetControlEnabled(object control, bool enabled)
        {
            ((UIControl)control).Enabled = enabled;
        }
    }
}
