using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Qoden.Binding;
using Xunit;

namespace Qoden.UI.Test
{
    public class AsyncCommandTest
    {
        public class CommandFixture
        {
            public int Executed;
            private AsyncCommand _command;
            public bool UseCancellationToken { get; set; } = true;
            public AsyncCommandSerializationPolicy Policy { get; set; } = AsyncCommandSerializationPolicy.None;

            public IAsyncCommand Command
            {
                get
                {
                    if (_command == null)
                    {
                        _command = new AsyncCommand()
                        {
                            Action = (p, t) => CommandAction(p, t),
                            SerializationPolicy = Policy
                        };
                    }
                    return _command;
                }
            }
            
            private Task CommandAction(object p, CancellationToken _)
            {
                if (SyncAction)
                {
                    return Task.FromResult(Executed++);
                }
                else if (UseCancellationToken)
                {
                    return Task.Delay(100, _).ContinueWith(t => Executed++, _);
                }
                else
                {
                    return Task.Delay(100).ContinueWith(t => Executed++);
                }
            }

            public bool SyncAction { get; set; }
        }
        
        [Fact]
        public async Task AsyncCommandExecutes()
        {
            var fixture = new CommandFixture();
            var command = fixture.Command;
            
            Assert.False(command.IsRunning);
            
            command.Execute();
            
            Assert.True(command.IsRunning);
            Assert.NotNull(command.Task);
            Assert.Equal(0, fixture.Executed);
            
            await command.Task;
            
            Assert.Equal(1, fixture.Executed);
            Assert.False(command.IsRunning);
        }

        [Fact]
        public void MultipleExecutions_DifferentTasks()
        {
            var fixture = new CommandFixture();
            var command = fixture.Command;
            
            command.Execute();
            var t1 = command.Task;
            
            command.Execute();
            var t2 = command.Task;
            
            command.Execute();
            var t3 = command.Task;
            
            Assert.NotSame(t1, t2);
            Assert.NotSame(t2, t3);
        }

        [Fact]
        public void SyncExecutionFinishRightAway()
        {
            var fixture = new CommandFixture()
            {
                SyncAction = true
            };
            var command = fixture.Command;
            
            command.Execute();
            Assert.False(command.IsRunning);
            Assert.Equal(1, fixture.Executed);
        }

        [Fact]
        public async Task ExecuteAsyncThrows()
        {
            var command = new AsyncCommand()
            {
                Action = (p, token) => throw new NotImplementedException()
            };
            await Assert.ThrowsAsync<NotImplementedException>(async () => await command.ExecuteAsync());
        }

        [Fact]
        public void ExecuteAsyncThrowsOnCancel()
        {
            var command = new AsyncCommand()
            {
                Action = (p, token) => Task.Delay(100, token)
            };
            var task = command.ExecuteAsync();
            command.CancelCommand.Execute();
            Assert.Throws<TaskCanceledException>(() => task.GetAwaiter().GetResult());
        }

        [Fact]
        public void CancelCommand()
        {
            var fixture = new CommandFixture();
            var command = fixture.Command;
            
            Assert.False(command.CancelCommand.CanExecute());
            command.Execute();
            Assert.True(command.CancelCommand.CanExecute());
            command.CancelCommand.Execute();
            Assert.True(command.IsCancellationRequested);
            Assert.False(command.IsRunning);
            Assert.Equal(0, fixture.Executed);
        }
        
        [Fact]
        public void CancelCommand_ActionDoesNotUseCancelToken()
        {
            var fixture = new CommandFixture()
            {
                UseCancellationToken = false
            };
            var command = fixture.Command;
            
            command.Execute();
            command.CancelCommand.Execute();
            Assert.True(command.IsCancellationRequested);
            Assert.False(command.IsRunning);
            Assert.Throws<TaskCanceledException>(() => command.Task.GetAwaiter().GetResult());
            Assert.Equal(0, fixture.Executed);
        }

        [Fact]
        public async Task SerializationPolicy_Wait()
        {
            var fixture = new CommandFixture()
            {
                Policy = AsyncCommandSerializationPolicy.Wait
            };
            var command = fixture.Command;

            command.Execute();
            command.Execute();
            await command.Task;
            Assert.False(command.IsRunning);
            Assert.Equal(2, fixture.Executed);
        }
        
        [Fact]
        public void SerializationPolicy_WaitCancel()
        {
            var fixture = new CommandFixture()
            {
                Policy = AsyncCommandSerializationPolicy.Wait | AsyncCommandSerializationPolicy.Cancel
            };
            var command = fixture.Command;

            command.Execute();
            var execution1 = command.Task;
            command.Execute();
            Assert.Throws<TaskCanceledException>(() => execution1.GetAwaiter().GetResult());
            command.Task.Wait();
            Assert.Equal(1, fixture.Executed);
        }

        [Fact]
        public void CanExecute_False()
        {
            var command = new AsyncCommand()
            {
                Action = (p, _) => Task.Delay(100, _),
                CanExecute = p => false
            };
            command.Execute();
            Assert.False(command.IsRunning);
            Assert.Null(command.Task);
        }

        [Fact]
        public void CancelWhile_Wait()
        {
            var fixture = new CommandFixture()
            {
                Policy = AsyncCommandSerializationPolicy.Wait
            };
            var command = fixture.Command;
            command.Execute();
            var task1 = command.Task;
            command.Execute();
            var task2 = command.Task;
            command.CancelCommand.Execute();
            Assert.Throws<TaskCanceledException>(() => task1.GetAwaiter().GetResult());
            Assert.Throws<TaskCanceledException>(() => task2.GetAwaiter().GetResult());
            Assert.False(command.IsRunning);
            Assert.Equal(0, fixture.Executed);
        }
        
        [Fact]
        public void CancelWhile_Cancel()
        {
            var fixture = new CommandFixture()
            {
                Policy = AsyncCommandSerializationPolicy.Cancel | AsyncCommandSerializationPolicy.Wait
            };
            var command = fixture.Command;
            command.Execute();
            command.Execute();            
            command.CancelCommand.Execute();
            Assert.Throws<TaskCanceledException>(() => command.Task.GetAwaiter().GetResult());
            Assert.False(command.IsRunning);
            Assert.Equal(0, fixture.Executed);
        }
    }
}