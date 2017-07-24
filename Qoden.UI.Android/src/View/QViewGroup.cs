using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;

namespace Qoden.UI
{
    public class QViewGroup : BaseView<ViewGroup>
    {
        public QViewGroup()
        {
        }

        public QViewGroup(ViewGroup target) : base(target)
        {
        }
    }

    public static class ViewGroupExtensions
    {
        public static IEnumerable<View> Subviews(this IQView<ViewGroup> view)
        {
            return view.PlatformView.Subviews();
        }

        public static IEnumerable<View> Subviews(this ViewGroup view)
        {
            for (int i = 0; i < view.ChildCount; ++i)
            {
                yield return view.GetChildAt(i);
            }
        }
    }
}
