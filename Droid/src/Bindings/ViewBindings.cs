using System;
using Android.Views;
using Qoden.Binding;
using Qoden.Util;

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
                    new Android.Views.View(null).Click += (o, a) => { };
                }
                return _ClickEvent;
            }
        }

        public static EventHandlerSource<T> ClickTarget<T>(this T view)
            where T : Android.Views.View
        {
            return new EventHandlerSource<T>(ClickEvent, view)
            {
                SetEnabledAction = SetViewEnabled
            };
        }

        static void SetViewEnabled(Android.Views.View view, bool enabled)
        {
            view.Enabled = enabled;
        }
    }
}
