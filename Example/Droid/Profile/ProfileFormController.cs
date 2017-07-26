using System;
using Android.Views.InputMethods;
using Android.Widget;
using Qoden.Binding;
using Qoden.UI;

namespace Example
{
    public partial class ProfileFormController
    {
        private void HideKeyboard(IEventSource<Button> target, Command command)
        {
            var inputMethodManager = (InputMethodManager)Context.GetSystemService(Android.Content.Context.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(View.WindowToken, 0);
        }
    }
}
