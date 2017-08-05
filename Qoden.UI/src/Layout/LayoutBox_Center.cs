#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    public static class LayoutBox_Center
    {
        public static T CenterHorizontally<T>(this T box, Pixel dx) where T : ILayoutBox
        {
            box.CenterX = (Pixel.Val(box.OuterBounds.Left + box.OuterBounds.Width / 2 + dx.Value)).Value;
            return box;
        }

        public static T CenterHorizontally<T>(this T box, float dx = 0) where T : ILayoutBox
        {
            return box.CenterHorizontally(box.Unit.ToPixels(dx));
        }

        public static T CenterVertically<T>(this T box, Pixel dx) where T : ILayoutBox
        {
            box.CenterY = (Pixel.Val(box.OuterBounds.Top + box.OuterBounds.Height / 2 + dx.Value)).Value;
            return box;
        }

        public static T CenterVertically<T>(this T box, float dx = 0) where T : ILayoutBox
        {
            return box.CenterVertically(box.Unit.ToPixels(dx));
        }
    }
}

