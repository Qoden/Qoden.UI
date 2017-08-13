using CoreGraphics;
using Foundation;
using Qoden.UI;
using UIKit;

namespace Qoden.UI
{
    public partial class QListView : BaseView<UITableView>
    {
#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
        static QListView()
        {
            if (LinkerTrick.False)
            {
                new UITableView();
            }
        }
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'    
    }

    public partial class QGroupedListView : BaseView<UITableView>
    {
#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
        static QGroupedListView()
        {
            if (LinkerTrick.False)
            {
                new UITableView(CGRect.Empty, UITableViewStyle.Plain);
            }
        }
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'  

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
            if (adapter is NSObject)
            {
                view.WeakDelegate = (NSObject)adapter;
            }
        }

        public static void SetContent(this IQView<UITableView> view, IUITableViewDataSource adapter)
        {
            view.PlatformView.SetContent(adapter);
        }

        public static void SetHeaderView(this UITableView view, UIView header)
        {
            view.TableHeaderView = header;            
        }

        public static void SetFooterView(this UITableView view, UIView header)
        {
            view.TableFooterView = header;
        }
    }
}
