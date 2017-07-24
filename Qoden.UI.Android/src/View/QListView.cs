using System;
using Android.Views;
using Android.Widget;
using Qoden.Validation;

namespace Qoden.UI
{
    public class QListView : BaseView<ListView>
    {
        public QListView()
        {
        }

        public QListView(ListView target) : base(target)
        {
        }
    }

    public class QGroupedListView : BaseView<ExpandableListView>
    {
        public QGroupedListView()
        {
        }

        public QGroupedListView(ExpandableListView target) : base(target)
        {
        }
    }

    public static class ListView_Extensions
    {
        public static void SetContent(this ListView view, IListAdapter adapter)
        {
            view.Adapter = adapter;
        }

        public static void SetContent(this IQView<ListView> view, IListAdapter adapter)
        {
            view.PlatformView.SetContent(adapter);
        }

        public static void SetContent(this ExpandableListView view, IExpandableListAdapter adapter)
        {
            view.SetAdapter(adapter);
        }

        public static void SetContent(this IQView<ExpandableListView> view, IExpandableListAdapter adapter)
        {
            view.PlatformView.SetContent(adapter);
        }
    }
}
