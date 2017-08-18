using System;
using Android.Widget;
using Android.Views;
using Qoden.Validation;

namespace Qoden.UI
{
    public abstract partial class PlainListContent : BaseAdapter<object>, IPlainListContent
    {
        public PlainListContent(ViewBuilder builder)
        {
            Builder = builder;
        }

        public ViewBuilder Builder { get; private set; }

        #region Redirects from iOS API to IPlainListContent methods

        public sealed override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            var context = new PlainListCellContext()
            {
                IsFresh = false,
                CellView = convertView,
                Row = position,
            };

            if (convertView == null)
            {
                var childTypeId = GetCellType(context.Row);
                CreateCell(childTypeId, ref context);
                Assert.State(context.CellView, "CellView").NotNull();
                context.IsFresh = true;
            }
            GetCell(context);
            return context.CellView;
        }

        public sealed override int ViewTypeCount => CellTypes.Length;

        public sealed override int GetItemViewType(int position)
        {
            return GetCellType(position);
        }

        public sealed override int Count => NumberOfRows();

        #endregion

        #region Internal API overrides

        object[] ActivatorArgs = new object[1];

        protected virtual void CreateCell(int cellTypeId, ref PlainListCellContext cellContext)
        {
            var childViewType = CellTypes[cellTypeId];
            ActivatorArgs[0] = Builder.Context;
            var convertView = (Android.Views.View)Activator.CreateInstance(childViewType, ActivatorArgs);
            cellContext.CellView = convertView;
        }

        #endregion


        #region Convenience overrides

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override bool HasStableIds { get { return true; } }

        #endregion
    }
}
