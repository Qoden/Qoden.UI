using Android.Views;
using Android.Widget;
using Qoden.UI.Wrappers;
using View = Qoden.UI.Wrappers.View;

namespace Qoden.UI
{
    public abstract class NewGroupedListContent : BaseExpandableListAdapter, INewGroupedListContent
    {
        protected NewGroupedListContent(ViewBuilder builder)
        {            
            Builder = builder;
        }

        public ViewBuilder Builder { get; }
        
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
        
        #region Redirects from Android API to IGroupedListContent methods

        public sealed override int GetChildType(int groupPosition, int childPosition)
        {
            return GetCellType(groupPosition, childPosition);
        }

        public sealed override int ChildTypeCount => CellTypeCount;

        public sealed override int GetGroupType(int groupPosition)
        {
            return GetSectionType(groupPosition);
        }

        public sealed override int GroupTypeCount => SectionTypeCount;

        public sealed override int GetChildrenCount(int groupPosition)
        {
            return RowsInSection(groupPosition);
        }

        public sealed override int GroupCount => NumberOfSections();

        public sealed override Android.Views.View GetChildView(int groupPosition, int childPosition, bool isLastChild, Android.Views.View convertView, ViewGroup parent)
        {
            var context = new GroupedListCellContext()
            {
                ReusableCell = convertView.AsCell(),
                Row = childPosition,
                Section = groupPosition,
                IsExpanded = isLastChild,
                Parent = parent.AsView()
            };
            return GetCell(context);
        }

        public sealed override Android.Views.View GetGroupView(int groupPosition, bool isExpanded, Android.Views.View convertView, ViewGroup parent)
        {
            var context = new GroupedListSectionContext()
            {
                SectionHeaderView = convertView.AsView(),
                Section = groupPosition,
                IsExpanded = isExpanded,
                Parent = parent.AsView()
            };
            var section = GetSection(context);
            if (convertView == null)
            {
                var mExpandableListView = parent as ExpandableListView;
                mExpandableListView?.ExpandGroup(groupPosition);
            }
            return section;
        }

        #endregion
        
        #region Convenience overrides

        public override long GetChildId(int groupPosition, int childPosition)
        {
            long id = groupPosition << 32;
            id = id | (uint)childPosition;
            return id;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return false;
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override bool HasStableIds => true;

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return null;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return null;
        }
        #endregion
    }
}
