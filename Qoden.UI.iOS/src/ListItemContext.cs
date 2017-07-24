using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace Qoden.UI
{
    public class ListItemContext<T>
    {
        public NSIndexPath IndexPath { get; internal set; }
        public int Position => IndexPath.Row;
        public UITableViewCell View
        {
            get
            {
                var id = Adapter.ItemViewType(Position);
                return Parent.DequeueReusableCell(id.ToString());
            }
        }
        public UITableView Parent { get; internal set; }

        public IList<T> DataSource { get; internal set; }
        public T Item => DataSource[Position];

        public IUITableViewBinding Adapter { get; internal set; }

        public UITableViewCell Result { get; set; }

        public TView CreateView<TView>() where TView : UITableViewCell, new()
        {
            return new TView();
        }
    }
}
