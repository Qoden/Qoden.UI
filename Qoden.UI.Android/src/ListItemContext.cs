using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;

namespace Qoden.UI
{
    public class ListItemContext<T>
    {
        public int Position { get; internal set; }
        public Android.Views.View View { get; internal set; }
        public Android.Views.ViewGroup Parent { get; internal set; }
        public IList<T> DataSource { get; internal set; }
        public T Item => DataSource[Position];
        public BaseAdapter<T> Adapter { get; internal set; }

        public Android.Views.View Result { get; set; }

        public TView CreateView<TView>() where TView : Android.Views.View
        {
            return (TView)Activator.CreateInstance(typeof(TView), Parent.Context);
        }
    }
}
