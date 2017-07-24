using System;
using Android.Widget;
using Qoden.Binding;
using Qoden.Reflection;

namespace Qoden.UI
{
    public static class AdapterViewBindings
    {
        public static BindingList Adapter<T>(this BindingList bindings, IQView<AdapterView<T>> view, T adapter)
            where T : IAdapter, IBinding
        {
            return bindings.Adapter(view.PlatformView, adapter);
        }

        /// <summary>
        /// Set adapter into view and then add it as a binding if it support IBinding interface. Usually this is done 
        /// to make adapter subscribe/unsubscribe to model together will all other bindings.
        /// </summary>
        /// <param name="bindings">Binding list</param>
        /// <param name="view">Adapter owner view</param>
        /// <param name="adapter">Adapter instance</param>
        /// <typeparam name="T">Adapter type</typeparam>
        public static BindingList Adapter<T>(this BindingList bindings, AdapterView<T> view, T adapter)
            where T : IAdapter, IBinding
        {
            view.Adapter = adapter;
            bindings.Add(adapter);
            return bindings;
        }

        private static readonly RuntimeEvent _ItemSelectedEvent = new RuntimeEvent(typeof(AdapterView), "ItemSelected");
        public static RuntimeEvent ItemSelectedEvent
        {
            get 
            {
                if (LinkerTrick.False)
                {
                    AdapterView adapter = null;
                    adapter.ItemSelected += (o, a) => { };
                }
                return _ItemSelectedEvent;
            }
        }

        public static readonly IPropertyBindingStrategy ItemSelectedEventBinding = new EventHandlerBindingStrategy<AdapterView.ItemSelectedEventArgs>(ItemSelectedEvent);

        public static EventHandlerSource<T> ItemSelectedTarget<T>(this IQView<T> view)
            where T : AdapterView
        {
            return view.PlatformView.ItemSelectedTarget();
        }

        public static EventHandlerSource<T> ItemSelectedTarget<T>(this T view)
            where T : AdapterView
        {
            return new EventHandlerSource<T>(ItemSelectedEvent, view)
            {
                SetEnabledAction = SetViewEnabled,
                ParameterExtractor = (sender, args) => ((AdapterView.ItemSelectedEventArgs)args).Position
            };
        }

        static readonly RuntimeEvent _ItemClickEvent = new RuntimeEvent(typeof(AdapterView), "ItemClick");
        public static RuntimeEvent ItemClickEvent
        {
            get
            {
                if (LinkerTrick.False)
                {
                    AdapterView adapter = null;
                    adapter.ItemClick += (o, a) => { };
                }
                return _ItemClickEvent;
            }
        }

        public static EventHandlerSource<T> ItemClickTarget<T>(this IQView<T> view)
            where T : AdapterView
        {
            return view.PlatformView.ItemClickTarget();
        }
        
        public static EventHandlerSource<T> ItemClickTarget<T>(this T view)
            where T : AdapterView
        {
            return new EventHandlerSource<T>(ItemClickEvent, view)
            {
                SetEnabledAction = SetViewEnabled,
                ParameterExtractor = (sender, args) => ((AdapterView.ItemClickEventArgs)args).Position
            };
        }

        static void SetViewEnabled(AdapterView view, bool enabled)
        {
            view.Enabled = enabled;
        }

        public static AdapterBinding<T> Adapter<T>(this IQView<AdapterView<T>> view) where T : IAdapter
        {
            return view.PlatformView.Adapter();
        }

        public static AdapterBinding<T> Adapter<T>(this AdapterView<T> view) where T : IAdapter
        {
            return new AdapterBinding<T>(view);
        }

        /// <summary>
        /// Helper class to make Adapter related properties bindings.
        /// </summary>
        public class AdapterBinding<T> where T : IAdapter
        {
            public AdapterBinding(AdapterView<T> view)
            {
                View = view;
            }

            public AdapterView<T> View { get; set; }

            /// <summary>
            /// Binding for SelectedItemProperty. It listen to 'ItemSelected' event, extracts
            /// selected object from Adapter and update source.
            /// NOTE: this logic requires Adapter to be subtype of Xamarin BaseAdapter. Otherwise there is no easy 
            /// way to get .NET object from Java Adapter.
            /// </summary>
            /// <returns>Property ready for binding</returns>
            /// <typeparam name="U">Type of adapter contents</typeparam>
            public IProperty<U> SelectedItemProperty<U>()
            {
                var property = View.GetProperty(_ => _.SelectedItemPosition, ItemSelectedEventBinding, setter: (view, pos) => view.SetSelection(pos));
                Func<U, int> itemToPos = null;
                var reverseAdapter = View.Adapter as IReverseAdapter<U>;
                if (reverseAdapter != null)
                {
                    itemToPos = (item) =>
                    {
                        return reverseAdapter.GetPosition(item);
                    };
                }
                return property.Convert(PositionToItem<U>, itemToPos);
            }

            U PositionToItem<U>(int pos)
            {
                var adapter = View.Adapter as BaseAdapter<U>;
                if (adapter == null)
                {
                    throw new InvalidOperationException("This type of Adapter is not supported. Use adapter derived from BaseAdapter<T> or use custom binding");
                }
                return adapter[pos];
            }
        }
    }
}
