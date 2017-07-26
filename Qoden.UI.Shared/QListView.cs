using System;
namespace Qoden.UI
{
#if __IOS__
    using ListView = UIKit.UITableView;
    using ExpandableListView = UIKit.UITableView;
#endif
#if __ANDROID__
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
    }
}
