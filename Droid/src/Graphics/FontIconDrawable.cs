using System;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace Qoden.UI
{
    public class FontIconDrawable : Drawable
    {
        string _icon;
        Android.Text.TextPaint _paint;

        public FontIconDrawable(string icon, Typeface typeface)
        {
            _icon = icon;
            _paint = new Android.Text.TextPaint();
            _paint.SetTypeface(typeface);
            _paint.SetStyle(Paint.Style.Stroke);
            _paint.TextAlign = Paint.Align.Center;
            _paint.UnderlineText = false;
            _paint.Color = Android.Graphics.Color.White;
            _paint.AntiAlias = true;
            _paint.Alpha = 255;
        }

        public Pixel Size { get; set; }

        public RGB Color
        {
            get => _paint.Color.ToRGB();
            set => _paint.Color = value.ToColor();
        }

        const int PixelFormat_OPAQUE = -1;
        public override int Opacity => PixelFormat_OPAQUE;

        public override int IntrinsicWidth
        {
            get
            {
                return Size.IntValue;
            }
        }

        public override int IntrinsicHeight
        {
            get
            {
                return Size.IntValue;
            }
        }

        public override void Draw(Canvas canvas)
        {
            var bounds = Bounds;
            var size = Size.IntValue;
            if (bounds.Width() != size || bounds.Height() != size)
            {
                var left = (int) ((bounds.Width() - size) / 2f);
                var top = (int) ((bounds.Height() - size) / 2f);
                SetBounds(left, top, left + size, top + size);
                bounds = Bounds;
            }

            _paint.TextSize = bounds.Height();
            using (var textBounds = new Rect())
            {
                _paint.GetTextBounds(_icon, 0, 1, textBounds);
                float textBottom = textBounds.Height();
                canvas.DrawText(_icon, bounds.CenterX(), bounds.Bottom, _paint);
            }
        }

        public override bool IsStateful => true;

        public override bool SetState(int[] stateSet)
        {
            var oldValue = _paint.Alpha;
            var newValue = IsEnabled(stateSet) ? oldValue : (int)Math.Round(oldValue / 2.0f);
            _paint.Alpha = newValue;
            return oldValue != newValue;
        }

        public static bool IsEnabled(int[] stateSet)
        {
            foreach (var state in stateSet)
                if (state == Android.Resource.Attribute.StateEnabled)
                    return true;
            return false;
        }

        public override void SetAlpha(int alpha)
        {
            _paint.Alpha = alpha;
        }

        public override void SetColorFilter(ColorFilter colorFilter)
        {
            _paint.SetColorFilter(colorFilter);
        }

        public override void ClearColorFilter()
        {
            _paint.SetColorFilter(null);
        }

        public void SetStyle(Paint.Style style)
        {
            _paint.SetStyle(style);
        }
    }
}
