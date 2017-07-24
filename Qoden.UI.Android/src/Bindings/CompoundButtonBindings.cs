using System;
using Android.Widget;
using Qoden.Binding;
using Qoden.Reflection;

namespace Qoden.UI
{
    public static class CompoundButtonBindings
    {
        static readonly RuntimeEvent _CheckedChangeEvent = new RuntimeEvent(typeof(CompoundButton), "CheckedChange");
        public static RuntimeEvent CheckedChangeEvent
        {
            get
            {
                if (LinkerTrick.False)
                {
                    CompoundButton btn = null;
                    btn.CheckedChange += (sender, args) => { };
                }
                return _CheckedChangeEvent;
            }
        }

        public static readonly IPropertyBindingStrategy CheckedChangeBinding = new EventHandlerBindingStrategy<CompoundButton.CheckedChangeEventArgs>(CheckedChangeEvent);

        public static IProperty<bool> CheckedProperty(this IQView<CompoundButton> view)
        {
            return view.PlatformView.CheckedProperty();
        }

        public static IProperty<bool> CheckedProperty(this CompoundButton view)
        {
            return view.GetProperty(_ => _.Checked, CheckedChangeBinding);
        }
    }
}
