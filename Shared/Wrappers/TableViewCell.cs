using System;

namespace Qoden.UI.Wrappers
{
#if __IOS__
    using UIKit;
    using PlatformCellView = UIKit.UITableViewCell;
    using PlatformView = UIKit.UIView;

#endif
#if __ANDROID__
using Android.Views;
using PlatformCellView = Android.Views.View;
using PlatformView = Android.Views.View;
#endif

    public struct TableViewCell
    {
        public static implicit operator PlatformCellView(TableViewCell area)
        {
            return area.PlatformView;
        }

        public PlatformCellView PlatformView { get; set; }

        public TableViewCell(object view)
        {
            PlatformView = (PlatformCellView) view;
        }

        public View AsView()
        {
            return new View(PlatformView);
        }

        public static TableViewCell Create(int type)
        {
#if __IOS__
            var view = new TableViewCell()
            {
                PlatformView = new PlatformCellView(UITableViewCellStyle.Default, type.ToString())
            };
            view.PlatformView.SelectionStyle = UITableViewCellSelectionStyle.None;
#endif
#if __ANDROID__
            var view = new TableViewCell() { PlatformView = new PlatformView(b.Context) };
#endif
            return view;
        }
    }

    public static class TableViewCellExtensions
    {
        public static TableViewCell TableViewCell<T>(this ViewBuilder builder, int type)
        {
            return builder.TableViewCell(typeof(T), type);
        }

        public static TableViewCell TableViewCell(this ViewBuilder builder, Type cellType, int type)
        {
#if __IOS__
            var cell = Activator.CreateInstance(cellType, type.ToString());
#endif
#if __ANDROID__
            var cell = builder.Create(cellType);
            #endif
            return new TableViewCell(cell);
        }

        public static TableViewCell AsCell(this PlatformCellView view)
        {
            return new TableViewCell(view);
        }

        public static TableViewCell AsCell(this PlatformView view, int cellTypeId)
        {
#if __ANDROID__
            return new TableViewCell(view.PlatformView);
    #endif
#if __IOS__
            return new TableViewCell(TableViewUtil.ToTableViewCell(view, cellTypeId));
#endif
        }
        
        public static TableViewCell AsCell(this View view, int cellTypeId)
        {
            return view.PlatformView.AsCell(cellTypeId);
        }
    }
}