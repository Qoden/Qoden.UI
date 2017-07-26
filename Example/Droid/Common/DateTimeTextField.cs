using System;
using Android.Content;
using Android.Support.V7.App;
using Android.Support.V4.App;
using Qoden.UI;
using Android.OS;

namespace Example
{
    public partial class DateTimeTextField : QodenView
    {
        public DateTimeTextField(Context ctx) : base(ctx)
        {
        }

        private void PlatformCreate()
        {
            Text.PlatformView.Focusable = false;
            Text.PlatformView.Click += OnClick;
        }

        private void PlatformShowDatePicker()
        {
            var activity = (AppCompatActivity)Context;
            var pickerFragment = (DatePickerFragment)activity.SupportFragmentManager.FindFragmentByTag("DatePicker");
            pickerFragment = pickerFragment ?? new DatePickerFragment();
            pickerFragment.Owner = this;
            pickerFragment.Show(activity.SupportFragmentManager, "DatePicker");
        }

        private void OnDateSet(DateTime dt)
        {
            Date = dt;
        }

        private class DatePickerFragment : DialogFragment
        {
            public DateTimeTextField Owner { get; set; }

            public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                var dateTime = Owner.Date;
                return new Android.App.DatePickerDialog(Activity, OnTimeSet, dateTime.Year, dateTime.Month, dateTime.Day);
            }

            void OnTimeSet(object sender, Android.App.DatePickerDialog.DateSetEventArgs e)
            {
                Owner.OnDateSet(e.Date);
            }
        }
    }
}
