﻿using System;
using Foundation;
 using Qoden.UI.Wrappers;
 using UIKit;

namespace Qoden.UI
{
    public abstract class NewGroupedListContent : UITableViewDataSource, INewGroupedListContent, IKeepLastCell,
        IKeepLastSection
    {
        private UITableViewCell _lastCell;
        private NSIndexPath _lastIndexPath;
        private UIView _lastSectionView;
        private nint _lastSection;
        
        protected NewGroupedListContent(ViewBuilder builder)
        {
            Builder = builder;
        }

        public ViewBuilder Builder { get; private set; }
        public UITableViewCell LastCell => _lastCell;
        public NSIndexPath LastIndexPath => _lastIndexPath;
        public UIView LastSectionView => _lastSectionView;
        public nint LastSection => _lastSection;
        
        #region Cross platform interface
        public abstract int NumberOfSections();
        public abstract int RowsInSection(int section);
        public abstract int SectionTypeCount { get; }
        public abstract int CellTypeCount { get; }
        public abstract int GetCellType(int section, int row);
        public abstract int GetSectionType(int section);
        public abstract View GetSection(GroupedListSectionContext context);
        public abstract TableViewCell GetCell(GroupedListCellContext context);
        #endregion
        
        #region Redirects from iOS API to IGroupedListContent methods
        public sealed override nint RowsInSection(UITableView tableView, nint section)
        {
            return RowsInSection((int)section);
        }

        public sealed override nint NumberOfSections(UITableView tableView)
        {
            return NumberOfSections();
        }

        public sealed override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cellType = GetCellType(indexPath.Section, indexPath.Row);
            var context = new GroupedListCellContext()
            {
                ReusableCell = tableView.DequeueReusableCell(cellType.ToString()).AsCell(),
                Row = indexPath.Row,
                Section = indexPath.Section
            };
            var cell = GetCell(context);
            _lastCell = cell;
            _lastIndexPath = indexPath;
            return cell;
        }

        #endregion
        
        #region Default implementations for common methods

        protected UIView DefaultGetViewForHeader(UITableView tableView, nint section)
        {
            var groupTypeId = GetSectionType((int)section);
            var sectionView = tableView.DequeueReusableHeaderFooterView(groupTypeId.ToString());
            var view = sectionView.AsView();
            var context = new GroupedListSectionContext()
            {
                SectionHeaderView = view,
                Section = (int) section
            };
            view = GetSection(context);
            _lastSectionView = sectionView;
            _lastSection = section;
            return view;
        }

        #endregion
    }


    public interface IKeepLastCell : IUITableViewDataSource, IUITableViewDelegate
    {
        UITableViewCell LastCell { get; }
        NSIndexPath LastIndexPath { get; }
    }

    public interface IKeepLastSection : IUITableViewDataSource, IUITableViewDelegate
    {
        UIView LastSectionView { get; }
        nint LastSection { get; }
    }
}
