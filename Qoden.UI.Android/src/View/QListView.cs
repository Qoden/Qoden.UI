using Android.Widget;

namespace Qoden.UI
{
    public partial class QListView : BaseView<ListView>
    {
    }

    public partial class QGroupedListView : BaseView<ExpandableListView>
    {
    }

    public static partial class QListView_Extensions
    {
        public static void SetContent(this ListView view, IListAdapter adapter)
        {
            view.Adapter = adapter;
        }

        public static void SetContent(this ExpandableListView view, IExpandableListAdapter adapter)
        {
            view.SetAdapter(adapter);
        }

        public static void SetContent(this IQView<ListView> view, IListAdapter adapter)
        {
            view.PlatformView.SetContent(adapter);
        }

        public static void SetContent(this IQView<ExpandableListView> view, IExpandableListAdapter adapter)
        {
            view.PlatformView.SetContent(adapter);
        }
    }
}
