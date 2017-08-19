using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Android.Widget;
using Qoden.Binding;
using Qoden.Validation;
using AndroidView = Android.Views.View;
using AndroidViewGroup = Android.Views.ViewGroup;

namespace Qoden.UI
{
    public class GroupedObservableAdapter<GT, T> : BaseAdapter, IBinding where GT : IList<T>, INotifyCollectionChanged
    {
        IList<GT> dataSource;
        List<GT> previousDataSource;
        INotifyCollectionChanged _notifier;

        /// <summary>
        /// Gets the number of items in the DataSource.
        /// </summary>
        public override int Count
        {
            get
            {
                if (dataSource == null)
                {
                    return 0;
                }
                return dataSource.Sum(groupData => groupData.Count());
            }
        }

        /// <summary>
        /// Gets or sets the list containing the items to be represented in the list control.
        /// </summary>
        public IList<GT> DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                if (Bound) throw new InvalidOperationException("Cannot change DataSource when adapter is bound.");

                if (Equals(dataSource, value))
                {
                    return;
                }

                dataSource = value;
                previousDataSource = new List<GT>(dataSource);
                _notifier = dataSource as INotifyCollectionChanged;
            }
        }

        /// <summary>
        /// Gets and sets a method taking an item's position in the list, the item itself,
        /// and a recycled Android View, and returning an adapted View for this item. Note that the recycled
        /// view might be null, in which case a new View must be inflated by this method.
        /// </summary>
        public Func<int, GT, T, AndroidView, Android.Views.ViewGroup, AndroidView> ViewFactory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets a method taking an group's position in the list, the group itself,
        /// and a recycled Android View, and returning an adapted header View for this group. Note that the recycled
        /// view might be null, in which case a new View must be inflated by this method.
        /// </summary>
        public Func<int, GT, AndroidView, Android.Views.ViewGroup, AndroidView> HeaderViewFactory
        {
            get;
            set;
        }

        public bool Enabled
        {
            get;
            set;
        }

        public bool Bound
        {
            get;
            private set;
        }

        /// <summary>
        /// Don't use this method. Use adapter[position] instead.
        /// </summary>
        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        /// <summary>
        /// Gets the item or group corresponding to the index in the DataSource.
        /// </summary>
        /// <param name="position">The index of the item/group that needs to be returned.</param>
        /// <returns>The item/group corresponding to the index in the DataSource</returns>
        public object this[int position]
        {
            get
            {
                GT groupData;
                return GetItemAtPosition(position, out groupData);
            }
        }

        protected object GetItemAtPosition(int position, out GT outGroup)
        {
            if (dataSource == null)
            {
                outGroup = default(GT);
                return null;
            }
            foreach (var groupData in dataSource)
            {
                outGroup = groupData;
                if (position < 0)
                    break;

                if (position < groupData.Count())
                {
                    return groupData[position];
                }
                position -= groupData.Count();
            }
            outGroup = default(GT);
            return null;
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

            GT groupData;
            var item = GetItemAtPosition(position, out groupData);
            if (item is T)
            {
                return ViewFactory(position, groupData, (T)item, convertView, parent);
            }
            if (item is GT)
            {
                return HeaderViewFactory(position, (GT)item, convertView, parent);
            }
            return null;
        }

        void NotifierCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Enabled)
                NotifyDataSetChanged();

            if (Bound)
            {
                foreach (var groupData in previousDataSource)
                {
                    UnbindGroup(groupData);
                }
                foreach (var groupData in DataSource)
                {
                    BindGroup(groupData);
                }
                previousDataSource = new List<GT>(dataSource);
            }
        }

        void HandleGroupCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Enabled)
                NotifyDataSetChanged();
        }

        public override int GetItemViewType(int position)
        {
            GT groupData;
            var item = GetItemAtPosition(position, out groupData);

            if (item is T)
                return TYPE_DATA;
            else return TYPE_HEADER;
        }

        public override int ViewTypeCount
        {
            get
            {
                return TYPES_COUNT;
            }
        }

        private const int TYPES_COUNT = 2;

        private const int TYPE_HEADER = 0;
        private const int TYPE_DATA = 1;

        #region Binding

        public void Bind()
        {
            if (Bound)
                return;
            Assert.State(DataSource != null).IsTrue("Source is not set");

            foreach (var groupData in DataSource)
            {
                BindGroup(groupData);
            }
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

            foreach (var groupData in DataSource)
            {
                UnbindGroup(groupData);
            }
            if (_notifier != null)
            {
                _notifier.CollectionChanged -= NotifierCollectionChanged;
            }
            Bound = false;
        }

        void BindGroup(GT groupData)
        {
            INotifyCollectionChanged notifier = groupData as INotifyCollectionChanged;
            if (notifier != null)
            {
                notifier.CollectionChanged += HandleGroupCollectionChanged;
            }
        }

        void UnbindGroup(GT groupData)
        {
            INotifyCollectionChanged notifier = groupData as INotifyCollectionChanged;
            if (notifier != null)
            {
                notifier.CollectionChanged -= HandleGroupCollectionChanged;
            }
        }

        #endregion

        public void UpdateTarget()
        {
            NotifyDataSetChanged();
        }

        public void UpdateSource()
        {
            throw new NotSupportedException();
        }
    }


    public static class GroupedObservableCollectionAdapterExtension
    {
        /// <summary>
        /// Creates a new <see cref="GroupedObservableAdapter{GT, T}"/> for a given <see cref="IList{GT}"/>.
        /// </summary>
        /// <typeparam name="GT">The type of the groups contained in the <see cref="IList{GT}"/>.</typeparam>
        /// <typeparam name="T">The type of the items contained in the <see cref="IList{GT}"/>.</typeparam>
        /// <param name="collection">The collection that the adapter will be created for.</param>
        /// <param name="viewFactory">A method taking an item's position in the list, the item itself,
        /// and a recycled Android View, and returning an adapted View for this item. Note that the recycled
        /// view might be null, in which case a new View must be inflated by this method.</param>
        /// <param name="headerViewFactory">A method taking an group's position in the list, the group itself,
        /// and a recycled Android View, and returning an adapted header View for this group. Note that the recycled
        /// view might be null, in which case a new View must be inflated by this method.</param>
        /// <returns>A View adapted for the item passed as parameter.</returns>
        public static GroupedObservableAdapter<GT, T> GetGroupedAdapter<GT, T>(this IList<GT> collection,
                                                         Func<int, GT, T, AndroidView, Android.Views.ViewGroup, AndroidView> viewFactory,
                                                         Func<int, GT, AndroidView, Android.Views.ViewGroup, AndroidView> headerViewFactory)
            where GT : IList<T>, INotifyCollectionChanged
        {
            return new GroupedObservableAdapter<GT, T>
            {
                DataSource = collection,
                ViewFactory = viewFactory,
                HeaderViewFactory = headerViewFactory
            };
        }
    }
}
