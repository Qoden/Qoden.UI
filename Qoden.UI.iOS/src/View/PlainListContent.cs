﻿using System;
using Foundation;
using Qoden.Validation;
using UIKit;

namespace Qoden.UI
{
    public abstract class PlainListContent : UITableViewDataSource
    {
        QView _item = new QView();
        IViewHierarchyBuilder _builder;

        public PlainListContent(IViewHierarchyBuilder builder)
        {
            Assert.Argument(builder, nameof(builder)).NotNull();
            Builder = builder;
        }

        public IViewHierarchyBuilder Builder { get; private set; }

        #region Cross platform inteface

        public abstract int Count { get; }
        public abstract QView CreateView(int position, IViewHierarchyBuilder builder);
        public abstract void FillView(int pos, QView convertView);
        public abstract int GetItemViewType(int position);

        #endregion

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var position = indexPath.Row;
            var typeIdx = GetItemViewType(position);
            var cell = tableView.DequeueReusableCell(typeIdx.ToString());
            if (cell == null)
            {
                var view = CreateView(position, ViewHierarchyBuilder.Instance);
                cell = TableViewUtil.ToTableViewCell(view);
            }
            if (cell is UITableViewCellAdapter)
            {
                _item.PlatformView = ((UITableViewCellAdapter)cell).CellView;
            }
            else
            {
                _item.PlatformView = cell;
            }
            FillView(position, _item);
            return cell;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return Count;
        }
    }
}
