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

            Bindings.Command(_model.UpdateCommand).To(View.Title.Done.ClickTarget());
            Bindings.Command(_model.CancelCommand)
                    .To(View.Title.Cancel.ClickTarget())
                    .WhenFinished(HideKeyboard);

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
                    .To(View.BirthDate.DateProperty())
                    .AfterSourceUpdate(ProcessDateTimeError);
        }

        private void ProcessDateTimeError(IProperty<DateTime> target, IProperty<DateTime> source)
        {
            var dtView = (DateTimeTextField)target.Owner;
            if (source.HasErrors)
            {
                dtView.Text.SetTextColor(Theme.Colors.Error);
            }
            else
            {
                dtView.Text.SetTextColor(Theme.Colors.TableFieldText);
            }
        }

        private BindingAction<T> ProcessError<T>()
        {
            return (t, s) =>
            {
                var view = QView.Wrap<QEditText>(t.Owner);
                if (s.HasErrors)
                {
                    view.SetTextColor(Theme.Colors.Error);
                }
                else 
                {
                    view.SetTextColor(Theme.Colors.TableFieldText);
                }
            };
        }
    }
}
