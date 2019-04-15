using Android.Content;
using Android.Graphics;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Qoden.UI.Wrappers;
using ActionBar = Android.Support.V7.App.ActionBar;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Qoden.UI
{
    public sealed class CustomViewToolbar : Toolbar
    {
        public CustomViewToolbar(Context context, GravityFlags gravity) : base(context)
        {
            Gravity = gravity;

            CustomViewWrapper = new FrameLayout(context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            };

            TitleView = new TextView(context)
            {
                Gravity = GravityFlags.CenterVertical | GravityFlags.CenterHorizontal
            }.AsLabel();
            TextViewCompat.SetTextAppearance(TitleView, Android.Resource.Style.TextAppearanceDeviceDefaultWidgetActionBarTitle);

            ResetCustomViews();
            AddView(CustomViewWrapper, new ActionBar.LayoutParams(
                                           ViewGroup.LayoutParams.WrapContent,
                                           ViewGroup.LayoutParams.MatchParent,
                                           (int) Gravity));
        }

        public TextView TitleView { get; }

        public override ICharSequence TitleFormatted
        {
            get => TitleView.TextFormatted;
            set => TitleView.TextFormatted = value;
        }

        private FrameLayout CustomViewWrapper { get; }
        private GravityFlags Gravity { get; }

        public void SetCustomView(Wrappers.View view)
        {
            CustomViewWrapper.RemoveAllViews();
            CustomViewWrapper.AddView(view, new ActionBar.LayoutParams(
                                                ViewGroup.LayoutParams.WrapContent,
                                                ViewGroup.LayoutParams.MatchParent,
                                                (int) Gravity));
        }

        public void ResetCustomViews()
        {
            CustomViewWrapper.RemoveAllViews();
            CustomViewWrapper.AddView(TitleView);
        }

        public override void SetTitleTextColor(int color)
        {
            TitleView.SetTextColor(new Color(color));
        }
    }
}
