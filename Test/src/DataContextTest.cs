using System.Collections.Generic;
using Qoden.Binding;
using Qoden.Validation;
using Xunit;
using Assert = Xunit.Assert;

namespace Qoden.UI.Test
{
    public class DataContextTest
    {
        [Fact]
        public void PropertyChanges()
        {
            var ctx = new TestContext();
            Assert.True(ctx.HasErrors, "Valid/Invalid state indicated correctly right after data context instantiated");
            Assert.NotNull(ctx.GetErrors("Id"));
            ctx.Id = "Some Id";
            ctx.Industry = "Finance";
            ctx.Name = "Andrew";
            Assert.False(ctx.HasErrors, "Property changes affect context validtity status");
        }

		[Fact]
        public void PropertyChangeEvents()
        {
            var ctx = new TestContext();
            var propertyName = "";
            ctx.PropertyChanged += (o, e) => { propertyName = e.PropertyName; };
            ctx.Name = "Andrew";
            Assert.Equal(propertyName, "Name");
        }

		[Fact]
        public void NoPropertyChangeDuringValidation()
        {
            var ctx = ValidContext();
            var propertyName = "";
            ctx.PropertyChanged += (o, e) => { propertyName = e.PropertyName; };
            ctx.Validate();
            Assert.Equal(propertyName, "");
        }

        [Fact]
        public void EditingStatusFlags()
        {
            var ctx = ValidContext();
            Assert.False(ctx.Editing);

            ctx.BeginEdit();
            Assert.True(ctx.Editing);
            Assert.False(ctx.HasChanges);

            ctx.EndEdit();
            Assert.False(ctx.HasChanges);
            Assert.False(ctx.Editing);
        }

        [Fact]
        public void CancelEditRestoreStates()
        {
            var ctx = ValidContext();
            ctx.BeginEdit();

            var oldIndustru = ctx.Industry;
            ctx.Industry = "Some Industry";
            var oldName = ctx.Name;
            ctx.Name = "New Name";
            var oldId = ctx.Id;
            ctx.Id = "New ID";

            ctx.CancelEdit();

            Assert.Equal(oldIndustru, ctx.Industry);
            Assert.Equal(oldName, ctx.Name);
            Assert.Equal(oldId, ctx.Id);
        }

        [Fact]
        public void CancelEditFiresPropertyChange()
        {
            var ctx = ValidContext();
            var props = new List<string>();
            ctx.PropertyChanged += (sender, e) => { props.Add(e.PropertyName); };

            ctx.Industry = "New Industry";
            ctx.Id = "New ID";
            ctx.CancelEdit();

            Assert.Equal(new[] { "Industry", "Id" }, props);
        }

		private TestContext ValidContext()
		{
			return new TestContext
			{
				Id = "Some Id",
				Industry = "Finance",
				Name = "Andrew"
			};
		}
    }

    public class MyDto
    {
        public string Name { get; set; }
        public string Industry { get; set; }
    }

    public class TestContext : DataContext
    {
        public static string[] ValidIndsutries = {"Finance", "Healthcare", "Education"};

        private string _id;
        private readonly MyDto _dto;

        public TestContext()
        {
            _id = "";
            _dto = new MyDto();
        }

        public string Id
        {
            get => _id;
            set
            {
                Validator.CheckProperty(value)
                    .NotNull()
                    .NotEmpty();
                Remember(nameof(Id));
                SetProperty(ref _id, value, nameof(Id));
            }
        }

        public string Name
        {
            get => _dto.Name;
            set
            {
                Validator.CheckProperty(value)
                    .NotEmpty()
                    .MinLength(2)
                    .MaxLength(100);
                if (!Validating && _dto.Name != value)
                {
                    Remember(nameof(Name));
                    _dto.Name = value;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        public string Industry
        {
            get => _dto.Industry;
            set
            {
                Validator.CheckProperty(value)
                    .In(ValidIndsutries);
                if (!Validating && _dto.Industry != value)
                {
                    Remember(nameof(Industry));
                    _dto.Industry = value;
                    RaisePropertyChanged(nameof(Industry));
                }
            }
        }
    }
}