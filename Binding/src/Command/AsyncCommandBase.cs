using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Qoden.Binding
{
    /// <summary>
    /// Base class for async commands.
    /// </summary>
    public abstract class AsyncCommandBase : CommandBase, IAsyncCommand
    {
        /// <summary>
        /// Prepare for new command execution. If <see cref="CommandExecution"/> is not completed after this call then 
        /// command will join existing execution.
        /// </summary>
        /// <param name="parameter">parameter passed to <see cref="ICommand.Execute"/></param>
        /// <remarks>
        /// This method primary responsibility is to make sure that <see cref="CommandExecution"/> either shutdown 
        /// or awaited before new execution starts.
        /// </remarks>
        protected virtual Task PrepareCommandExecution(object parameter)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Async command execution logic
        /// </summary>
        /// <param name="parameter">parameter passed to <see cref="ICommand.Execute"/></param>
        /// <param name="token">async execution cancellation token</param>
        /// <returns>async command task</returns>
        protected abstract Task ExecuteAsyncCommand(object parameter, CancellationToken token);

        public Task Task => _commandTaskSource?.Task;

        private int _isRunning;
        private TaskCompletionSource<object> _commandTaskSource;

        public Task CommandExecution { get; private set; }

        protected sealed override async void ExecuteCommand(object parameter)
        {
            bool canceled = false;
            Exception error = null;
            try
            {
                Error = null;

                _isRunning++;
                if (_isRunning == 1)
                {
                    _commandTaskSource = new TaskCompletionSource<object>();
                    if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                    {
                        Logger.LogDebug("Task '{commandTask}' started.", Task.Id);
                    }
                    RaisePropertyChanged(nameof(Task));
                    RaisePropertyChanged(nameof(IsRunning));
                    RaiseCanExecuteChanged();
                    _cancelCommand?.RaiseCanExecuteChanged();
                }

                try
                {
                    await PrepareCommandExecution(parameter);
                }
                catch (OperationCanceledException)
                {
                    if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                    {
                        if (CommandExecution == null)
                        {
                            Logger.LogDebug("Task '{commandTask}' execution canceled during preparation.",
                                Task.Id);
                        }
                        else
                        {
                            Logger.LogDebug(
                                "Task '{commandTask}' execution canceled during preparation. Continue with already running execution {commandExecution}",
                                Task.Id, CommandExecution.Id);
                        }
                    }
                    return;
                }

                try
                {
                    if (CommandExecution != null && !CommandExecution.IsCompleted)
                    {
                        if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                        {
                            Logger.LogDebug(
                                "Task '{commandTask}', join to running command execution '{commandExecution}'.",
                                Task.Id, CommandExecution.Id);
                        }
                        await CommandExecution;
                        return;
                    }
                    
                    var cts = CancellationTokenSource = new CancellationTokenSource();
                    RaisePropertyChanged(nameof(IsCancellationRequested));

                    CommandExecution = ExecuteAsyncCommand(parameter, cts.Token);
                    if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                    {
                        Logger.LogDebug(
                            "Task '{commandTask}' started new execution '{commandExecution}'.",
                            Task.Id, CommandExecution.Id);
                    }
                    await CommandExecution;
                    cts.Token.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    canceled = true;
                    if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                        Logger.LogDebug("Task '{commandTask}' execution '{commandExecution}' canceled.", Task.Id,
                            CommandExecution?.Id);
                }
                catch (Exception e)
                {
                    error = e;
                    if (Logger != null && Logger.IsEnabled(LogLevel.Warning))
                        Logger.LogWarning("Task '{commandTask}' execution '{commandExecution}' finished with error.",
                            Task.Id, CommandExecution?.Id);
                }
            }
            finally
            {
                _isRunning--;
                if (_isRunning == 0)
                {
                    var tcs = _commandTaskSource;
                    Error = error;
                    CommandExecution = null;
                    _commandTaskSource = null;
                    RaisePropertyChanged(nameof(Task));
                    RaisePropertyChanged(nameof(IsRunning));
                    RaiseCanExecuteChanged();
                    _cancelCommand?.RaiseCanExecuteChanged();

                    if (canceled)
                    {
                        tcs.SetCanceled();
                    }
                    else if (Error != null)
                    {
                        tcs.SetException(Error);
                    }
                    else
                    {
                        tcs.SetResult(null);    
                    }
                }
            }
        }

        internal bool CancelCommandRunning;

        private CancelAsyncCommand _cancelCommand;

        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new CancelAsyncCommand(this);
                }
                return _cancelCommand;
            }
        }

        protected CancellationTokenSource CancellationTokenSource { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(PropertyChangedEventArgs key)
        {
            PropertyChanged?.Invoke(this, key);
        }

        protected void RaisePropertyChanged(string key)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(key));
        }

        public bool IsRunning => _isRunning > 0;

        public bool IsCancellationRequested => CancellationTokenSource?.IsCancellationRequested ?? false;

        private Exception _error;

        public Exception Error
        {
            get => _error;
            set
            {
                if (_error != value)
                {
                    _error = value;
                    RaisePropertyChanged(nameof(Error));
                }
            }
        }

        private class CancelAsyncCommand : CommandBase
        {
            readonly AsyncCommandBase _owner;

            public CancelAsyncCommand(AsyncCommandBase owner)
            {
                _owner = owner;
            }

            protected override bool CanExecuteCommand(object parameter)
            {
                return _owner.IsRunning && !_owner.CancellationTokenSource.IsCancellationRequested;
            }

            protected override void ExecuteCommand(object parameter)
            {
                try
                {
                    if (_owner.Logger != null && _owner.Logger.IsEnabled(LogLevel.Debug))
                    {
                        _owner.Logger.LogDebug("Cancelation requested.");
                    }
                    _owner.CancellationTokenSource.Cancel();
                    _owner.RaisePropertyChanged(nameof(IsCancellationRequested));
                }
                catch (ObjectDisposedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    if (_owner.Logger != null && _owner.Logger.IsEnabled(LogLevel.Debug))
                    {
                        _owner.Logger.LogDebug("Cancellation finished with error.", e);
                    }
                }
            }
        }
    }
}