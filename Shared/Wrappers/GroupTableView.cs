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
#endif
#if __ANDROID__
            get => PlatformView.ExpandableListAdapter;
#endif
        }

        public void SetDataSource(PlatformTableViewContent value)
        {
#if __IOS__
            PlatformView.DataSource = value;
            if (value is NSObject)
            {
                PlatformView.WeakDelegate = (NSObject)value;
            }
#endif
#if __ANDROID__
            PlatformView.SetAdapter(value);
#endif
        }
    }

    public static class GroupedTableViewExtensions
    {
        public static GroupTableView GroupedTableView(this ViewBuilder b, bool addSubview = true)
        {
#if __IOS__
            var view = new GroupTableView() { PlatformView = new PlatformGroupTableView(CGRect.Empty, UITableViewStyle.Grouped) };
#endif
#if __ANDROID__
            var view = new GroupTableView() { PlatformView = new PlatformGroupTableView(b.Context) };
#endif
            if (addSubview) b.AddSubview(view.PlatformView);
            return view;
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
