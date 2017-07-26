using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Qoden.UI;

namespace Example
{
    public partial class ProfileForm
    {
        public ProfileForm(Context ctx) : base(ctx){}

        private const int TopInset = 0;

        private void PlatformCreate()
        {
            FirstName.LayoutParameters.Width = LayoutParams.MatchParent;
            LastName.LayoutParameters.Width = LayoutParams.MatchParent;
            BirthDate.LayoutParameters.Width = LayoutParams.MatchParent;
            Email.LayoutParameters.Width = LayoutParams.MatchParent;
            PhoneNumber.LayoutParameters.Width = LayoutParams.MatchParent;
            NotificationSettings.LayoutParameters.Width = LayoutParams.MatchParent;
            ChangePassword.LayoutParameters.Width = LayoutParams.MatchParent;
            LogOut.LayoutParameters.Width = LayoutParams.MatchParent;

            Form.PlatformView.GroupClick += Form_GroupClick;
            Form.PlatformView.SetGroupIndicator(new ColorDrawable(Color.Transparent));
        }

        private void Form_GroupClick(object sender, ExpandableListView.GroupClickEventArgs e)
        {
            e.Handled = true;
        }

        private void PlatformDispose()
        {
            
        }
    }
}
