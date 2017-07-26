using System;
using System.Collections.Generic;
using Foundation;
using Qoden.Binding;
using Qoden.UI;
using UIKit;

namespace Example
{
    public partial class ProfileFormController : QodenController<ProfileForm>
    {
        private void HideKeyboard(IEventSource<UIButton> target, Command command)
        {
            foreach (var v in View.TextFields)
            {
                v.Text.PlatformView.ResignFirstResponder();
            }
        }
    }
}
