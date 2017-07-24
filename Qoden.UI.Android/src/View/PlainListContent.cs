using System;
using Android.Widget;
using Android.Views;
using Qoden.Validation;

namespace Qoden.UI
{
    public abstract class PlainListContent : BaseAdapter<object>
    {
        QView _item = new QView();
        IViewHierarchyBuilder _builder;

        public PlainListContent(IViewHierarchyBuilder builder)
        {
            Assert.Argument(builder, nameof(builder)).NotNull();
            Builder = builder;
        }

        public IViewHierarchyBuilder Builder { get; private set; }

        public sealed override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                var typeId = GetItemViewType(position);
                var view = CreateView(position, Builder);
                convertView = view.PlatformView;
            }
            _item.PlatformView = convertView;
            FillView(position, _item);
            return _item.PlatformView;
        }

        #region Corss platform interface
        //Override keywords below just for clarity, to have all methods listed in one place
        public override abstract object this[int position] { get; }
        public override abstract int Count { get; }
        public override abstract long GetItemId(int position);
        public override abstract int ViewTypeCount { get; }
        public override abstract int GetItemViewType(int position);
        public abstract QView CreateView(int position, IViewHierarchyBuilder builder);
        public abstract void FillView(int pos, QView convertView);
        #endregion
    }
}
