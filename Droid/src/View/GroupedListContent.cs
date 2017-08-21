using System;
using Android.Views;
using Android.Widget;
using Qoden.Validation;

namespace Qoden.UI
{
    public abstract partial class GroupedListContent : BaseExpandableListAdapter, IHeterogeneousExpandableList, IGroupedListContent
    {
        public GroupedListContent(ViewBuilder builder)
        {            
            Builder = builder;
        }

        public ViewBuilder Builder { get; private set; }

        #region Cross Platform interface

        public abstract int NumberOfSections();
        public abstract int RowsInSection(int section);
        public abstract void GetSection(GroupedListSectionContext sectionContext);
        public abstract void GetCell(GroupedListCellContext cellContext);

        #endregion

        #region Redirects from Android API to IGroupedListContent methods

        public sealed override int GetChildType(int groupPosition, int childPosition)
        {
            return GetCellType(groupPosition, childPosition);
        }

        public sealed override int ChildTypeCount => CellTypes.Length;

        public sealed override int GetGroupType(int groupPosition)
        {
            return GetSectionType(groupPosition);
        }

        public sealed override int GroupTypeCount => SectionTypes.Length;

        public sealed override int GetChildrenCount(int groupPosition)
        {
            return RowsInSection(groupPosition);
        }

        public sealed override int GroupCount => NumberOfSections();

        public sealed override Android.Views.View GetChildView(int groupPosition, int childPosition, bool isLastChild, Android.Views.View convertView, ViewGroup parent)
        {
            var context = new GroupedListCellContext()
            {
                IsFresh = false,
                CellView = convertView,
                Row = childPosition,
                Section = groupPosition
            };

            if (convertView == null)
            {
                var childTypeId = GetCellType(groupPosition, childPosition);
                CreateCell(childTypeId, ref context);
                Assert.State(context.CellView, "CellView").NotNull();
                context.IsFresh = true;
            }
            GetCell(context);
            return context.CellView;
        }

        public sealed override Android.Views.View GetGroupView(int groupPosition, bool isExpanded, Android.Views.View convertView, ViewGroup parent)
        {
            var context = new GroupedListSectionContext()
            {
                IsFresh = false,
                SectionHeaderView = convertView,
                Section = groupPosition
            };
            if (convertView == null)
            {
                var groupTypeId = GetSectionType(groupPosition);
                CreateSection(groupTypeId, ref context);
                Assert.State(context.SectionHeaderView, "SectionHeaderView").NotNull();

                var mExpandableListView = parent as ExpandableListView;
                if (mExpandableListView != null)
                    mExpandableListView.ExpandGroup(groupPosition);
                context.IsFresh = true;
            }

            GetSection(context);
            return context.SectionHeaderView;
        }

        #endregion

        #region Internal API overrides

        protected virtual void CreateCell(int cellTypeId, ref GroupedListCellContext cellContext)
        {
            var childViewType = CellTypes[cellTypeId];
            ActivatorArgs[0] = Builder.Context;
            var convertView = (Android.Views.View)Activator.CreateInstance(childViewType, ActivatorArgs);
            cellContext.CellView = convertView;
        }

        object[] ActivatorArgs = new object[1];

        protected virtual void CreateSection(int sectionTypeId, ref GroupedListSectionContext sectionContext)
        {
            var groupType = SectionTypes[sectionTypeId];
            ActivatorArgs[0] = Builder.Context;
            var convertView = (Android.Views.View)Activator.CreateInstance(groupType, ActivatorArgs);
            sectionContext.SectionHeaderView = convertView;
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

        public override bool HasStableIds { get { return true; } }

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
