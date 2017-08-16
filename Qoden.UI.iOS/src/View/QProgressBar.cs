using System;
using UIKit;

namespace Qoden.UI
{
    public partial class QProgressBar
    {
    }

    public static partial class QProgressBar_Extensions
    {
        public static void SetProgress(this UIProgressView view, float progress)
        {
            view.Progress = progress;
        }

        public static float GetProgress(this UIProgressView view)
        {
            return view.Progress;
        }

        public static void SetProgressTint(this UIProgressView view, RGB color)
        {
            view.ProgressTintColor = color.ToColor();
        }

        public static void SetTrackTint(this UIProgressView view, RGB color)
        {
            view.TrackTintColor = color.ToColor();
        }
    }
}
