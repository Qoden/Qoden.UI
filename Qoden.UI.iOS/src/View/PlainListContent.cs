﻿using System;
using Foundation;
using Qoden.Validation;
using UIKit;

namespace Qoden.UI
{
    public abstract partial class PlainListContent : UITableViewDataSource, IPlainListContent, IKeepLastCell
    {
        UITableViewCell _lastCell;
        NSIndexPath _lastIndexPath;

        public PlainListContent(IViewHierarchyBuilder builder)
        {
            Assert.Argument(builder, nameof(builder)).NotNull();
            Builder = builder;
        }

        public IViewHierarchyBuilder Builder { get; private set; }

        public UITableViewCell LastCell => _lastCell;
        public NSIndexPath LastIndexPath => _lastIndexPath;

        #region Redirects from iOS API to IPlainListContent methods

        public sealed override nint RowsInSection(UITableView tableView, nint section)
        {
            return NumberOfRows();
        }

        public sealed override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var position = indexPath.Row;
            var context = new PlainListCellContext()
            {
                Row = position,
                CellView = null,
                IsFresh = false
            };

            var typeIdx = GetCellType(position);
            var cell = tableView.DequeueReusableCell(typeIdx.ToString());

            if (cell == null)
            {
                context.IsFresh = true;
                var cellTypeId = GetCellType(context.Row);
                CreateView(cellTypeId, ref context);
                Assert.State(context.CellView, "CellView").NotNull();
                cell = TableViewUtil.ToTableViewCell(context.CellView, typeIdx);
            }
            if (cell is UITableViewCellAdapter)
            {
                context.CellView = ((UITableViewCellAdapter)cell).CellView;
            }
            else
            {
                context.CellView = cell;
            }
            GetCell(context);
            _lastCell = cell;
            _lastIndexPath = indexPath;
            return cell;
        }

        #endregion

        #region Internal API overrides

        protected virtual void CreateView(int cellTypeId, ref PlainListCellContext cellContext)
        {
            cellContext.CellView = TableViewUtil.CreateView(cellTypeId, CellTypes, Builder);
        }

        #endregion
    }
}
