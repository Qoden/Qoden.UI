#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    public static class LayoutBox_LTRBWH
    {
        public static T Width<T>(this T box, float w) where T : ILayoutBox
        {
            box.SetWidth(w);
            return box;
        }

        public static T Width<T>(this T box, Pixel w) where T : ILayoutBox
        {
            box.Width = w.Value;
            return box;
        }

        public static T Height<T>(this T box, float h) where T : ILayoutBox
        {
            box.SetHeight(h);
            return box;
        }

        public static T Height<T>(this T box, Pixel h) where T : ILayoutBox
        {
            box.Height = h.Value;
            return box;
        }

        public static T Left<T>(this T box, float l) where T : ILayoutBox
        {
            box.SetLeft(l);
            return box;
        }

        public static T Left<T>(this T box, Pixel l) where T : ILayoutBox
        {
            box.MarginLeft = l.Value;
            return box;
        }

        public static T Right<T>(this T box, float r) where T : ILayoutBox
        {
            box.SetRight(r);
            return box;
        }

        public static T Right<T>(this T box, Pixel r) where T : ILayoutBox
        {
            box.MarginRight = r.Value;
            return box;
        }

        public static T Top<T>(this T box, float t) where T : ILayoutBox
        {
            box.SetTop(t);
            return box;
        }

        public static T Top<T>(this T box, Pixel t) where T : ILayoutBox
        {
            box.MarginTop = t.Value;
            return box;
        }

        public static T Bottom<T>(this T box, float b) where T : ILayoutBox
        {
            box.Bottom(box.Unit.ToPixels(b));
            return box;
        }

        public static T Bottom<T>(this T box, Pixel b) where T : ILayoutBox
        {
            box.MarginBottom = b.Value;
            return box;
        }
    }
}

