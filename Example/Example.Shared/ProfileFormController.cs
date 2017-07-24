using System;
using Example.Model;
using Qoden.Binding;
using Qoden.UI;

namespace Example
{
    public partial class ProfileFormController : QodenController<ProfileForm>
    {
        ProfileModel _model;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _model = this.GetViewModelStore().Get(() => new ProfileModel());

            //Bindings.Command(_model.UpdateCommand).To(View.Update.ClickTarget());

            Bindings.Property(_model, x => x.FirstName)
                    .To(View.FirstName.Text.TextProperty())
                    .AfterSourceUpdate(ProcessError<string>());
            Bindings.Property(_model, x => x.LastName)
                    .To(View.LastName.Text.TextProperty())
                    .AfterSourceUpdate(ProcessError<string>());
            Bindings.Property(_model, x => x.Email)
                    .To(View.Email.Text.TextProperty())
                    .AfterSourceUpdate(ProcessError<string>());
            Bindings.Property(_model, x => x.PhoneNumber)
                    .To(View.PhoneNumber.Text.TextProperty())
                    .AfterSourceUpdate(ProcessError<string>());
            Bindings.Property(_model, x => x.BirthDate)
                    .Convert(x => x.ToString())
                    .To(View.BirthDate.Text.TextProperty())
                    .AfterSourceUpdate(ProcessError<string>());
        }

        private BindingAction<T> ProcessError<T>()
        {
            return (t, s) =>
            {
                //var ui = new PlatformView(t.Owner);
                if (s.HasErrors)
                {
                    //ui.GetView().SetBackgroundColor(new RGB(255, 0, 0));
                }
                else 
                {
                    //ui.GetView().SetBackgroundColor(RGB.DarkGray);
                }
            };
        }
    }
}
