using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
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
        private Task _task;
        private int _isRunning;
        private CancelAsyncCommand _cancelCommand;
        private Exception _error;
        private CancellationTokenSource _commandCancellationTokenSource;

        /// <summary>
        /// Async command execution logic
        /// </summary>
        /// <param name="parameter">parameter passed to <see cref="ICommand.Execute"/></param>
        /// <param name="token">async execution cancellation token</param>
        /// <returns>async command task</returns>
        protected abstract Task ExecuteAsyncCommand(object parameter, CancellationToken token);

        protected sealed override async void ExecuteCommand(object parameter)
        {
            Task execution = null;
            Exception error = null;
            bool canceled = false;
            CancellationTokenSource cts = null;

            if (IsCanceled) return;

            var executionId = 0;
            try
            {
                _isRunning++;
                executionId = _isRunning;

                if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug("New execution '{executionId}'.", executionId);
                }

                if (_isRunning == 1)
                {
                    _commandCancellationTokenSource = new CancellationTokenSource();
                    Error = null;
                    RaisePropertyChanged(nameof(IsRunning));
                    RaiseCanExecuteChanged();
                }

                try
                {
                    cts = CancellationTokenSource.CreateLinkedTokenSource(_commandCancellationTokenSource.Token);
                    //Command might cancel current Task (if any) this is why temp local variable is used.
                    execution = Task = ExecuteAsyncCommand(parameter, cts.Token)
                        .ContinueWith(t =>
                        {
                            if (t.Exception != null)
                            {
                                ExceptionDispatchInfo.Capture(t.Exception.Flatten().InnerException).Throw();
                            }
                        }, cts.Token, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                    if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                    {
                        Logger.LogDebug("{executionId} started new Task {commandTask}", executionId, execution.Id);
                    }

                    CancellationTokenSource = cts;

                    if (_isRunning == 1)
                    {
                        _cancelCommand?.RaiseCanExecuteChanged();
                    }

                    if (CancellationTokenSource.IsCancellationRequested)
                    {
                        return;
                    }

                    await execution;
                }
                catch (OperationCanceledException)
                {
                    canceled = true;
                }
                catch (Exception e)
                {
                    error = e;
                }
                finally
                {
                    if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                    {
                        if (canceled || (cts?.IsCancellationRequested ?? false))
                        {
                            Logger.LogDebug("Execution '{executionId}' task '{commandTask}' canceled.",
                                executionId, execution?.Id);
                        }
                        else if (error != null)
                        {
                            Logger.LogDebug(error,
                                "Execution '{commandExecution}' task '{commandTask}' finished with error.",
                                executionId, execution?.Id);
                        }
                        else
                        {
                            Logger.LogDebug("Execution '{commandExecution}' task '{commandTask}' finished.",
                                executionId, execution?.Id);
                        }
                    }
                }
            }
            finally
            {
                if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug("'{executionId}' finished.", executionId);
                }

                if (Task == execution)
                {
                    Error = error;
                }

                _isRunning--;
                if (_isRunning == 0)
                {
                    _commandCancellationTokenSource = null;
                    RaisePropertyChanged(nameof(IsRunning));
                    RaiseCanExecuteChanged();
                    _cancelCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public void CancelRunningTask()
        {
            var isCancellationRequested = IsCancellationRequested;
            CancellationTokenSource?.Cancel();
            if (isCancellationRequested != IsCancellationRequested)
            {
                RaisePropertyChanged(nameof(IsCancellationRequested));
            }
        }

        public Task Task
        {
            get => _task;
            set
            {
                if (value != _task)
                {
                    _task = value;
                    RaisePropertyChanged(nameof(Task));
                }
            }
        }

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

        private CancellationTokenSource _cancellationTokenSource;

        protected CancellationTokenSource CancellationTokenSource
        {
            get => _cancellationTokenSource;
            private set
            {
                if (value != _cancellationTokenSource)
                {
                    var raiseIsCancellationRequested =
                        _cancellationTokenSource?.IsCancellationRequested != value?.IsCancellationRequested;
                    _cancellationTokenSource = value;
                    if (raiseIsCancellationRequested)
                    {
                        RaisePropertyChanged(nameof(IsCancellationRequested));
                    }
                }
            }
        }

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

        public bool IsCanceled => _commandCancellationTokenSource?.IsCancellationRequested ?? false;

        private class CancelAsyncCommand : CommandBase
        {
            readonly AsyncCommandBase _owner;

            public CancelAsyncCommand(AsyncCommandBase owner)
            {
                _owner = owner;
            }

            protected override bool CanExecuteCommand(object parameter)
            {
                return _owner.IsRunning && !_owner.IsCancellationRequested;
            }

            protected override void ExecuteCommand(object parameter)
            {
                var task = _owner.Task;
                var execution = _owner.Task;
                try
                {
                    if (_owner.Logger != null && _owner.Logger.IsEnabled(LogLevel.Debug))
                    {
                        _owner.Logger.LogDebug("Cancelation requested.");
                    }
                    _owner._commandCancellationTokenSource.Cancel();
                    ;
                    _owner.CancelRunningTask();
                }
                catch (ObjectDisposedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    if (_owner.Logger != null && _owner.Logger.IsEnabled(LogLevel.Debug))
                    {
                        _owner.Logger.LogDebug(e,
                            " Task '{commandTask}' execution '{commandExecution}' cancellation finished with error.",
                            execution.Id, task.Id);
                    }
                }
            }
        }
    }
}