#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    public static class LayoutBoxCenter
    {
        public static T CenterHorizontally<T>(this T box, Pixel dx) where T : ILayoutBox
        {
            box.CenterX = (Pixel.Val(box.OuterBounds.Width / 2 + dx.Value)).Value;
            return box;
        }

        public static T CenterHorizontally<T>(this T box, float dx = 0) where T : ILayoutBox
        {
            return box.CenterHorizontally(box.Unit.ToPixels(dx));
        }

        public static T CenterVertically<T>(this T box, Pixel dx) where T : ILayoutBox
        {
            box.CenterY = (Pixel.Val(box.OuterBounds.Height / 2 + dx.Value)).Value;
            return box;
        }

        public static T CenterVertically<T>(this T box, float dx = 0) where T : ILayoutBox
        {
            return box.CenterVertically(box.Unit.ToPixels(dx));
        }

        public static T CenterX<T>(this T box, float centerX) where T : ILayoutBox
        {
            return box.CenterX(box.Unit.ToPixels(centerX));
        }
        
        public static T CenterX<T>(this T box, Pixel centerX) where T : ILayoutBox
        {
            box.CenterX = centerX.Value;
            return box;
        }
        
        public static T CenterY<T>(this T box, float centerY) where T : ILayoutBox
        {
            return box.CenterY(box.Unit.ToPixels(centerY));
        }
        
        public static T CenterY<T>(this T box, Pixel centerY) where T : ILayoutBox
        {
            box.CenterY = centerY.Value;
            return box;
        }

        
    }
}

