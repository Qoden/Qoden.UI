using System;
using Qoden.UI;
using UIKit;

namespace Example
{
    public partial class DateTimeTextField : QodenView
    {
        private void PlatformShowDatePicker()
        {
        }

        UIDatePicker _datePicker;
        private void PlatformCreate()
        {
            _datePicker = new UIDatePicker();
            _datePicker.Mode = UIDatePickerMode.Date;
            _datePicker.Date = DateTime.Now.ToNSDate();
            Text.PlatformView.InputView = _datePicker;
            _datePicker.ValueChanged += OnDateChanged;
            Text.PlatformView.TouchUpInside += DatePickerWillShow;
        }

        private void DatePickerWillShow(object sender, EventArgs e)
        {
            _datePicker.Date = Date.ToNSDate();
        }

        private void OnDateChanged(object sender, EventArgs e)
        {
            Date = _datePicker.Date.ToDateTime();
        }
    }
}
