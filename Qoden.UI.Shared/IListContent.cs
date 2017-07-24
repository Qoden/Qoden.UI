using System;
using System.Collections.Generic;
using Qoden.Validation;

namespace Qoden.UI
{
    public interface IListContent<T>
    {
        QView CreateView(int position, IViewHierarchyBuilder builder);
        void FillView(int pos, QView convertView);

        int ViewTypeCount { get; }
        int GetItemViewType(int position);

        T this[int pos] { get; }
        int Count { get; }

        long GetItemId(int position);
    }

    //public abstract class ListContent<T> : IListContent<T>
    //{
        
    //}

    public interface IGroupedContent
    {
        int GroupCount { get; }
        int GetChildrenCount(int group);

        int GetChildType(int group, int child);
        int ChildTypeCount { get; }

        int GetGroupType(int group);
        int GroupTypeCount { get; }
        bool HasStableIds { get; }

        QView CreateChildView(int group, int child, IViewHierarchyBuilder builder);
        void FillChildView(int group, int child, QView item);
        QView CreateGroupView(int group, IViewHierarchyBuilder builder);
        void FillGroupView(int group, QView section);

        bool IsSelectable(int group, int child);
        long GetGroupId(int groupPosition);
        long GetChildId(int groupPosition, int childPosition);
        float GetGroupHeaderHeight(int section);
    }

    public abstract class ListGroupedContent<TItem, TGroup> : IGroupedContent where TGroup : IList<TItem>
    {
        IList<TGroup> _dataSource;
        public IList<TGroup> DataSource 
        {
            get => _dataSource;
            set
            {
                Assert.Property(value).NotNull();
                _dataSource = value;
            }
        }

        public int GroupCount => DataSource.Count;

        public int GetChildrenCount(int group)
        {
            return DataSource[group].Count;
        }

        public abstract int ChildTypeCount { get; }
        public abstract int GetChildType(int group, int child);

        public abstract int GroupTypeCount { get; }
        public abstract int GetGroupType(int group);

        public virtual bool HasStableIds { get { return true; } }
        public virtual bool IsSelectable(int group, int child) { return false; }

        public virtual long GetGroupId(int groupPosition) { return groupPosition; }
        public virtual long GetChildId(int groupPosition, int childPosition) { return childPosition; }

        public abstract void FillChildView(int group, int child, QView item);
        public abstract void FillGroupView(int group, QView section);
        public abstract QView CreateChildView(int group, int child, IViewHierarchyBuilder builder);
        public abstract QView CreateGroupView(int group, IViewHierarchyBuilder builder);
        public abstract float GetGroupHeaderHeight(int section);
    }
}
