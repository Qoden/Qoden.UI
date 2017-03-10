using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Foundation;
using UIKit;
using Qoden.Reflection;
using Qoden.Validation;
using Qoden.Binding;

namespace Qoden.UI.iOS
{
	public interface IUITableViewBinding : IUITableViewDataSource, IBinding 
	{
        /// <summary>
        /// Occurs when item is selected
        /// </summary>
        event EventHandler ItemSelected;
    }

    public static class IUITableViewBindingProperties
    {
		private static readonly Type t = typeof(IUITableViewBinding);
        public static readonly RuntimeEvent ItemSelectedEvent = new RuntimeEvent(t, "ItemSelected");

        public static EventHandlerSource<T> ItemSelectedTarget<T>(this T dataSource)
            where T : IUITableViewBinding
        {
            return new EventHandlerSource<T>(ItemSelectedEvent, dataSource)
            {
                ParameterExtractor = (sender, args) => ((ItemSelectedEventArgs)args).Item
            };
        }

        public class ItemSelectedEventArgs : EventArgs
        {
            public object Item { get; private set; }

            public ItemSelectedEventArgs(object item)
            {
                Item = item;
            }
        }
    }

    /// <summary>
	/// Base class for table view data sources. Subclasses implement one particular binding strategy.
	/// For example one subclass my implement binding for plain list of data objects, while another can implement
	/// binding for a list of groups of objects. 
	/// </summary>
	public abstract class UITableViewBindingBase<T> : NSObject, IUITableViewBinding
	{
		IList<T> dataSource;
		INotifyCollectionChanged notifier;
		UITableView tableView;

		public UITableViewBindingBase()
		{
			AddAnimation = UITableViewRowAnimation.Automatic;
			DeleteAnimation = UITableViewRowAnimation.Automatic;
		}

		/// <summary>
		/// When set, specifies which animation should be used when rows are added.
		/// </summary>
		public UITableViewRowAnimation AddAnimation
		{
			get;
			set;
		}

		/// <summary>
		/// When set, specifieds which animation should be used when a row is deleted.
		/// </summary>
		public UITableViewRowAnimation DeleteAnimation
		{
			get;
			set;
		}

        /// <summary>
        /// When set to true, item selected returns list of selected indexes
        /// </summary>
        public bool  MultipleSelection
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs when item is selected
        /// </summary>
        public event EventHandler ItemSelected;

        /// <summary>
        /// Selection is preserved if true. Otherwise selection is reset after click.
        /// </summary>
        public bool PreserveSelection { get; set; }

        /// <summary>
        /// The data source of this list controller.
        /// </summary>
        public IList<T> DataSource
		{
			get
			{
				return dataSource;
			}

			set
			{
				if (Bound) throw new InvalidOperationException("Cannot update bound DataSource");

				if (Equals(dataSource, value))
				{
					return;
				}

				dataSource = value;
				notifier = value as INotifyCollectionChanged;

				if (tableView != null && Bound && Enabled)
				{
					UpdateTarget();
				}
			}
		}
        #region Selection

        /// <summary>
        /// Override this.
        /// Returns Item at indexPath.
        /// </summary>
        public virtual object ItemForSelection(NSIndexPath indexPath)
        {
            return null;
        }

        /// <summary>
        /// Override this.
        /// Returns Item at indexPath.
        /// </summary>
        public virtual object ItemsForSelections(NSIndexPath[] indexPaths)
        {
            return null;
        }

        [Export("tableView:willSelectRowAtIndexPath:")]
        public NSIndexPath WillSelectRow(UITableView tableView, NSIndexPath indexPath)
        {
            TableView = tableView;
            if (!PreserveSelection)
            {
                if (MultipleSelection)
                    InvokeItemsSelected(tableView.IndexPathsForSelectedRows);
                else
                    InvokeItemSelected(indexPath);
                return null;
            }
            return indexPath;
        }

        [Export("tableView:willDeselectRowAtIndexPath:")]
        public NSIndexPath WillDeselectRow(UITableView tableView, NSIndexPath indexPath)
        {
            TableView = tableView;
            if (!PreserveSelection)
            {
                if (MultipleSelection)
                    InvokeItemsSelected(tableView.IndexPathsForSelectedRows);
                else
                    InvokeItemSelected(indexPath);
                return null;
            }
            return indexPath;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void DidSelectRow(UITableView tableView, NSIndexPath indexPath)
        {
            TableView = tableView;
            if (PreserveSelection)
            {
                if(MultipleSelection)
                    InvokeItemsSelected(tableView.IndexPathsForSelectedRows);
                else
                    InvokeItemSelected(indexPath);
            }
        }

        [Export("tableView:didDeselectRowAtIndexPath:")]
        public void DidDeselectRow(UITableView tableView, NSIndexPath indexPath)
        {
            TableView = tableView;
            if (PreserveSelection)
            {
                if (MultipleSelection)
                    InvokeItemsSelected(tableView.IndexPathsForSelectedRows);
            }
        }


        void InvokeItemSelected(NSIndexPath indexPath)
        {
            if (ItemSelected != null)
            {
                var item = ItemForSelection(indexPath);
                var args = new IUITableViewBindingProperties.ItemSelectedEventArgs(item);
                ItemSelected.Invoke(this, args);
            }
        }

        void InvokeItemsSelected(NSIndexPath[] indexPaths)
        {
            if (ItemSelected != null)
            {
                var item = ItemsForSelections(indexPaths);
                var args = new IUITableViewBindingProperties.ItemSelectedEventArgs(item);
                ItemSelected.Invoke(this, args);
            }
        }

        #endregion

        public UITableView TableView
		{
			get
			{
				return tableView;
			}

			set
			{
				Assert.Property(value).NotNull();

				if (value == tableView) return;

				bool rebind = false;
				if (tableView != null)
				{
					tableView.WeakDataSource = null;
					if (this is IUITableViewDelegate) tableView.WeakDelegate = null;
					if (Bound)
					{
						Unbind();
						rebind = true;
					}
				}
				tableView = value;
				tableView.WeakDataSource = this;
				if (this is IUITableViewDelegate) tableView.WeakDelegate = this;
				if (rebind)
				{
					Bind();
					UpdateTarget();
				}
			}
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

		public virtual void Bind()
		{
			if (Bound) return;
			if (notifier != null)
			{
				notifier.CollectionChanged += HandleCollectionChanged;
			}
            Bound = true;
		}

		public virtual void Unbind()
		{
			if (!Bound) return;
			if (notifier != null)
			{
				notifier.CollectionChanged -= HandleCollectionChanged;
			}
            Bound = false;
		}

		public void UpdateTarget()
		{
			if (Bound && tableView != null)
			{
				tableView.ReloadData();
			}
		}

		public void UpdateSource()
		{
			throw new NotSupportedException();
		}

		protected abstract void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
		public abstract UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath);
		public abstract nint RowsInSection(UITableView tableView, nint section);

		[Export("numberOfSectionsInTableView:")]
		public abstract nint NumberOfSections(UITableView tableView);
	}
}
