using UIKit;

namespace Qoden.UI
{
    public class ActivityIndicatorView : QodenView
    {
        UIActivityIndicatorView ActivityIndicator { get; }


        public ActivityIndicatorView()
        {
            ActivityIndicator = Builder.Add<UIActivityIndicatorView>();
            ActivityIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
            ActivityIndicator.StartAnimating();    
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
                    BackgroundColor = BackgroundColor.ColorWithAlpha(_dimBackground ? 0.4f : 1);
                }
            }
        }
    }
}