using System;
namespace Qoden.UI
{
#if __IOS__
    using ProgressBar = UIKit.UIProgressView;
#endif
#if __ANDROID__
    using ProgressBar = Android.Widget.ProgressBar;
#endif

    public partial class QProgressBar : BaseView<ProgressBar>
    {
        public QProgressBar(ProgressBar view) : base(view) { }

        public QProgressBar() { }
    }

    public static partial class QProgressBar_Extensions
    {
        public static void SetProgress(this IQView<ProgressBar> view, float progress)
        {
            view.PlatformView.SetProgress(progress);
        }

        public static float GetProgress(this IQView<ProgressBar> view)
        {
            return view.PlatformView.GetProgress();
        }

        public static void SetProgressTint(this IQView<ProgressBar> view, RGB color)
        {
            view.PlatformView.SetProgressTint(color);
        }

        public static void SetTrackTint(this IQView<ProgressBar> view, RGB color)
        {
            view.PlatformView.SetTrackTint(color);
        }
    }
}
