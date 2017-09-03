#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    public static class LayoutBox_LTRBWH
    {
        public static T Width<T>(this T box, float w) where T : ILayoutBox
        {
            box.Width = w;
            return box;
        }

        public static T Height<T>(this T box, float h) where T : ILayoutBox
        {
            box.Height = h;
            return box;
        }

        public static T Left<T>(this T box, float l) where T : ILayoutBox
        {
            box.MarginLeft = l;
            return box;
        }

        public static T Right<T>(this T box, float r) where T : ILayoutBox
        {
            box.MarginRight = r;
            return box;
        }

        public static T Top<T>(this T box, float t) where T : ILayoutBox
        {
            box.MarginTop = t;
            return box;
        }

        public static T Bottom<T>(this T box, float b) where T : ILayoutBox
        {
            box.MarginBottom = b;
            return box;
        }
    }
}

