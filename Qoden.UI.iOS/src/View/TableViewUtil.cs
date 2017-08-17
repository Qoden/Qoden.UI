using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Qoden.UI
{
    public static class TableViewUtil
    {
        public static UITableViewCell ToTableViewCell(UIView view, int cellTypeId)
        {
            var cellView = view;
            if (!(cellView is UITableViewCell))
            {
                return new UITableViewCellAdapter(cellView, cellTypeId.ToString());
            }
            else
            {
                return (UITableViewCell)cellView;
            }
        }

        internal static UITableViewHeaderFooterView ToTableViewHeaderFooter(UIView view, int sectionTypeId)
        {
            var cellView = view;
            if (!(cellView is UITableViewHeaderFooterView))
            {
                return new UITableViewHeaderFooterViewAdapter(cellView, sectionTypeId.ToString());
            }
            else
            {
                return (UITableViewHeaderFooterView)cellView;
            }
        }

        public static nfloat DefaultGetHeightForRow<T>(this T content, UITableView tableView, NSIndexPath indexPath)
            where T : IUITableViewDataSource, IKeepLastCell
        {
            UITableViewCell cell;
            if (indexPath.Equals(content.LastIndexPath))
            {
                cell = content.LastCell;
            }
            else
            {
                cell = content.GetCell(tableView, indexPath);
            }
            return cell.Bounds.Height;
        }

        public static UIView CreateView(int cellTypeId, Type[] cellTypes, IViewHierarchyBuilder builder)
        {
            var cellType = cellTypes[cellTypeId];
            if (typeof(UITableViewCell).IsAssignableFrom(cellType))
            {
                return (UITableViewCell)Activator.CreateInstance(cellType, new[] { cellTypeId.ToString() });
            }
            else
            {
                return (UIView)builder.MakeView(cellType);
            }
        }
    }

    internal class UITableViewCellAdapter : UITableViewCell
    {
        public UIView CellView { get; private set; }

        public UITableViewCellAdapter(UIView cellView, string reuseId) : base(UITableViewCellStyle.Default, reuseId)
        {
            CellView = cellView;
            ContentView.AddSubview(CellView);
            BackgroundColor = CellView.BackgroundColor;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (CellView != null)
            {
                CellView.Frame = ContentView.Bounds;
            }
        }

        public override CoreGraphics.CGSize SizeThatFits(CoreGraphics.CGSize size)
        {
            return CellView?.SizeThatFits(size) ?? ContentView.SizeThatFits(size);
        }
    }

    internal class UITableViewHeaderFooterViewAdapter : UITableViewHeaderFooterView
    {
        QView _view;
        public UIView View => _view.PlatformView;

        public UITableViewHeaderFooterViewAdapter(UIView cellView, string reuseId) : base(new NSString(reuseId))
        {
            _view = new QView(cellView);
            AddSubview(cellView);
        }

        public sealed override CGSize SizeThatFits(CGSize size)
        {
            return _view.SizeThatFits((SizeF)size);
        }

        public override void LayoutSubviews()
        {
            _view.Frame = (RectangleF)Bounds;
        }
    }
}
