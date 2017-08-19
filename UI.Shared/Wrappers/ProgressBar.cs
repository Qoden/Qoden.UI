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
#if __IOS__
            get => PlatformView.Progress;
            set => PlatformView.Progress = value;
#endif
#if __ANDROID__
            get => PlatformView.Progress;
            set => PlatformView.Progress = (int)value;
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
        public static ProgressBar ProgressBar(this ViewBuilder b)
        {
#if __IOS__
            return new ProgressBar() { PlatformView = b.AddSubview(new PlatformProgressBar()) };
#endif
#if __ANDROID__
            return new ProgressBar() { PlatformView = b.AddSubview(new PlatformProgressBar(b.Context)) };
#endif
        }

        public static ProgressBar AsProgressBar(this PlatformProgressBar label)
        {
            return new ProgressBar() { PlatformView = label };
        }

    }
}
