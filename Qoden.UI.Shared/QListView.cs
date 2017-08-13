using System;
namespace Qoden.UI
{
#if __IOS__
    using View = UIKit.UIView;
    using ListView = UIKit.UITableView;
    using ExpandableListView = UIKit.UITableView;
#endif
#if __ANDROID__
    using View = Android.Views.View;
    using ListView = Android.Widget.ListView;
    using ExpandableListView = Android.Widget.ExpandableListView;
#endif

    public partial class QListView
    {
        public QListView()
        {
        }

        public QListView(ListView target) : base(target)
        {
        }
    }

    public partial class QGroupedListView
    {
        public QGroupedListView()
        {
        }

        public QGroupedListView(ExpandableListView target) : base(target)
        {
        }
    }

    public static partial class QListView_Extensions
    {
        public static void SetFooterView(this IQView<ListView> view, View header)
        {
            view.PlatformView.SetFooterView(header);
        }

        public static void SetHeaderView(this IQView<ListView> view, View header)
        {
            view.PlatformView.SetHeaderView(header);
        }
    }
}
