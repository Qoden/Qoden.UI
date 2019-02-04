using Android.Graphics;
using Android.Text;
using Android.Text.Style;

namespace Qoden.UI
{
    // Such class is required to apply custom typeface
    public class CustomTypefaceSpan : MetricAffectingSpan
    {
        private readonly Typeface _typeface;

        public CustomTypefaceSpan(Typeface typeface)
        {
            _typeface = typeface;
        }

        public override void UpdateDrawState(TextPaint tp)
        {
            ApplyCustomTypeface(tp, _typeface);
        }

        public override void UpdateMeasureState(TextPaint p)
        {
            ApplyCustomTypeface(p, _typeface);
        }

        private static void ApplyCustomTypeface(Paint paint, Typeface typeface)
        {
            paint.SetTypeface(typeface);
        }
    }
}
