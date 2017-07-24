using System;
using Foundation;
using UIKit;

namespace Qoden.UI
{
    public abstract class GroupedListContent : UITableViewDataSource
    {
        QView _item = new QView();
        QView _section = new QView();

        public GroupedListContent(IViewHierarchyBuilder builder)
        {
            Builder = builder;
        }

        #region Cross platform interface

        public abstract int GroupCount { get; }
        public IViewHierarchyBuilder Builder { get; private set; }

        public abstract int GetChildrenCount(int groupPosition);
        public abstract int GetChildType(int groupPosition, int childPosition);
        public abstract int GetGroupType(int groupPosition);
        public abstract QView CreateChildView(int groupPosition, int childPosition, IViewHierarchyBuilder builder);
        public abstract void FillChildView(int groupPosition, int childPosition, QView item);
        public abstract QView CreateGroupView(int groupPosition, IViewHierarchyBuilder builder);
        public abstract void FillGroupView(int groupPosition, QView section);

        #endregion

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return GetChildrenCount((int)section);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return GroupCount;
        }

        public sealed override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var child = indexPath.Row;
            var @group = indexPath.Section;

            var id = GetChildType(@group, child);
            var cell = tableView.DequeueReusableCell(id.ToString());
            if (cell == null)
            {
                var listItem = CreateChildView(@group, child, ViewHierarchyBuilder.Instance);
                cell = TableViewUtil.ToTableViewCell(listItem);
            }
            if (cell is UITableViewCellAdapter)
            {
                _item.PlatformView = ((UITableViewCellAdapter)cell).CellView;
            }
            else
            {
                _item.PlatformView = cell;
            }
            FillChildView(@group, child, _item);
            return cell;
        }

        [Export("tableView:viewForHeaderInSection:")]
        public UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var prefix = "Header";
            var group = (int)section;
            var groupType = GetGroupType(group);
            var groupView = tableView.DequeueReusableHeaderFooterView(prefix + groupType.ToString());
            if (groupView == null)
            {
                var qGroupView = CreateGroupView(group, ViewHierarchyBuilder.Instance);
                groupView = TableViewUtil.ToTableViewHeaderFooter(qGroupView);
            }
            if (groupView is UITableViewHeaderFooterViewAdapter)
            {
                _section.PlatformView = ((UITableViewHeaderFooterViewAdapter)groupView).View;
            }
            else
            {
                _section.PlatformView = groupView;
            }
            FillGroupView(group, _section);
            return groupView;
        }
    }
}
