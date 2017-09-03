#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    public static class LayoutBoxCenter
    {
        public static T CenterHorizontally<T>(this T box, float dx = 0) where T : ILayoutBox
        {
            box.CenterX = box.OuterBounds.Width / 2 + dx;
            return box;
        }

        public static T CenterVertically<T>(this T box, float dx = 0) where T : ILayoutBox
        {
            box.CenterY = box.OuterBounds.Height / 2 + dx;
            return box;
        }
        
        public static T CenterX<T>(this T box, float centerX) where T : ILayoutBox
        {
            box.CenterX = centerX;
            return box;
        }
        
        public static T CenterY<T>(this T box, float centerY) where T : ILayoutBox
        {
            box.CenterY = centerY;
            return box;
        }
    }
}

