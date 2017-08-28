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
            public AsyncCommandExecutionPolicy Policy { get; set; } = AsyncCommandExecutionPolicy.Join;

            public IAsyncCommand Command
            {
                get
                {
                    if (_command == null)
                    {
                        _command = new AsyncCommand()
                        {
                            Action = (p, t) => CommandAction(p, t),
                            ExecutionPolicy = Policy
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
            Assert.NotNull(command.CommandExecution);
            Assert.Equal(0, fixture.Executed);
            
            await command.Task;
            
            Assert.Equal(1, fixture.Executed);
            Assert.False(command.IsRunning);
            Assert.Null(command.Task);
            Assert.Null(command.CommandExecution);
        }

        [Fact]
        public void MultipleExecutions_SameTask()
        {
            var fixture = new CommandFixture();
            var command = fixture.Command;
            
            command.Execute();
            var t1 = command.Task;
            command.Execute();
            var t2 = command.Task;
            command.Execute();
            var t3 = command.Task;
            
            Assert.Same(t1, t2);
            Assert.Same(t2, t3);
            command.Task?.Wait();
        }

        [Fact]
        public void MultipleExecutions_DifferentTask()
        {
            var fixture = new CommandFixture();
            var command = fixture.Command;
            
            command.Execute();
            var t1 = command.Task;
            command.Task.Wait();
            
            command.Execute();
            var t2 = command.Task;
            command.Task.Wait();
            
            command.Execute();
            var t3 = command.Task;
            command.Task.Wait();
            
            Assert.NotSame(t1, t2);
            Assert.NotSame(t2, t3);
        }

        [Fact]
        public void SyncExecutionFinishesRightAway()
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
        public void ExecuteAsyncThrows()
        {
            var command = new AsyncCommand()
            {
                Action = (p, token) => throw new NotImplementedException()
            };
            var task = command.ExecuteAsync();
            Assert.Throws<NotImplementedException>(() => task.GetAwaiter().GetResult());
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
            Assert.Null(command.Task);
            Assert.Equal(0, fixture.Executed);
        }
        
        [Fact]
        public void CancelCommand_ActionDoesNotHonourCancelToken()
        {
            var fixture = new CommandFixture()
            {
                UseCancellationToken = false
            };
            var command = fixture.Command;
            
            command.Execute();
            command.CancelCommand.Execute();
            Assert.True(command.IsCancellationRequested);
            Assert.True(command.IsRunning);
            Assert.NotNull(command.Task);
            Assert.Throws<TaskCanceledException>(() => command.Task.GetAwaiter().GetResult());
            Assert.Equal(1, fixture.Executed);
        }

        [Fact]
        public void AsyncCommandExecutionPolicy_Join()
        {
            var fixture = new CommandFixture()
            {
                Policy = AsyncCommandExecutionPolicy.Join
            };
            var command = fixture.Command;
            
            command.Execute();
            var task1 = command.Task;
            var execution1 = command.CommandExecution;
            
            command.Execute();
            var task2 = command.Task;
            var execution2 = command.CommandExecution;
            
            Assert.Same(task1, task2);
            Assert.Same(execution1, execution2);
            command.Task.Wait();
            Assert.Equal(1, fixture.Executed);
        }
        
        [Fact]
        public async Task AsyncCommandExecutionPolicy_Wait()
        {
            var fixture = new CommandFixture()
            {
                Policy = AsyncCommandExecutionPolicy.Wait
            };
            var command = fixture.Command;

            command.Execute();
            var task1 = command.Task;
            var execution1 = command.CommandExecution;
            
            command.Execute();
            var task2 = command.Task;
            var execution2 = command.CommandExecution;
            
            Assert.Same(task1, task2);
            Assert.Same(execution1, execution2);

            await task1;
            execution2 = command.CommandExecution;
            Assert.NotSame(execution1, execution2);
            
            await Task.WhenAll(task1, task2);
            Assert.Equal(2, fixture.Executed);
        }
        
        [Fact]
        public void AsyncCommandExecutionPolicy_Cancel()
        {
            var fixture = new CommandFixture()
            {
                Policy = AsyncCommandExecutionPolicy.Cancel
            };
            var command = fixture.Command;

            command.Execute();
            var execution1 = command.CommandExecution;
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
        public void CancelWhile_Join()
        {
            var fixture = new CommandFixture()
            {
                Policy = AsyncCommandExecutionPolicy.Join
            };
            var command = fixture.Command;
            command.Execute();
            command.Execute();            
            command.CancelCommand.Execute();
            command.Task?.Wait();
            Assert.False(command.IsRunning);
            Assert.Equal(0, fixture.Executed);
        }        
        
        [Fact]
        public void CancelWhile_Wait()
        {
            var fixture = new CommandFixture()
            {
                Policy = AsyncCommandExecutionPolicy.Wait
            };
            var command = fixture.Command;
            command.Execute();
            command.Execute();
            //since command waits for previous execution this actually cancel first execution
            //and then command proceed to second one
            command.CancelCommand.Execute(); 
            command.Task?.Wait();
            Assert.False(command.IsRunning);
            Assert.Equal(1, fixture.Executed);
        }
        
        [Fact]
        public void CancelWhile_Cancel()
        {
            var fixture = new CommandFixture()
            {
                Policy = AsyncCommandExecutionPolicy.Cancel
            };
            var command = fixture.Command;
            command.Execute();
            command.Execute();            
            command.CancelCommand.Execute();
            command.Task?.Wait();
            Assert.False(command.IsRunning);
            Assert.Equal(0, fixture.Executed);
        }
    }
}