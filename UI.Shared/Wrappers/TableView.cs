using System;

namespace Qoden.UI.Wrappers
{
#if __IOS__
    using Foundation;
    using PlatformView = UIKit.UIView;
    using PlatformTableView = UIKit.UITableView;
    using PlatformTableViewContent = UIKit.IUITableViewDataSource;
#endif
#if __ANDROID__
    using Android.Widget;
    using PlatformView = Android.Views.View;
    using PlatformTableView = Android.Widget.ListView;
    using PlatformTableViewContent = Android.Widget.IListAdapter;
#endif

    public struct TableView
    {
        public static implicit operator PlatformTableView(TableView area) { return area.PlatformView; }
        public PlatformTableView PlatformView { get; set; }
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
            get => PlatformView.Adapter;
            set => PlatformView.Adapter = value;
#endif
        }

        public void AddHeaderView(View view)
        {
#if __IOS__
            if (PlatformView.TableHeaderView != null) throw new InvalidOperationException("Cannot add more than one header");
            PlatformView.TableHeaderView = view;
#endif
#if __ANDROID__
            PlatformView.AddHeaderView(view);
#endif
        }

        public void AddFooterView(View view)
        {
#if __IOS__
            if (PlatformView.TableFooterView != null) throw new InvalidOperationException("Cannot add more than one footer");
            PlatformView.TableFooterView = view;
#endif
#if __ANDROID__
            PlatformView.AddFooterView(view);
#endif
        }

        public void ReloadData()
        {
#if __IOS__
            PlatformView.ReloadData();
#endif
#if __ANDROID__
            if (PlatformView.Adapter == null) return;
            var adapter = PlatformView.Adapter as BaseAdapter;
            if (adapter == null) throw new InvalidOperationException("Cannot reload table view");
            adapter.NotifyDataSetChanged();
#endif
        }
    }

    public static class TableViewExtensions
    {
        public static TableView TableView(this ViewBuilder b)
        {
#if __IOS__
            return new TableView() { PlatformView = b.AddSubview(new PlatformTableView()) };
#endif
#if __ANDROID__
            return new TableView() { PlatformView = b.AddSubview(new PlatformTableView(b.Context)) };
#endif
        }

        /// <summary>
        /// Wrap convert platform view with cross platform wrapper
        /// </summary>
        public static TableView AsTableView(this PlatformTableView view)
        {
            return new TableView() { PlatformView = view };
        }
    }
}
