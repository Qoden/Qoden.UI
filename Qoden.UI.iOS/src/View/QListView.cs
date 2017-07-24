using CoreGraphics;
using Foundation;
using Qoden.UI;
using UIKit;

namespace Qoden.UI
{
    public class QListView : BaseView<UITableView>
    {
        public QListView()
        {
        }

        public QListView(UITableView uITableView) : base(uITableView)
        {
        }
    }

    public class QGroupedListView : BaseView<UITableView>
    {
        public override UITableView Create(IViewHierarchyBuilder builder)
        {
            return new UITableView(CGRect.Empty, UITableViewStyle.Grouped);
        }
    }



    public static class ListView_Extensions
    {
        public static void SetContent(this UITableView view, IUITableViewDataSource adapter)
        {
            view.DataSource = adapter;
            if (view.WeakDelegate == null && adapter is NSObject)
            {
                view.WeakDelegate = (NSObject)adapter;
            }
        }

        public static void SetContent(this IQView<UITableView> view, IUITableViewDataSource adapter)
        {
            view.PlatformView.SetContent(adapter);
        }
    }
}
