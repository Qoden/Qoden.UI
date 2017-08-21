using System;
#if __IOS__
    using PlatformProgressBar = UIKit.UIProgressView;
#endif
#if __ANDROID__
using Android.Graphics.Drawables;
using PlatformProgressBar = Android.Widget.ProgressBar;
#endif

namespace Qoden.UI.Wrappers
{
    public struct ProgressBar
    {
        public static implicit operator PlatformProgressBar(ProgressBar area) { return area.PlatformView; }

        public PlatformProgressBar PlatformView { get; set; }

        public View AsView() { return new View() { PlatformView = PlatformView }; }

        public float Progress
        {
            get => PlatformView.Progress;
        }

        public void SetProgress(float progress)
        {
#if __IOS__
            PlatformView.Progress = progress;
#endif
#if __ANDROID__
            PlatformView.Progress = (int)Math.Round(progress);
#endif
        }

        public void SetProgressTint(RGB color)
        {
#if __IOS__
            PlatformView.ProgressTintColor = color.ToColor();
#endif

#if __ANDROID__
            PlatformView.ProgressDrawable = new ColorDrawable(color.ToColor());
#endif
        }

        public void SetTrackTint(RGB color)
        {
#if __IOS__
            PlatformView.TrackTintColor = color.ToColor();
#endif
#if __ANDROID__
            PlatformView.Background = new ColorDrawable(color.ToColor());
#endif
        }
    }

    public static class ProgressBarExtensions
    {
        public static ProgressBar ProgressBar(this ViewBuilder b, bool addSubview = true)
        {
#if __IOS__
            var view = new ProgressBar() { PlatformView = new PlatformProgressBar() };
#endif
#if __ANDROID__
            var view = new ProgressBar() { PlatformView = new PlatformProgressBar(b.Context) };
#endif
            if (addSubview) b.AddSubview(view.PlatformView);
            return view;
        }

        public static ProgressBar AsProgressBar(this PlatformProgressBar label)
        {
            return new ProgressBar() { PlatformView = label };
        }

    }
}
