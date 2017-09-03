#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    public static class LayoutBoxMinMax
    {
        public static T MinWidth<T>(this T box, float mw) where T : ILayoutBox
        {
            if (box.Width < mw)
            {
                box.Width = mw;
            }
            return box;
        }

        public static T MinHeight<T>(this T box, float mh) where T : ILayoutBox
        {
            if (box.Height < mh)
            {
                box.Height = mh;
            }

            return box;
        }

        public static T MaxWidth<T>(this T box, float mw) where T : ILayoutBox
        {
            if (box.Width > mw)
            {
                box.Width = mw;
            }
            return box;
        }

        public static T MaxHeight<T>(this T box, float mh) where T : ILayoutBox
        {
            if (box.Height > mh)
            {
                box.Height = mh;
            }

            return box;
        }
    }
}

