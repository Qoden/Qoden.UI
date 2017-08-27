using System;
using Qoden.Binding;
using Qoden.Binding.Test;
using Xunit;
using Assert = Xunit.Assert;

namespace Qoden.UI.Test
{
    public class PropertyBindingTest
    {
        private BindingList Bindings { get; } = new BindingList();
        private FakeModel Model { get; } = new FakeModel();
        private FakeUiControl Control { get; }= new FakeUiControl();
        private IProperty<string> Target { get; set; }
        private IProperty<string> Source { get; set; }

        [Fact]
        public void UpdateTarget()
        {
            Bindings.Property(Model, x => x.Name)
                .UpdateTarget((_, s) => Control.Text = Control.Text + "1");
            Bindings.Bind();
            Bindings.UpdateTarget();
            Assert.Equal("1", Control.Text);
            Model.Name = "New Name";
            Assert.Equal("11", Control.Text);
        }
        
        [Fact]
        public void UpdateSource()
        {
            Bindings.Property(Model, x => x.Name)
                .To(Control.TextProperty())
                .UpdateSource((_, s) => Model.Name = _.Target.Value + " HAHA");
            Bindings.Bind();
            Control.Text = "Hello World";            
            Assert.Equal(Model.Name, "Hello World HAHA");
        }
        
        [Fact]
        public void AfterTargetUpdate()
        {
            Bindings.Property(Model, x => x.Name).To(Control.TextProperty())
              .AfterTargetUpdate((binding, s) =>
            {
                Target = (IProperty<string>)binding.Target;
                Source = (IProperty<string>)binding.Source;
            });
            Bindings.Bind();

            Model.Name = "Hello World";

            Assert.NotNull(Target);
            Assert.NotNull(Source);
        }

        [Fact]
        public void AfterSourceUpdate()
        {
            Bindings.Property(Model, x => x.Name).To(Control.TextProperty())
              .AfterSourceUpdate((binding, s) =>
              {
                  Target = (IProperty<string>)binding.Target;
                  Source = (IProperty<string>)binding.Source;
              });
            Bindings.Bind();

            Control.Text = "Hello World";

            Assert.NotNull(Target);
            Assert.NotNull(Source);
        }

        [Fact]
        public void Convert()
        {
            Bindings.Property(Model, x => x.Name)
                .Convert(x => x + " ABC")
                .To(Control.TextProperty());
            Bindings.Bind();
            Model.Name = "Hello";
            Assert.Equal("Hello ABC", Control.Text);
            //Since there is no back conversion specified binding became OneWay automatically.
            Control.Text = "Change";
            Assert.Equal("Hello", Model.Name);
        }

        [Fact]
        public void ConvertBackAndForth()
        {
            Bindings.Property(Model, x => x.Age)
                .Convert(x => x.ToString(), Int32.Parse)
                .To(Control.TextProperty());
            Bindings.Bind();
            Model.Age = 20;
            Assert.Equal("20", Control.Text);
            Control.Text = "25";
            Assert.Equal(25, Model.Age);
        }
    }
}
