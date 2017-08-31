using Android.Widget;
using Android.Views;
using Qoden.UI.Wrappers;

namespace Qoden.UI
{
    public abstract class PlainListContent : BaseAdapter<object>, IPlainListContent
    {
        #region Cross platform API

        public abstract int NumberOfRows();

        public abstract int GetCellType(int row);

        public abstract int CellTypeCount { get; }
        
        public abstract TableViewCell GetCell(PlainListCellContext context);

        #endregion
        
        #region Redirects from Android API to IPlainListContent methods

        public sealed override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            var context = new PlainListCellContext()
            {
                ReusableCell = convertView.AsCell(),
                Row = position,
                Parent = parent.AsView()
            };
            return GetCell(context);
        }

        public sealed override int ViewTypeCount => CellTypeCount;

        public sealed override int GetItemViewType(int position)
        {
            return GetCellType(position);
        }

        public sealed override int Count => NumberOfRows();

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
