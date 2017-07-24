using System;
using Android.Views;
using Android.Widget;
using Qoden.Validation;

namespace Qoden.UI
{
    public abstract class GroupedListContent : BaseExpandableListAdapter, IHeterogeneousExpandableList
    {
        QView _item = new QView();
        QView _group = new QView();

        public GroupedListContent(IViewHierarchyBuilder builder)
        {
            Assert.Argument(builder, nameof(builder)).NotNull();
            Builder = builder;
        }

        public IViewHierarchyBuilder Builder { get; private set; }

        #region Cross Platform interface
        public override abstract int GroupCount { get; }
        public override abstract int GetChildrenCount(int groupPosition);

        public abstract QView CreateChildView(int group, int child, IViewHierarchyBuilder builder);
        public abstract QView CreateGroupView(int group, IViewHierarchyBuilder builder);
        public abstract void FillChildView(int group, int child, QView item);
        public abstract void FillGroupView(int group, QView section);
        #endregion

        public override bool HasStableIds { get { return true; } }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return null;
        }

        public sealed override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                var view = CreateChildView(groupPosition, childPosition, Builder);
                convertView = view.PlatformView;
            }
            _item.PlatformView = convertView;
            FillChildView(groupPosition, childPosition, _item);
            return _item.PlatformView;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return null;
        }

        public sealed override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                var view = CreateGroupView(groupPosition, Builder);
                convertView = view.PlatformView;
                var mExpandableListView = parent as ExpandableListView;
                if (mExpandableListView != null)
                    mExpandableListView.ExpandGroup(groupPosition);
            }
            _group.PlatformView = convertView;
            FillGroupView(groupPosition, _group);
            return _group.PlatformView;
        }

    }
}
