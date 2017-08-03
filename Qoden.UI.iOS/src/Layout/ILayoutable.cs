using UIKit;

namespace Qoden.UI
{
    public interface ILayoutable
    {
        void OnLayout(LayoutBuilder layout);
        UIEdgeInsets LayoutMargins { get; }
    }
}
