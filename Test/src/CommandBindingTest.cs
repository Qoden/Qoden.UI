using Qoden.Binding;
using Qoden.Binding.Test;
using Xunit;

namespace Qoden.UI.Test
{
    public class CommandBindingTest
    {
        private BindingList Bindings { get; } = new BindingList();
        private FakeModel Model { get; } = new FakeModel();
        private FakeUiControl Control { get; }= new FakeUiControl();
        private IProperty<string> Target { get; set; }
        private IProperty<string> Source { get; set; }

        [Fact]
        public void ControlTriggersCommand()
        {
            Bindings.Command(Model.LoadCommand)
                .To(Control.ButtonClickTrigger());
            Bindings.Bind();
            Control.TriggerClick();
            Assert.True(Model.LoadCommand.IsRunning);
            Model.LoadCommand.Task.Wait();
            Assert.Equal("Name From Server", Model.Name);
        }

        [Fact]
        public void WhenStartedAndFinished()
        {
            bool finished = false;
            bool started = false;
            Bindings.Command(Model.LoadCommand)
                .WhenStarted(_ => started = true)
                .WhenFinished(_ => finished = true);
            Bindings.Bind();
            Model.LoadCommand.Execute();
            Assert.True(started);
            Assert.False(finished);
            Model.LoadCommand.Task.Wait();
            Assert.True(finished);
        }

        [Fact]
        public void BeforeAndAfterExecute()
        {
            bool beforeExecute = true, afterExecute = false;
            Bindings.Command(Model.LoadCommand)
                .BeforeExecute(_ => beforeExecute = Model.LoadCommand.IsRunning)
                .AfterExecute(_ => afterExecute = Model.LoadCommand.IsRunning)
                .To(Control.ButtonClickTrigger());
            Bindings.Bind();

            //Before and After execute does NOT execute when command launched from code.
            Model.LoadCommand.ExecuteAsync().Wait();
            Assert.True(beforeExecute);
            Assert.False(afterExecute);
            
            //Before and After execute are triggered when command initiated from UI.
            Control.TriggerClick();
            Assert.False(beforeExecute);
            Assert.True(afterExecute);
        }
    }
}