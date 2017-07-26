using System;
using System.Threading.Tasks;
using Qoden.Binding;
using Qoden.Validation;

namespace Example.Model
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Convert checked value from one type to another.
        /// </summary>
        /// <remarks>If converter throws then exception is supressed and check failed with error</remarks>
        /// <typeparam name="T">Original type</typeparam>
        /// <typeparam name="TRet">Target value type</typeparam>
        /// <param name="check">checker</param>
        /// <param name="converter">converter function</param>
        /// <param name="force">run converter even if checker has error</param>
        /// <returns></returns>
        public static Check<TRet> Convert<T, TRet>(this Check<T> check, Func<T, TRet> converter, bool force = false)
        {
            if (check.HasError && !force)
                return new Check<TRet>(default(TRet), check.Key, check.Validator, check.OnErrorAction);

            try
            {
                var value = converter(check.Value);
                return new Check<TRet>(value, check.Key, check.Validator, check.OnErrorAction);
            }
            catch (Exception e)
            {
                var newCheck = new Check<TRet>(default(TRet), check.Key, check.Validator, check.OnErrorAction);
                var error = new Error(e.Message)
                {
                    {"Value", check.Value},
                    {"Exception", e}
                };
                newCheck.FailValidator(error, newCheck.OnErrorAction);
                return newCheck;
            }
        }
    }

    public class ProfileModel : DataContext
    {
        ProfileDto _dto;

        public ProfileModel()
        {
            _dto = new ProfileDto()
            {
                PhoneNumber = "+79219736635"
            };
            UpdateCommand = new AsyncCommand()
            {
                Action = OnUpdate,
                CanExecute = CanUpdate
            };

            CancelCommand = new Command()
            {
                Action = OnCancel,
                CanExecute = (arg) => HasChanges
            };

            PropertyChanged += (sender, e) =>
            {
                CancelCommand.RaiseCanExecuteChanged();
                UpdateCommand.RaiseCanExecuteChanged();
            };
        }

        private void OnCancel(object obj)
        {
            CancelEdit();
            CancelCommand.RaiseCanExecuteChanged();
        }

        public Command CancelCommand { get; private set; }

        private bool CanUpdate(object arg)
        {
            return !UpdateCommand.IsRunning && Validator.IsValid;
        }

        private async Task OnUpdate(object arg)
        {
            Console.WriteLine("!!!!!!");
            await Task.Delay(1000);
            EndEdit();
        }

        private void ValidateName(Check<string> checker)
        {
            checker.NotEmpty().MinLength(1).MaxLength(40);
        }

        public string FirstName
        {
            get => _dto.FirstName;
            set
            {
                ValidateName(Validator.CheckProperty(value));
                if (!Validating && _dto.FirstName != value)
                {
                    RememberAndBeginEdit();
                    _dto.FirstName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string LastName
        {
            get => _dto.LastName;
            set
            {
                ValidateName(Validator.CheckProperty(value));
                if (!Validating && value != _dto.LastName)
                {
                    RememberAndBeginEdit();
                    _dto.LastName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public DateTime BirthDate
        {
            get => _dto.BirthDate;
            set
            {
                Validator.CheckProperty(value)
                         .Less(DateTime.Now);
                if (!Validating && _dto.BirthDate != value)
                {
                    RememberAndBeginEdit();
                    _dto.BirthDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        //private string _birthDayStr;
        //public string BirthDate 
        //{
        //    get => _birthDayStr;
        //    set
        //    {
        //        var birthDay = Validator.CheckProperty(value)
        //                                .NotEmpty()
        //                                .Convert(DateTime.Parse)
        //                                .Less(DateTime.Now);
        //        if (!Validating && _birthDayStr != value)
        //        {
        //            RememberAndBeginEdit();
        //            _birthDayStr = value;
        //            if (birthDay.IsValid)
        //                _dto.BirthDate = birthDay.Value;
        //            RaisePropertyChanged();
        //        }
        //    }
        //}

        public string PhoneNumber
        {
            get => _dto.PhoneNumber;
            set
            {
                Validator.CheckProperty(value).NotEmpty();
                if (!Validating && _dto.PhoneNumber != value)
                {
                    RememberAndBeginEdit();
                    _dto.PhoneNumber = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Email
        {
            get => _dto.Email;
            set
            {
                Validator.CheckProperty(value).IsEmail();
                if (!Validating && _dto.Email != value)
                {
                    RememberAndBeginEdit();
                    _dto.Email = value;
                    RaisePropertyChanged();
                }
            }
        }

        public AsyncCommand UpdateCommand { get; private set; }
    }
}
