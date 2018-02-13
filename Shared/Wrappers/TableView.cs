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
#endif
#if __ANDROID__
            get => PlatformView.Adapter;
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
            PlatformView.Adapter = value;
#endif
        }

        public void AddHeaderView(View view)
        {
#if __IOS__
            if (PlatformView.TableHeaderView != null) throw new InvalidOperationException("Cannot add more than one header");
            PlatformView.TableHeaderView = view;
#endif
#if __ANDROID__
            if (PlatformView.HeaderViewsCount > 0) throw new InvalidOperationException("Cannot add more than one header");
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
            if (PlatformView.FooterViewsCount > 0) throw new InvalidOperationException("Cannot add more than one footer");
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
            if (adapter == null)
            {
                var wrappingAdapter = PlatformView.Adapter as HeaderViewListAdapter;
                adapter = wrappingAdapter?.WrappedAdapter as BaseAdapter;
            }
            if (adapter == null) throw new InvalidOperationException("Cannot reload table view");
            adapter.NotifyDataSetChanged();
#endif
        }
    }

    public static class TableViewExtensions
    {
        public static TableView TableView(this ViewBuilder b, bool addSubview = true)
        {
#if __IOS__
            var view = new TableView() { PlatformView = new PlatformTableView() };
#endif
#if __ANDROID__
            var view = new TableView() { PlatformView = new PlatformTableView(b.Context) };
#endif
            if (addSubview) b.AddSubview(view.PlatformView);
            return view;
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
