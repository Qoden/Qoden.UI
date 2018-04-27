using Android.Content;
using Android.Widget;
using Android.Graphics.Drawables;


namespace Qoden.UI
{
    public class ActivityIndicatorView : QodenView
    {
        ProgressBar ActivityIndicator { get; }

        public ActivityIndicatorView(Context context) : base(context)
        {
            ActivityIndicator = Builder.Add<ProgressBar>();
            ActivityIndicator.Indeterminate = true;
        }

        protected override void OnLayout(LayoutBuilder layout)
        {
            base.OnLayout(layout);
            layout.Padding = new EdgeInsets(0, 20.Dp(), 0, 20.Dp());
            var indicator = layout.View(ActivityIndicator)
                                  .CenterHorizontally()
                                  .CenterVertically()
                                  .AutoSize();
            layout.SetPreferredHeight(indicator.Height, true);
        }

        bool _dimBackground;

        public bool DimBackground
        {
            get => _dimBackground;
            set
            {
                if (_dimBackground != value)
                {
                    _dimBackground = value;
                    Android.Graphics.Color bgColor;
                    switch(Background)
                    {
                        case PaintDrawable paintDrawable: bgColor = paintDrawable.Paint.Color; break;
                        case ColorDrawable colorDrawable: bgColor = colorDrawable.Color; break;
                        default: bgColor = Android.Graphics.Color.Black; break;
                    }
                    bgColor.A = _dimBackground ? (byte)105 : (byte)255;
                    var bg = new ColorDrawable(bgColor);
                    Background = bg;
                }
            }
        }
    }
}