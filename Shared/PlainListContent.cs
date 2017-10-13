using System;
using Qoden.UI.Wrappers;

namespace Qoden.UI
{
#if __IOS__
    using UIKit;
    using PlatformView = UIKit.UITableViewCell;
    using PlatformCellView = UIKit.UITableViewCell;
#endif
#if __ANDROID__
using Android.Views;
using PlatformView = Android.Views.View;
using PlatformCellView = Android.Views.View;
#endif
    
    public struct PlainListCellContext
    {
        public PlatformCellView ReusableCell;
        public int Row;
#if __ANDROID__
        public View Parent;
#endif
        public bool IsFresh => ReusableCell == null;
    }
    public interface IPlainListContent
    {
        /// <summary>
        /// Return numbers rows in list view.
        /// </summary>
        int NumberOfRows();
        
        /// <summary>
        /// Get index of a cell type in <see cref="CellTypes"/> array for given section and cell index.
        /// </summary>
        int GetCellType(int row);
        
        /// <summary>
        /// Get number of different cell types in a list.
        /// </summary>
        int CellTypeCount { get; }
        
        /// <summary>
        /// Populate cell view with data.
        /// </summary>
        TableViewCell GetCell(PlainListCellContext context);
    }
}
