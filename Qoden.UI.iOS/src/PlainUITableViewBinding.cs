using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Qoden.UI.iOS
{
    /// <summary>
    /// Implements UITableViewDataSource backed by plain list (no groups).
    /// </summary>
    public class PlainUITableViewBinding<T> : UITableViewBindingBase<T>, IUITableViewDelegate
    {
        /// <summary>
        /// Provide table view cell for given data object at given index
        /// </summary>
        public Func<UITableView, T, int, UITableViewCell> CellFactory { get; set; }
        public Action<UITableView, UITableViewCell, NSIndexPath> WillDisplayCell { get; set; }
        public Func<T, String> GetNameForIndexing { get; set; }
        public Func<UITableView, NSIndexPath, UITableViewRowAction[]> EditActionsForRow { get; set; }

        static readonly NSString DEFAULT_ID = new NSString("DEFAULT_CELL_ID");

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            TableView = tableView;
            if (CellFactory != null)
            {
                return CellFactory(tableView, DataSource[indexPath.Row], indexPath.Row);
            }
            else
            {
                return tableView.DequeueReusableCell(DEFAULT_ID, indexPath) ?? new UITableViewCell(UITableViewCellStyle.Default, DEFAULT_ID);
            }
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            TableView = tableView;
            return DataSource != null ? DataSource.Count : 0;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            TableView = tableView;
            return 1;
        }

        [Export("tableView:willDisplayCell:forRowAtIndexPath:")]
        public void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (WillDisplayCell != null)
                WillDisplayCell(tableView, cell, indexPath);
        }

        public override object ItemForSelection(NSIndexPath indexPath)
        {
            return DataSource[indexPath.Row];
        }

        public override object ItemsForSelections(NSIndexPath[] indexPaths)
        {
            if (indexPaths == null) return new List<T>();
            return DataSource.Where((T item, int index) => indexPaths.Select(_ => _.Row).Contains(index));
        }
        #region indexing
        [Export("sectionIndexTitlesForTableView:")]
        public string[] SectionIndexTitles(UITableView tableView)
        {
            if (GetNameForIndexing == null) return null;
            if (DataSource.Count == 0) return null;
            return new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "#" };
        }

        [Export("tableView:sectionForSectionIndexTitle:atIndex:")]
        public nint SectionFor(UITableView tableView, string title, nint atIndex)
        {
            if (GetNameForIndexing == null) return -1;
            if (DataSource.Count == 0) return -1;
            for (int i = 0; i < DataSource.Count; i++)
            {
                if (String.IsNullOrEmpty(GetNameForIndexing(DataSource[i]))) return -1;
                string firstLetter = GetNameForIndexing(DataSource[i]).Substring(0, 1);

                //if firstLetter is after 'title' by alpabet
                if (!title.Equals("#") && firstLetter.CompareTo(title) > 0)
                {
                    tableView.ScrollToRow(NSIndexPath.FromRowSection(i == 0 ? i : i - 1, 0), UITableViewScrollPosition.Top, false);
                    return -1;
                }
                //if firstLetter equals 'title'
                else if (!title.Equals("#") && firstLetter.Equals(title))
                {
                    tableView.ScrollToRow(NSIndexPath.FromRowSection(i, 0), UITableViewScrollPosition.Top, false);
                    return -1;
                }
                //if firstLetter is not a letter
                else if (title.Equals("#") && !char.IsLetter(firstLetter.ElementAt(0)))
                {
                    tableView.ScrollToRow(NSIndexPath.FromRowSection(i, 0), UITableViewScrollPosition.Top, false);
                    return -1;
                }
            }
            tableView.ScrollToRow(NSIndexPath.FromRowSection(DataSource.Count - 1, 0), UITableViewScrollPosition.Middle, false);
            return -1;
        }
        #endregion indexing

        [Export("tableView:editActionsForRowAtIndexPath:")]
        public UITableViewRowAction[] EditActions(UITableView tableView, NSIndexPath indexPath)
        {
            if (EditActionsForRow != null)
                return EditActionsForRow(tableView, indexPath);
            else
            return new List<UITableViewRowAction>().ToArray();

        }

        [Export("tableView:editingStyleForRowAtIndexPath:")]
        public UITableViewCellEditingStyle EditingStyle(UITableView tableView, NSIndexPath indexPath)
        {
            if (EditActionsForRow != null)
                return UITableViewCellEditingStyle.Delete;
            else
                return UITableViewCellEditingStyle.None;
        }

        public NSIndexPath FirstIndexPathForItem(T item)
        {
            int index = DataSource.IndexOf(item);
            if (index < 0) return null;
            return NSIndexPath.FromRowSection(index, 0);
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
                        var paths = new NSIndexPath[count];

                        for (var i = 0; i < count; i++)
                        {
                            paths[i] = NSIndexPath.FromRowSection(e.NewStartingIndex + i, 0);
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
                            var index = NSIndexPath.FromRowSection(e.OldStartingIndex + i, 0);
                            paths[i] = index;
                        }

                        Action action = () =>
                        {
                            TableView.DeleteRows(paths, DeleteAnimation);
                        };
                        if (DeleteAnimation == UITableViewRowAnimation.None)
                        {
                            UIView.PerformWithoutAnimation(action);
                        }
                        else
                        {
                            action();
                        }
                    }
                    break;

                default:
                    TableView.ReloadData();
                    break;
            }
        }
    }
}

