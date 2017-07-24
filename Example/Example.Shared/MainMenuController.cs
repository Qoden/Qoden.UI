using System;
using System.Collections.Generic;
using System.Linq;
using Qoden.UI;

namespace Example
{
    public partial class MainMenuController
    {
        ProfileFormController profileController;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            profileController = GetChildViewController<ProfileFormController>();
            profileController.View = View.ProfileForm;
        }
    }
}
