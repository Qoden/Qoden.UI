using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Qoden.Binding
{
    public enum AsyncCommandExecutionPolicy
    {
        /// <summary>
        /// Join to existing command task if possible.
        /// </summary>
        Join,

        /// <summary>
        /// Wait until command task finish before executing.
        /// </summary>
        Wait,

        /// <summary>
        /// Cancel running command task before esecuting it.
        /// </summary>
        Cancel
    }

    public sealed class AsyncCommand : AsyncCommandBase
    {
        CancellationTokenSource _delayToken;
        public AsyncCommandExecutionPolicy ExecutionPolicy { get; set; } = AsyncCommandExecutionPolicy.Join;

        public AsyncCommand()
        {
        }

        public AsyncCommand(Func<object, CancellationToken, Task> action) : this(action, null)
        {
        }

        public AsyncCommand(Func<object, CancellationToken, Task> action, Func<object, bool> canExecute)
        {
            Action = action;
            if (canExecute != null)
                CanExecute = canExecute;
            else
                CanExecute = DefaultCanExecute;
            Delay = TimeSpan.Zero;
        }

        protected override async Task PrepareCommandExecution(object o)
        {
            if (Action != null)
            {
                if (_delayToken != null && !_delayToken.IsCancellationRequested)
                {
                    _delayToken.Cancel();
                    _delayToken = null;
                }

                if (Delay > TimeSpan.Zero)
                {
                    if (Logger != null && Logger.IsEnabled(LogLevel.Trace))
                    {
                        Logger.LogTrace("Delayed by {delay}", Delay);
                    }
                    _delayToken = new CancellationTokenSource();
                    //If this throws OperationCanceledException then entire command execution is canceled
                    await Task.Delay(Delay, _delayToken.Token);
                }

                if (ExecutionPolicy == AsyncCommandExecutionPolicy.Cancel && CommandExecution != null && !CommandExecution.IsCompleted)
                {
                    await CancelRunningTask(CommandExecution);
                }

                if (ExecutionPolicy == AsyncCommandExecutionPolicy.Wait && CommandExecution != null && !CommandExecution.IsCompleted)
                {
                    await WaitForRunningTask(CommandExecution);
                }
            }
        }

        private async Task WaitForRunningTask(Task prevTask)
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug(
                    "Wait for command execution '{commandExecution}'", prevTask.Id);
            }
            try
            {
                await prevTask;
            }
            catch
            {
                //ignore
            }
        }

        private async Task CancelRunningTask(Task prevTask)
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug(
                    "Cancel previous command execution '{commandExecution}'", prevTask.Id);
            }
            try
            {
                ((IAsyncCommand) this).CancelCommand.Execute();
                await prevTask;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug(
                        e, "Previous execution '{commandExecution}' cancellation finished with error.", prevTask?.Id);
                }
            }

            if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug(
                    "Pervious execution '{commandExecution}' canceled. Command execution continue.", prevTask?.Id);
            }
        }

        protected override async Task ExecuteAsyncCommand(object parameter, CancellationToken token)
        {
            if (Action != null)
            {
                await Action(parameter, token);
            }
        }

        protected override bool CanExecuteCommand(object parameter)
        {
            return Action != null && (CanExecute == null || CanExecute(parameter));
        }

        Func<object, CancellationToken, Task> _action;

        public Func<object, CancellationToken, Task> Action
        {
            get => _action;
            set => _action = value ?? throw new ArgumentNullException(nameof(value));
        }

        Func<object, bool> _canExecute;

        public Func<object, bool> CanExecute
        {
            get => _canExecute;
            set => _canExecute = value ?? throw new ArgumentNullException(nameof(value));
        }

        public TimeSpan Delay { get; set; }

        public Task ExecuteWithoutDelay(object param)
        {
            var oldDelay = Delay;
            try
            {
                Delay = TimeSpan.Zero;
                Execute(param);
            }
            finally
            {
                Delay = oldDelay;
            }
            return Task;
        }

        private bool DefaultCanExecute(object arg)
        {
            return true;
        }
    }
}