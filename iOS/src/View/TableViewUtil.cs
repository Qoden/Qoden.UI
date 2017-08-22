using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Qoden.UI
{
    public static class TableViewUtil
    {
        static TableViewUtil()
        {
            if (LinkerTrick.False)
            {
                // ReSharper disable once ObjectCreationAsStatement
                new UITableViewCell(UITableViewCellStyle.Default, "");
            }
        }
        
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

        public static UIView CreateView(int cellTypeId, Type[] cellTypes)
        {
            var cellType = cellTypes[cellTypeId];
            if (typeof(UITableViewCell) == cellType)
            {
                return (UITableViewCell)Activator.CreateInstance(cellType, UITableViewCellStyle.Default, cellTypeId.ToString());
            }
            if (typeof(UITableViewCell).IsAssignableFrom(cellType))
            {
                return (UITableViewCell)Activator.CreateInstance(cellType, cellTypeId.ToString());
            }
            return (UIView)Activator.CreateInstance(cellType);
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

        public override CGSize SizeThatFits(CGSize size)
        {
            return CellView?.SizeThatFits(size) ?? ContentView.SizeThatFits(size);
        }
    }

    internal class UITableViewHeaderFooterViewAdapter : UITableViewHeaderFooterView
    {
        UIView _view;
        public UIView View => _view;

        public UITableViewHeaderFooterViewAdapter(UIView cellView, string reuseId) : base(new NSString(reuseId))
        {
            _view = cellView;
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
