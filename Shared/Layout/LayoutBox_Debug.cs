using System;
namespace Qoden.UI
{
    public static class LayoutBox_Debug
    {
        public static T Debug<T>(this T box) where T : ILayoutBox
        {
            System.Diagnostics.Debug.WriteLine(box);
            return box;
        }
    }
}
