using Android.Graphics;
using Android.Text;
using Android.Text.Style;

namespace Qoden.UI
{
    public static class SpannableStringExtensions
    {
        public static SpannableString WithFont(this string text, Font font)
        {
            var typeface = TypefaceCollection.Get(font.Name, font.Style);
            var fontSize = new AbsoluteSizeSpan((int) font.Size.Dp());

            var spannableString = new SpannableString(text);
            spannableString.SetSpan(new CustomTypefaceSpan(typeface), 0, spannableString.Length(), 0);
            spannableString.SetSpan(fontSize, 0, spannableString.Length(), 0);

            return spannableString;
        }

        public static SpannableString WithColor(this SpannableString spannableString, Color color)
        {
            spannableString.SetSpan(new ForegroundColorSpan(color), 0, spannableString.Length(), 0);
            return spannableString;
        }
    }
}