using Qoden.UI.Wrappers;

namespace Qoden.UI
{
#if __IOS__
    using PlatformView = UIKit.UIView;
    using PlatformCellView = UIKit.UITableViewCell;
#endif
#if __ANDROID__
    using PlatformView = Android.Views.View;
    using PlatformCellView = Android.Views.View;
#endif

    public struct GroupedListSectionContext
    {
        public int Section;
        public PlatformView SectionHeaderView;
#if __ANDROID__
        public bool IsExpanded;
        public PlatformView Parent;
#endif
        public bool IsFresh => SectionHeaderView == null;
    }
    
    public struct GroupedListCellContext
    {
        public int Section;
        public int Row;
        public PlatformCellView ReusableCell;
#if __ANDROID__
        public bool IsExpanded;
        public View Parent;
#endif
        public bool IsFresh => ReusableCell == null;
    }
    
    public interface INewGroupedListContent
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
        /// Get number of types of section headers.
        /// </summary>
        int SectionTypeCount { get; }
        /// <summary>
        /// Get number of possible types of cells.
        /// </summary>
        int CellTypeCount { get; }
        /// <summary>
        /// Get cell type.
        /// </summary>
        int GetCellType(int section, int row);
        /// <summary>
        /// Get section view type.
        /// </summary>
        int GetSectionType(int section);
        /// <summary>
        /// Get section view.
        /// </summary>
        View GetSection(GroupedListSectionContext context);
        /// <summary>
        /// Get cell.
        /// </summary>
        TableViewCell GetCell(GroupedListCellContext context);
    }
    
    public static class GroupedListContentExtensions
    {
        public static int GetCellTypeFromContext(this INewGroupedListContent content, GroupedListCellContext context)
        {
            return content.GetCellType(context.Section, context.Row);
        }
        
        public static int GetSectionTypeFromContext(this INewGroupedListContent content, GroupedListSectionContext context)
        {
            return content.GetSectionType(context.Section);
        }
    }
}
