using System;
using Android.Views;
using Qoden.Binding;
using Qoden.Reflection;

namespace Qoden.UI
{
    public static class ViewBindings
    {
        static readonly RuntimeEvent _ClickEvent = new RuntimeEvent(typeof(View), "Click");
        public static RuntimeEvent ClickEvent
        {
            get
            {
                if (LinkerTrick.False)
                {
                    new View(null).Click += (o, a) => { };
                }
                return _ClickEvent;
            }
        }

        public static EventHandlerSource<T> ClickTarget<T>(this T view)
            where T : View
        {
            return new EventHandlerSource<T>(ClickEvent, view)
            {
                SetEnabledAction = SetViewEnabled
            };
        }

        static void SetViewEnabled(View view, bool enabled)
        {
            view.Enabled = enabled;
        }
    }
}
