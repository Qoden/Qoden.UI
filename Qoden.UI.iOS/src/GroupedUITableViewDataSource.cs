using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Foundation;
using UIKit;

namespace Qoden.UI.iOS
{
    /// <summary>
    /// Implements UITableViewDataSource backed by groped list.
    /// </summary>
    public class GroupedUITableViewDataSource<GT, T> : UITableViewBindingBase<GT>, IUITableViewDelegate where GT : IList<T>, INotifyCollectionChanged
    {
        /// <summary>
        /// Provide table view cell for given data object at given group index and row index
        /// </summary>
        public Func<UITableView, T, int, int, UITableViewCell> CellFactory { get; set; }

        /// <summary>
        /// Provide table header view for given group data object at given index
        /// </summary>
        public Func<UITableView, GT, int, UIView> HeaderFactory { get; set; }

        /// <summary>
        /// Provide table header view height for given group data object at given index
        /// </summary>
        public Func<UITableView, GT, int, float> HeaderHeightFactory { get; set; }

        /// <summary>
        /// Provide table footer view for given group data object at given index
        /// </summary>
        public Func<UITableView, GT, int, UIView> FooterFactory { get; set; }

        /// <summary>
        /// Provide table footer view height for given group data object at given index
        /// </summary>
        public Func<UITableView, GT, int, float> FooterHeightFactory { get; set; }

        #region Row

        static readonly NSString DEFAULT_ID = new NSString("DEFAULT_CELL_ID");

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            TableView = tableView;
            if (CellFactory != null)
            {
                return CellFactory(tableView, DataSource[indexPath.Section][indexPath.Row], indexPath.Section, indexPath.Row);
            }
			return tableView.DequeueReusableCell(DEFAULT_ID, indexPath) ?? new UITableViewCell(UITableViewCellStyle.Default, DEFAULT_ID);
        }

        #endregion

        #region Section Header

        static readonly NSString DEFAULT_HEADER_ID = new NSString("DEFAULT_HEADER_ID");

        [Export("tableView:viewForHeaderInSection:")]
        public UIView GetViewForHeader(UITableView tableView, nint section)
        {
            TableView = tableView;
            if (HeaderFactory != null)
            {
                int sectionIdx = Convert.ToInt32(section);
                return HeaderFactory(tableView, DataSource[sectionIdx], sectionIdx);
            }
			return tableView.DequeueReusableHeaderFooterView(DEFAULT_HEADER_ID);
        }

        [Export("tableView:estimatedHeightForHeaderInSection:")]
        public nfloat EstimatedHeightForHeader(UITableView tableView, nint section)
        {
            TableView = tableView;
            if (HeaderHeightFactory != null)
            {
                int sectionIdx = Convert.ToInt32(section);
                return HeaderHeightFactory(tableView, DataSource[sectionIdx], sectionIdx);
            }
            if (HeaderFactory != null)
            {
                int sectionIdx = Convert.ToInt32(section);
                return HeaderFactory(tableView, DataSource[sectionIdx], sectionIdx).Frame.Height;
            }
			return 0;
        }

        #endregion

        #region Section Footer

        static readonly NSString DEFAULT_FOOTER_ID = new NSString("DEFAULT_FOOTER_ID");

        [Export("tableView:viewForFooterInSection:")]
        public UIView GetViewForFooter(UITableView tableView, nint section)
        {
            TableView = tableView;
            if (FooterFactory != null)
            {
                int sectionIdx = Convert.ToInt32(section);
                return FooterFactory(tableView, DataSource[sectionIdx], sectionIdx);
            }
			return tableView.DequeueReusableHeaderFooterView(DEFAULT_FOOTER_ID);
        }

        [Export("tableView:estimatedHeightForFooterInSection:")]
        public nfloat EstimatedHeightForFooter(UITableView tableView, nint section)
        {
            TableView = tableView;
            if (FooterHeightFactory != null)
            {
                int sectionIdx = Convert.ToInt32(section);
                return FooterHeightFactory(tableView, DataSource[sectionIdx], sectionIdx);
            }
            if (FooterFactory != null)
            {
                int sectionIdx = Convert.ToInt32(section);
                return FooterFactory(tableView, DataSource[sectionIdx], sectionIdx).Frame.Height;
            }
			return 0;
        }

        #endregion

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            TableView = tableView;
            if (DataSource == null)
            {
                return 0;
            }
            return DataSource[Convert.ToInt32(section)].Count;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            TableView = tableView;
            return DataSource != null ? DataSource.Count : 0;
        }

        public override object ItemForSelection(NSIndexPath indexPath)
        {
            return DataSource[indexPath.Section][indexPath.Row];
        }

        public override object ItemsForSelections(NSIndexPath[] indexPaths)
        {
            var result = new List<T>();
			for (int i = 0; i < indexPaths.Length; ++i)
			{
				var indexPath = indexPaths[i];
				result.Add(DataSource[indexPath.Section][indexPath.Row]);	
			}
            return result;
        }
        #region Binding

        public override void Bind()
        {
            if (Bound) return;
            foreach(var groupData in DataSource)
            {
                BindGroup(groupData);
            }
            base.Bind();
        }

        public override void Unbind()
        {
            if (!Bound) return;
            foreach (var groupData in DataSource)
            {
                UnbindGroup(groupData);
            }
            base.Unbind();
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

        protected void HandleGroupCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (TableView == null || !Enabled)
            {
                return;
            }

            int sectionIdx = DataSource.IndexOf((GT)sender);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var count = e.NewItems.Count;
                        var paths = new NSIndexPath[count];

                        for (var i = 0; i < count; i++)
                        {
                            paths[i] = NSIndexPath.FromRowSection(e.NewStartingIndex + i, sectionIdx);
                        }

                        TableView.InsertRows(paths, AddAnimation);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        var count = e.OldItems.Count;
                        var paths = new NSIndexPath[count];

                        for (var i = 0; i < count; i++)
                        {
                            paths[i] = NSIndexPath.FromRowSection(e.OldStartingIndex + i, sectionIdx);
                        }

                        TableView.DeleteRows(paths, DeleteAnimation);
                    }
                    break;

                default:
                    TableView.ReloadData();
                    break;
            }
        }

        protected override void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (TableView == null || !Enabled)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var count = e.NewItems.Count;
						var paths = new NSMutableIndexSet();

                        for (var i = 0; i < count; i++)
                        {
                            var itemIndex = e.NewStartingIndex + i;
                            paths.Add((nuint)(itemIndex));
                            BindGroup(DataSource[itemIndex]);
                        }

                        TableView.InsertSections(paths, AddAnimation);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        var count = e.OldItems.Count;
                        var paths = new NSMutableIndexSet();

                        for (var i = 0; i < count; i++)
                        {
                            var itemIndex = e.OldStartingIndex + i;
                            paths.Add((nuint)(itemIndex));
                            UnbindGroup(DataSource[itemIndex]);
                        }

                        TableView.DeleteSections(paths, DeleteAnimation);
                    }
                    break;

                default:
                    TableView.ReloadData();
                    break;
            }
        }
    }
}
