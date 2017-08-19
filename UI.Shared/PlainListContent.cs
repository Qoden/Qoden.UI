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
        #region Cross platform inteface

        public abstract int NumberOfRows();
        public abstract Type[] CellTypes { get; }
        public abstract int GetCellType(int row);
        public abstract void GetCell(PlainListCellContext cellContext);

        #endregion
    }
}
