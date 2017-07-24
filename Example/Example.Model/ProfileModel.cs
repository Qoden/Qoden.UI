using System;
using System.Threading.Tasks;
using Qoden.Binding;
using Qoden.Validation;

namespace Example.Model
{
    public class ProfileModel : DataContext
    {
        Field<ProfileDto> _dto;

        public ProfileModel()
        {
            _dto = FieldValue(new ProfileDto());
            UpdateCommand = new AsyncCommand()
            {
                Action = OnUpdate,
                CanExecute = (_) => !UpdateCommand.IsRunning
            };
        }

        private async Task OnUpdate(object arg)
        {
            await Task.Delay(1000);
        }

        public string FirstName 
        {
            get => _dto.Get<string>();
            set 
            {
                Validator.CheckProperty(value).NotEmpty();
                _dto.Set(value); 
            }
        }

        public string LastName 
        { 
            get => _dto.Get<string>(); 
            set
            {
                Validator.CheckProperty(value).NotEmpty();    
                _dto.Set(value);
            }
        }

        public DateTime BirthDate 
        { 
            get=> _dto.Get<DateTime>();
            set
            {
                Validator.CheckProperty(value).LessOrEqualTo(DateTime.Now);
                _dto.Set(value);
            }
        }

        public string PhoneNumber 
        { 
            get => _dto.Get<string>();
            set
            {
                Validator.CheckProperty(value).NotEmpty();
                _dto.Set(value);
            }
        }

        public string Email 
        { 
            get => _dto.Get<string>();
            set
            {
                Validator.CheckProperty(value ?? "").IsEmail();
                _dto.Set(value);
            }
        }

        public AsyncCommand UpdateCommand { get; private set; }
    }
}
