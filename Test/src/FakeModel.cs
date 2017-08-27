using System;
using System.Threading;
using System.Threading.Tasks;
using Qoden.Validation;

namespace Qoden.Binding.Test
{
    public class FakeModel : DataContext
    {

        public FakeModel()
        {
            LoadCommand = new AsyncCommand()
            {
                Action = Load,
            };
        }
        string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Validator.CheckProperty(_name).NotEmpty();
                RaisePropertyChanged(nameof(Name));
            }
        }

        private int _age;
        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                Validator.CheckProperty(_age).GreaterOrEqualTo(18);
                RaisePropertyChanged(nameof(Age));
            }
        }

        public IAsyncCommand LoadCommand { get; } 

        private async Task Load(object _, CancellationToken token)
        {
            await Task.Delay(100, token);
            Name = "Name From Server";
        }
    }
}
