using System;
namespace Qoden.UI
{
#if __IOS__
    using PlatformView = UIKit.UIView;
#endif
#if __ANDROID__
    using PlatformView = Android.Views.View;
#endif

    public interface IPlainListContent 
    {
        /// <summary>
        /// Return numbers rows in list view.
        /// </summary>
        int NumberOfRows();
        /// <summary>
        /// Get all possible types of cells. This list might have dublicates.
        /// </summary>
        Type[] CellTypes { get; }
        /// <summary>
        /// Get index of a cell type in <see cref="CellTypes"/> array for given section and cell index.
        /// </summary>
        int GetCellType(int row);
        /// <summary>
        /// Populate cell view with data.
        /// </summary>
        void GetCell(PlainListCellContext cellContext);
    }

    public struct PlainListCellContext
    {
        public bool IsFresh;
        public PlatformView CellView;
        public int Row;
    }

    public abstract partial class PlainListContent : IPlainListContent
    {
        
#if __IOS__
        static readonly Type[] _CellTypes = { typeof(UIKit.UITableViewCell) };
#endif
#if __ANDROID__
        static readonly Type[] _CellTypes = { typeof(Android.Views.View) };
#endif
        #region Cross platform inteface

        public virtual Type[] CellTypes => _CellTypes;
        public virtual int GetCellType(int row)
        {
            return 0;
        }

        public abstract int NumberOfRows();
        public abstract void GetCell(PlainListCellContext cellContext);

        #endregion
    }
}
