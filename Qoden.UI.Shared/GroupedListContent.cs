using System;
using System.Collections.Generic;
using Qoden.Validation;

namespace Qoden.UI
{
#if __IOS__
    using View = UIKit.UIView;
#endif
#if __ANDROID__
    using View = Android.Views.View;
#endif

    public interface IGroupedListContent
    {
        /// <summary>
        /// Return numbers sections in list view.
        /// </summary>
        int NumberOfSections();
        /// <summary>
        /// Return number of rows in section.
        /// </summary>
        int RowsInSection(int section);
        /// <summary>
        /// Get all possible types of section headers. This list might have dublicates.
        /// </summary>
        Type[] SectionTypes { get; }
        /// <summary>
        /// Get all possible types of cells. This list might have dublicates.
        /// </summary>
        Type[] CellTypes { get; }
        /// <summary>
        /// Get index of a cell type in <see cref="CellTypes"/> array for given section and cell index.
        /// </summary>
        int GetCellType(int section, int row);
        /// <summary>
        /// Get index of a section view type in <see cref="SectionTypes"/> array for given section index.
        /// </summary>
        int GetSectionType(int section);
        /// <summary>
        /// Populate section view with data.
        /// </summary>
        void GetSection(GroupedListSectionContext sectionContext);
        /// <summary>
        /// Populate cell view with data.
        /// </summary>
        void GetCell(GroupedListCellContext cellContext);
    }

    public struct GroupedListCellContext
    {
        public bool IsFresh;
        public View CellView;
        public int Section;
        public int Row;
    }

    public struct GroupedListSectionContext
    {
        public bool IsFresh;
        public View SectionHeaderView;
        public int Section;
    }

    public abstract partial class GroupedListContent
    {
#if __IOS__
        static Type[] _SectionTypes = { typeof(UIKit.UITableViewHeaderFooterView) };
        static Type[] _CellTypes = { typeof(UIKit.UITableViewCell) };
#endif
#if __ANDROID__
        static Type[] _SectionTypes = { typeof(Android.Views.View) };
        static Type[] _CellTypes = { typeof(Android.Views.View) };
#endif
        public virtual Type[] SectionTypes { get; } = _SectionTypes;
        public virtual Type[] CellTypes { get; } = _CellTypes;

        public virtual int GetCellType(int section, int childPosition)
        {
            return 0;
        }
        public virtual int GetSectionType(int section)
        {
            return 0;
        }
    }
}
