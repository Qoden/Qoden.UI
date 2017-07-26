using System;
using Android.Content;
using Android.Telephony;
using Android.Text;
using Java.Lang;

namespace Example
{
    public partial class PhoneNumberField
    {
        public PhoneNumberField(Context ctx) : base(ctx)
        {}

        private void PlatformCreate()
        {
            Text.PlatformView.InputType = InputTypes.ClassPhone;
            Text.PlatformView.AddTextChangedListener(new PhoneNumberFormattingTextWatcher());
        }
    }
}
