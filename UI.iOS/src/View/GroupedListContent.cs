using System;
using Foundation;
using Qoden.Validation;
using UIKit;

namespace Qoden.UI
{
    public abstract partial class GroupedListContent : UITableViewDataSource, IGroupedListContent, IKeepLastCell
    {
        public GroupedListContent(ViewBuilder builder)
        {
            Builder = builder;
        }

        public ViewBuilder Builder { get; private set; }
        public UITableViewCell LastCell => _lastCell;
        public NSIndexPath LastIndexPath => _lastIndexPath;

        #region Cross platform interface

        public abstract int NumberOfSections();
        public abstract int RowsInSection(int section);
        public abstract void GetSection(GroupedListSectionContext sectionContext);
        public abstract void GetCell(GroupedListCellContext cellContext);

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

        protected UITableViewCell _lastCell;
        protected NSIndexPath _lastIndexPath;

        public sealed override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var childIdx = indexPath.Row;
            var sectionIdx = indexPath.Section;

            var context = new GroupedListCellContext()
            {
                IsFresh = false,
                Section = sectionIdx,
                Row = childIdx,
                CellView = null
            };

            var cellTypeId = GetCellType(sectionIdx, childIdx);
            UITableViewCell cellView;
            cellView = tableView.DequeueReusableCell(cellTypeId.ToString());
            if (cellView == null)
            {
                context.IsFresh = true;
                CreateCell(cellTypeId, ref context);
                Assert.State(context.CellView, "ChildView").NotNull();

                cellView = TableViewUtil.ToTableViewCell(context.CellView, cellTypeId);
            }

            if (cellView is UITableViewCellAdapter)
            {
                context.CellView = ((UITableViewCellAdapter)cellView).CellView;
            }
            else
            {
                context.CellView = cellView;
            }

            GetCell(context);

            //If cell view lives inside UITableViewCellAdapter then resize cell
            //as per contents
            if (cellView != context.CellView)
            {
                var heightDx = cellView.Frame.Height - cellView.ContentView.Frame.Height;
                cellView.Frame = new CoreGraphics.CGRect(0, 0, 
                                                         tableView.Bounds.Width, 
                                                         context.CellView.Bounds.Height + heightDx);
            }

            _lastCell = cellView;
            _lastIndexPath = indexPath;

            return cellView;
        }

        #endregion

        #region Default implementations for common methods

        protected UIView DefaultGetViewForHeader(UITableView tableView, nint section)
        {
            var groupTypeId = GetSectionType((int)section);

            var context = new GroupedListSectionContext()
            {
                IsFresh = false,
                SectionHeaderView = null,
                Section = (int)section
            };

            UITableViewHeaderFooterView sectionView;
            sectionView = tableView.DequeueReusableHeaderFooterView(groupTypeId.ToString());

            if (sectionView == null)
            {
                context.IsFresh = true;
                CreateSection(groupTypeId, ref context);
                Assert.State(context.SectionHeaderView, "SectionHeaderView").NotNull();
                sectionView = TableViewUtil.ToTableViewHeaderFooter(context.SectionHeaderView, groupTypeId);
                sectionView.Frame = new CoreGraphics.CGRect(0, 0, tableView.Bounds.Width, tableView.EstimatedSectionHeaderHeight);
            }

            if (sectionView is UITableViewHeaderFooterViewAdapter)
            {
                context.SectionHeaderView = ((UITableViewHeaderFooterViewAdapter)sectionView).View;
            }
            else
            {
                context.SectionHeaderView = sectionView;
            }

            GetSection(context);

            return sectionView;
        }

        #endregion

        #region Internal API overrides

        protected virtual void CreateCell(int cellTypeId, ref GroupedListCellContext cellContext)
        {
            cellContext.CellView = TableViewUtil.CreateView(cellTypeId, CellTypes);
        }

        protected virtual void CreateSection(int sectionTypeId, ref GroupedListSectionContext sectionContext)
        {
            var groupType = SectionTypes[sectionTypeId];
            if (typeof(UITableViewHeaderFooterView).IsAssignableFrom(groupType))
            {
                sectionContext.SectionHeaderView = (UITableViewHeaderFooterView)Activator.CreateInstance(groupType, new[] { sectionTypeId.ToString() });
            }
            else
            {
                sectionContext.SectionHeaderView = (UIView)Activator.CreateInstance(groupType);
            }
        }
        #endregion

    }

    public interface IKeepLastCell
    {
        UITableViewCell LastCell { get; }
        NSIndexPath LastIndexPath { get; }
    }
}
