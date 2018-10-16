using Qoden.UI.Wrappers;
using UIKit;

namespace Qoden.UI
{
    public class ActivityIndicatorView : QodenView
    {
        public UIActivityIndicatorView ActivityIndicator { get; }

        public ActivityIndicatorView()
        {
            ActivityIndicator = Builder.Add<UIActivityIndicatorView>();
            ActivityIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
            this.AsView().SetBackgroundColor(new RGB(0, 0, 0, 0));
            DimBackground = true;
            ActivityIndicator.StartAnimating();
            this.AsView().SetVisible(false);
        }

        protected override void OnLayout(LayoutBuilder layout)
        {
            base.OnLayout(layout);
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
                    BackgroundColor = BackgroundColor.ColorWithAlpha(_dimBackground ? 0.4f : 0);
                }
            }
        }
    }
}