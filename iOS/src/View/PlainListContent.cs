using System;
using Foundation;
using Qoden.UI.Wrappers;
using UIKit;

namespace Qoden.UI
{
    public abstract class PlainListContent : UITableViewDataSource, IPlainListContent, IKeepLastCell
    {
        UITableViewCell _lastCell;
        NSIndexPath _lastIndexPath;

        protected PlainListContent(ViewBuilder builder)
        {
            Builder = builder;
        }

        public UITableViewCell LastCell => _lastCell;
        public NSIndexPath LastIndexPath => _lastIndexPath;
        public ViewBuilder Builder { get; }

        #region Cross platform API

        public abstract int NumberOfRows();

        public abstract int GetCellType(int row);

        public abstract int CellTypeCount { get; }
        
        public abstract TableViewCell GetCell(PlainListCellContext context);

        #endregion


        #region Redirects from iOS API to IPlainListContent methods

        public sealed override nint RowsInSection(UITableView tableView, nint section)
        {
            return NumberOfRows();
        }

        public sealed override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var position = indexPath.Row;
            var typeIdx = GetCellType(position);
            var cell = tableView.DequeueReusableCell(typeIdx.ToString());
            var context = new PlainListCellContext()
            {
                ReusableCell = cell.AsCell(),
                Row = position
            };
            var tableViewCell = GetCell(context);
            _lastCell = tableViewCell;
            _lastIndexPath = indexPath;
            return tableViewCell;
        }

        #endregion
    }
}