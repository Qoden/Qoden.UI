using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Widget;

namespace Qoden.UI
{
    public partial class QProgressBar
    {        
    }

    public static partial class QProgressBar_Extensions
    {
        public static void SetProgress(this ProgressBar view, float progress)
        {
            view.SetProgress(progress);
        }

        public static float GetProgress(this ProgressBar view)
        {
            return view.Progress;
        }

        public static void SetProgressTint(this ProgressBar view, RGB color)
        {
            view.ProgressDrawable = new ColorDrawable(color.ToColor());
        }

        public static void SetTrackTint(this ProgressBar view, RGB color)
        {
            view.Background = new ColorDrawable(color.ToColor());
        }
    }
}
