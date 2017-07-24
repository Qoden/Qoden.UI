using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Android.Views;
using Android.Widget;
using Qoden.Binding;
using Qoden.Validation;
using AndroidView = Android.Views.View;
using AndroidViewGroup = Android.Views.ViewGroup;

namespace Qoden.UI
{
    /// <summary>
    /// A <see cref="BaseAdapter{T}"/> that can be used with an Android ListView. After setting
    /// the <see cref="DataSource"/> and the <see cref="ViewFactory"/> properties, the adapter is
    /// suitable for a list control. If the DataSource is an <see cref="INotifyCollectionChanged"/>,
    /// changes to the collection will be observed and the UI will automatically be updated.
    /// </summary>
    /// <typeparam name="T">The type of the items contained in the <see cref="DataSource"/>.</typeparam>
    ////[ClassInfo(typeof(ObservableAdapter<T>),
    ////    VersionString = "5.1.1",
    ////    DateString = "201502072030",
    ////    UrlContacts = "http://www.galasoft.ch/contact_en.html",
    ////    Email = "laurent@galasoft.ch")]
    public class PlainListBinding<T> : BaseAdapter<T>, IBinding, IReverseAdapter<T>
    {
        IList<T> _list;
        INotifyCollectionChanged _notifier;

        /// <summary>
        /// Gets the number of items in the DataSource.
        /// </summary>
        public override int Count
        {
            get
            {
                var count = _list == null ? 0 : _list.Count;
                return count;
            }
        }

        /// <summary>
        /// Gets or sets the list containing the items to be represented in the list control.
        /// </summary>
        public IList<T> DataSource
        {
            get { return _list; }
            set
            {
                if (Bound) throw new InvalidOperationException("Cannot change DataSource when adapter is bound.");

                if (Equals(_list, value))
                {
                    return;
                }

                _list = value;
                _notifier = _list as INotifyCollectionChanged;
            }
        }

        /// <summary>
        /// Gets and sets a method taking an item's position in the list, the item itself,
        /// and a recycled Android View, and returning an adapted View for this item. Note that the recycled
        /// view might be null, in which case a new View must be inflated by this method.
        /// </summary>
        public Action<ListItemContext<T>> ViewFactory { get; set; }

        public Action<ListItemContext<T>> DropDownViewFactory { get; set; }

        public bool Enabled { get; set; }

        public bool Bound { get; private set; }

        /// <summary>
        /// Gets the item corresponding to the index in the DataSource.
        /// </summary>
        /// <param name="position">The index of the item that needs to be returned.</param>
        /// <returns>The item corresponding to the index in the DataSource</returns>
        public override T this[int position]
        {
            get { return _list == null ? default(T) : _list[position]; }
        }

        /// <summary>
        /// Returns a unique ID for the item corresponding to the position parameter.
        /// In this implementation, the method always returns the position itself.
        /// </summary>
        /// <param name="position">The position of the item for which the ID needs to be returned.</param>
        /// <returns>A unique ID for the item corresponding to the position parameter.</returns>
        public override long GetItemId(int position)
        {
            return position;
        }

        /// <summary>
        /// Returns position for the item.
        /// </summary>
        /// <param name="item">The item for which the position needs to be returned.</param>
        /// <returns>Position for the item corresponding to the position parameter or -1 if the item was not found in the list.</returns>
        public int GetPosition(T item)
        {
            return _list == null ? -1 : _list.IndexOf(item);
        }

        /// <summary>
        /// Prepares the view (template) for the item corresponding to the position
        /// in the DataSource. This method calls the <see cref="ViewFactory"/> method so that the caller
        /// can create (if necessary) and adapt the template for the corresponding item.
        /// </summary>
        /// <param name="position">The position of the item in the DataSource.</param>
        /// <param name="convertView">A recycled view. If this parameter is null,
        /// a new view must be inflated.</param>
        /// <param name="parent">The view's parent.</param>
        /// <returns>A view adapted for the item at the corresponding position.</returns>
        public override AndroidView GetView(int position, AndroidView convertView, AndroidViewGroup parent)
        {
            if (ViewFactory == null)
            {
                return convertView;
            }
            var listItemContext = new ListItemContext<T>
            {
                Position = position,
                View = convertView,
                Parent = parent,
                Adapter = this,
                DataSource = _list
            };
            ViewFactory(listItemContext);
            return listItemContext.Result;
        }

        public override AndroidView GetDropDownView(int position, AndroidView convertView, AndroidViewGroup parent)
        {
            if (DropDownViewFactory == null)
            {
                return convertView;
            }
            var listItemContext = new ListItemContext<T>()
            {
                Position = position,
                View = convertView,
                Parent = parent,
                Adapter = this,
                DataSource = _list
            };
            DropDownViewFactory(listItemContext);
            return listItemContext.Result;
        }

        public Func<int> ListItemTypeCount { get; set; } = () => 1;

        public override int ViewTypeCount
        {
            get { return ListItemTypeCount(); }
        }

        public Func<int, int> ItemViewType { get; set; } = p => 0;

        public override int GetItemViewType(int position)
        {
            return ItemViewType(position);
        }

        void NotifierCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Enabled)
                NotifyDataSetChanged();
        }

        public void Bind()
        {
            if (Bound)
                return;
            Assert.State(DataSource).NotNull("DataSource is not set");

            if (_notifier != null)
            {
                _notifier.CollectionChanged += NotifierCollectionChanged;
            }
            Bound = true;
        }

        public void Unbind()
        {
            if (!Bound)
                return;
            if (_notifier != null)
            {
                _notifier.CollectionChanged -= NotifierCollectionChanged;
            }
            Bound = false;
        }

        public void UpdateTarget()
        {
            NotifyDataSetChanged();
        }

        public void UpdateSource()
        {
            throw new NotSupportedException();
        }
    }

    public static class ObservableCollectionAdapterExtension
    {
        /// <summary>
        /// Creates a new <see cref="PlainListBinding{T}"/> for a given <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items contained in the <see cref="IList{T}"/>.</typeparam>
        /// <param name="collection">The collection that the adapter will be created for.</param>
        /// <param name="customize">Action to customize created binding before it is returned</param>
        /// <returns>A View adapted for the item passed as parameter.</returns>
        public static PlainListBinding<T> GetPlainListBinding<T>(this IList<T> collection, Action<PlainListBinding<T>> customize = null)
        {
            var binding = new PlainListBinding<T>
            {
                DataSource = collection
            };
            customize?.Invoke(binding);
            return binding;
        }
    }
}

