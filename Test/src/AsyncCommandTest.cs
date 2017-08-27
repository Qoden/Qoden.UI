using System.Threading.Tasks;
using Qoden.Binding;
using Xunit;

namespace Qoden.UI.Test
{
    public class AsyncCommandTest
    {
        [Fact]
        public async Task AsyncCommandExecutes()
        {
            bool executed = false;
            var command = new AsyncCommand()
            {
                Action = (p, token) =>
                {
                    return Task.Delay(100, token).ContinueWith(t => executed = true, token);
                }
            };
            
            Assert.False(command.IsRunning, "Command is not running after creation");
            
            command.Execute();
            
            Assert.True(command.IsRunning, "Command IsRunning after Execute");
            Assert.False(executed, "Command executed asynchroniously");
            
            await command.Task;
            
            Assert.True(executed, "Command executed asynchroniously");
            Assert.False(command.IsRunning, "Command IsRunning == false after command task completes");
        }

        [Fact]
        public void CancelCommand()
        {
            bool executed = false;
            var command = new AsyncCommand()
            {
                Action = (p, token) =>
                {
                    return Task.Delay(100, token).ContinueWith(t => executed = true, token);
                }
            };
            command.Execute();
            command.CancelCommand.Execute();
            Assert.Throws<TaskCanceledException>(() => command.Task.GetAwaiter().GetResult());
            Assert.False(executed, "Command not executed because it is canceled");
        }

        [Fact]
        public void AsyncCommandExecutionPolicy_Join()
        {
            var executed = 0;
            var command = new AsyncCommand()
            {
                Action = (p, _) =>
                {
                    return Task.Delay(100, _).ContinueWith(t => executed++, _);
                },
                ExecutionPolicy = AsyncCommandExecutionPolicy.Join
            };
            command.Execute();
            command.Execute();
            command.Task.Wait();
            Assert.Equal(1, executed);
        }
        
        [Fact]
        public async Task AsyncCommandExecutionPolicy_Wait()
        {
            var executed = 0;
            var command = new AsyncCommand()
            {
                Action = async (p, _) =>
                {
                    await Task.Delay(100, _);
                    executed = executed + 1;
                },
                ExecutionPolicy = AsyncCommandExecutionPolicy.Wait
            };
            command.Execute();
            var task1 = command.Task;
            command.Execute();
            var task2 = command.Task; //got same task instead of a new one here.
            await Task.WhenAll(task1, task2);
            Assert.Equal(2, executed);
        }
        
        [Fact]
        public void AsyncCommandExecutionPolicy_Cancel()
        {
            var executed = 0;
            var command = new AsyncCommand()
            {
                Action = (p, _) =>
                {
                    return Task.Delay(100, _).ContinueWith(t => executed++, _);
                },
                ExecutionPolicy = AsyncCommandExecutionPolicy.Cancel
            };
            command.Execute();
            var task1 = command.Task;
            command.Execute();
            Assert.Throws<TaskCanceledException>(() => task1.GetAwaiter().GetResult());
            command.Task.Wait();
            Assert.Equal(1, executed);
        }

        [Fact]
        public void CanExecute()
        {
            var executed = 0;
            var command = new AsyncCommand()
            {
                Action = (p, _) =>
                {
                    return Task.Delay(100, _).ContinueWith(t => executed++, _);
                },
                CanExecute = p => false
            };
            command.Execute();
            Assert.False(command.IsRunning);
            Assert.Null(command.Task);
        }
    }
}