using CoreGraphics;
using Foundation;
using Qoden.UI;
using UIKit;

namespace Qoden.UI
{
    public partial class QListView : BaseView<UITableView>
    {
    }

    public partial class QGroupedListView : BaseView<UITableView>
    {
        public override UITableView Create(IViewHierarchyBuilder builder)
        {
            return new UITableView(CGRect.Empty, UITableViewStyle.Grouped);
        }
    }

    public static partial class QListView_Extensions
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
