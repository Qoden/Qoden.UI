﻿using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Qoden.UI
{
    public static class TableViewUtil
    {
        public static readonly Selector ViewForHeaderInSection = new Selector("tableView:viewForHeaderInSection:");

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
                return (UITableViewCell) cellView;
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
                return (UITableViewHeaderFooterView) cellView;
            }
        }

        public static nfloat DefaultEstimatedHeightForHeader(this IKeepLastSection content, UITableView tableView,
            nint section, float defaultHeight = 0.01f)
        {
            var header = content.LastSectionView;
            if (section != content.LastSection || header == null)
            {
                header = content.GetViewForHeader(tableView, section);
            }
            return header?.Frame.Height ?? (nfloat)defaultHeight;
        }
        
        public static nfloat DefaultEstimatedHeightForFooter(this IKeepLastSection content, UITableView tableView,
            nint section)
        {
            var header = content.LastSectionView;
            if (section != content.LastSection || header == null)
            {
                header = content.GetViewForFooter(tableView, section);
            }
            return header.Frame.Height;
        }


        public static nfloat DefaultGetHeightForRow(this IKeepLastCell content, UITableView tableView,
            NSIndexPath indexPath)
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
                return (UITableViewCell) Activator.CreateInstance(cellType, UITableViewCellStyle.Default,
                    cellTypeId.ToString());
            }
            if (typeof(UITableViewCell).IsAssignableFrom(cellType))
            {
                return (UITableViewCell) Activator.CreateInstance(cellType, cellTypeId.ToString());
            }
            return (UIView) Activator.CreateInstance(cellType);
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

    internal sealed class UITableViewHeaderFooterViewAdapter : UITableViewHeaderFooterView
    {
        public UIView View { get; }

        public UITableViewHeaderFooterViewAdapter(UIView cellView, string reuseId) : base(new NSString(reuseId))
        {
            View = cellView;
            AddSubview(cellView);
        }

        public sealed override CGSize SizeThatFits(CGSize size)
        {
            return View.SizeThatFits((SizeF) size);
        }

        public override void LayoutSubviews()
        {
            View.Frame = (RectangleF) Bounds;
        }
    }
}