using System;
using System.ComponentModel;
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
        /// Prepare for new command execution.   
        /// </summary>
        /// <param name="parameter">parameter passed to <see cref="ICommand.Execute"/></param>
        /// <param name="executingTask"></param>
        /// <remarks>
        /// This method primary responsibility is to make sure that <see cref="executingTask"/> either shutdown or awaited before
        /// new execution starts.
        /// </remarks>
        protected virtual Task PrepareAsyncCommandExecution(object parameter, Task executingTask)
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
        
        public Task Task { get; private set; }

        protected sealed override async void ExecuteCommand(object parameter)
        {
            var oldTask = Task;
            var tcs = new TaskCompletionSource<object>();
            var runningTask = Task = tcs.Task;
            
            try
            {
                Error = null;
                IsRunning = true;

                try
                {
                    await PrepareAsyncCommandExecution(parameter, oldTask);
                }
                catch (OperationCanceledException)
                {
                    if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                    {
                        Logger.LogDebug("Command execution canceled during preparation.");
                    }
                    return;
                }

                //check if action still can be executed after preparations
                if (!CanExecuteCommand(parameter))
                {
                    if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                    {
                        Logger.LogDebug(
                            "Cannot execute command, CanExecute return false after command preparation.");
                    }
                    return;
                }

                if (oldTask != null && !oldTask.IsCompleted)
                {
                    if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                    {
                        Logger.LogDebug(
                            "Already running, join to existing execution with id '{commandExecution}'.",
                            runningTask.Id);
                    }
                    await oldTask;
                    return;
                }

                try
                {
                    CancellationTokenSource = new CancellationTokenSource();
                    RaisePropertyChanged(nameof(IsCancellationRequested));
                    if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                    {
                        Logger.LogDebug("Execution '{commandExecution}' started.", runningTask.Id);
                    }
                    await ExecuteAsyncCommand(parameter, CancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                        Logger.LogDebug("Execution '{commandExecution}' canceled.", runningTask.Id);
                }
                catch (Exception e)
                {
                    if (Logger != null && Logger.IsEnabled(LogLevel.Warning))
                        Logger.LogWarning("Execution '{commandExecution}' finished with error.", runningTask.Id, e);
                    Error = e;
                }
            }
            finally
            {
                IsRunning = false;
                tcs.SetResult(null);
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

        private bool _isRunning;

        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                if (_isRunning == value) return;
                _isRunning = value;
                RaisePropertyChanged(nameof(IsRunning));
                RaiseCanExecuteChanged();
                _cancelCommand?.RaiseCanExecuteChanged();
            }
        }

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