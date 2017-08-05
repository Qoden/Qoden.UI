#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    public static class LayoutBox_MinMax
    {
        public static T MinWidth<T>(this T box, float mw) where T : ILayoutBox
        {
            return box.MinWidth(box.Unit.ToPixels(mw));
        }

        public static T MinHeight<T>(this T box, float mh) where T : ILayoutBox
        {
            return box.MinHeight(box.Unit.ToPixels(mh));
        }

        public static T MinWidth<T>(this T box, Pixel mw) where T : ILayoutBox
        {
            if (box.FrameWidth < mw.Value)
            {
                box.SetWidth(mw);
            }
            return box;
        }

        public static T MinHeight<T>(this T box, Pixel mh) where T : ILayoutBox
        {
            if (box.FrameHeight < mh.Value)
            {
                box.SetHeight(mh);
            }

            return box;
        }

        public static T MaxWidth<T>(this T box, float mw) where T : ILayoutBox
        {
            return box.MaxWidth(box.Unit.ToPixels(mw));
        }

        public static T MaxHeight<T>(this T box, float mh) where T : ILayoutBox
        {
            return box.MaxHeight(box.Unit.ToPixels(mh));
        }

        public static T MaxWidth<T>(this T box, Pixel mw) where T : ILayoutBox
        {
            if (box.FrameWidth > mw.Value)
            {
                box.SetWidth(mw);
            }
            return box;
        }

        public static T MaxHeight<T>(this T box, Pixel mh) where T : ILayoutBox
        {
            if (box.FrameHeight > mh.Value)
            {
                box.SetHeight(mh);
            }

            return box;
        }
    }
}

