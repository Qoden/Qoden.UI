using System;
using System.Drawing;
using UIKit;

namespace Qoden.UI
{
    internal class TableViewUtil
    {
        public static UITableViewCell ToTableViewCell(QView view)
        {
            var cellView = view.PlatformView;
            if (!(cellView is UITableViewCell))
            {
                return new UITableViewCellAdapter(cellView);
            }
            else
            {
                return (UITableViewCell)cellView;
            }
        }

        internal static UITableViewHeaderFooterView ToTableViewHeaderFooter(QView view)
        {
            var cellView = view.PlatformView;
            if (!(cellView is UITableViewHeaderFooterView))
            {
                return new UITableViewHeaderFooterViewAdapter(cellView);
            }
            else
            {
                return (UITableViewHeaderFooterView)cellView;
            }
        }
    }

    internal class UITableViewCellAdapter : UITableViewCell
    {
        QView _view;
        public UIView CellView => _view.PlatformView;

        public UITableViewCellAdapter(UIView cellView)
        {
            _view = new QView(cellView);
            ContentView.AddSubview(cellView);
            BackgroundColor = CellView.BackgroundColor;
        }

        public UITableViewCellAdapter(IQView<UIView> cellView) : this(cellView.PlatformView)
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (_view != null)
            {
                _view.MakeViewLayoutBox((RectangleF)ContentView.Bounds)
                    .Left(0).Right(0).Top(0).Bottom(0)
                    .Layout();
            }
        }
    }

    internal class UITableViewHeaderFooterViewAdapter : UITableViewHeaderFooterView
    {
        QView _view;
        public UIView View => _view.PlatformView;

        public UITableViewHeaderFooterViewAdapter(UIView cellView)
        {
            _view = new QView(cellView);
            AddSubview(cellView);
        }

        public override void LayoutSubviews()
        {
            if (_view != null)
            {
                _view.MakeViewLayoutBox((RectangleF)Bounds)
                    .Left(0).Right(0).Top(0).Bottom(0)
                    .Layout();
            }
        }
    }
}
