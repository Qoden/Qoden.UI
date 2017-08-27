using System;
using Qoden.Binding;
using Xunit;

namespace Qoden.UI.Test
{
    public class CommandTest
    {
        [Fact]
        public void ExecuteCommand()
        {
            var executed = false;
            var command = new Command()
            {
                Action = _ => executed = true
            };
            command.Execute();
            Assert.True(executed);
        }

        [Fact]
        public void CanExecutePreventCommandExecution()
        {
            var executed = false;
            var command = new Command()
            {
                Action = _ => executed = true,
                CanExecute = _ => false
            };
            command.Execute();
            Assert.False(executed);
        }

        [Fact]
        public void ExceptionsArePassedToCaller()
        {
            var command = new Command()
            {
                Action = _ => throw new ArgumentException()
            };
            Assert.Throws<ArgumentException>(() => command.Execute());
        }
    }
}