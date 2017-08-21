using System;
using Android.Support.V4.View;
using Qoden.Binding;
using Qoden.Util;

namespace Qoden.UI
{
    public static class ViewPagerBindings
    {
        public static BindingList Adapter<T>(this BindingList bindings, ViewPager viewPager, T adapter)
            where T : PagerAdapter
        {
            viewPager.Adapter = adapter;
            if (adapter is IBinding)
            {
                bindings.Add((IBinding)adapter);
            }
            return bindings;
        }

        static readonly RuntimeEvent _PageSelectedEvent = new RuntimeEvent(typeof(ViewPager), "PageSelected");
        public static RuntimeEvent PageSelectedEvent
        {
            get 
            {
                if (LinkerTrick.False)
                {
                    new ViewPager(null).PageSelected += (o, a) => { };
                }
                return _PageSelectedEvent;
            }
        }

        public static readonly IPropertyBindingStrategy PageSelectedEventBinding = new EventHandlerBindingStrategy<ViewPager.PageSelectedEventArgs>(PageSelectedEvent);

        public static IProperty<int> SelectedPagePositionProperty(this ViewPager viewPager)
        {
            return viewPager.GetProperty(_ => _.CurrentItem, PageSelectedEventBinding);
        }
    }
}
