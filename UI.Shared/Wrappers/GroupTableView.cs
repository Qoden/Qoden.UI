namespace Qoden.UI.Wrappers
{
#if __IOS__
    using Foundation;
    using PlatformView = UIKit.UIView;
    using PlatformGroupTableView = UIKit.UITableView;
    using PlatformTableViewContent = UIKit.IUITableViewDataSource;
    using CoreGraphics;
    using UIKit;
#endif
#if __ANDROID__
    using PlatformView = Android.Views.View;
    using PlatformGroupTableView = Android.Widget.ExpandableListView;
    using PlatformTableViewContent = Android.Widget.IExpandableListAdapter;
#endif

    public struct GroupTableView
    {
        public static implicit operator PlatformGroupTableView(GroupTableView area) { return area.PlatformView; }
        public PlatformGroupTableView PlatformView { get; set; }

        public TableView AsTableView() { return new TableView() { PlatformView = PlatformView }; }
        public View AsView() { return new View() { PlatformView = PlatformView }; }

        public PlatformTableViewContent DataSource
        {
#if __IOS__
            get => PlatformView.DataSource;
            set 
            {
                PlatformView.DataSource = value;
                if (value is NSObject)
                {
                    PlatformView.WeakDelegate = (NSObject)value;
                }
            }
#endif
#if __ANDROID__
            get => PlatformView.ExpandableListAdapter;
            set => PlatformView.SetAdapter(value);
#endif
        }
    }

    public static class GroupedTableViewExtensions
    {
        public static GroupTableView GroupedTableView(this ViewBuilder b)
        {
#if __IOS__
            return new GroupTableView() { PlatformView = b.AddSubview(new PlatformGroupTableView(CGRect.Empty, UITableViewStyle.Grouped)) };
#endif
#if __ANDROID__
            return new GroupTableView() { PlatformView = b.AddSubview(new PlatformGroupTableView(b.Context)) };
#endif
        }

        /// <summary>
        /// Wrap convert platform view with cross platform wrapper
        /// </summary>
        public static GroupTableView AsGroupedTableView(this PlatformGroupTableView view)
        {
            return new GroupTableView() { PlatformView = view };
        }
    }
}
