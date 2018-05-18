using System;
namespace Qoden.UI
{
    public class VisibilityChangeEventArgs : EventArgs
    {
        public bool IsVisible { get; set; }

        public VisibilityChangeEventArgs(bool visible)
        {
            IsVisible = visible;
        }

        public static VisibilityChangeEventArgs WithValue(bool visible)
        {
            return new VisibilityChangeEventArgs(visible);
        }
    }
}
