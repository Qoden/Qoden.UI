using System;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Qoden.Validation;
using Qoden.Util;

namespace Qoden.Binding
{
    public interface ICommand
    {
        event EventHandler CanExecuteChanged;

        bool CanExecute(object parameter);

        void Execute(object parameter);
    }

    public static class CommandExtensions
    {
        public static void Execute(this ICommand command)
        {
            command.Execute(null);
        }

        public static bool CanExecute(this ICommand command)
        {
            return command.CanExecute(null);
        }
    }

    public abstract class CommandBase : ICommand
    {
        private string _name = "No Name";
        public ILogger Logger { get; set; }

        public string Name
        {
            get => _name;
            set { _name = Assert.Property(value).NotNull().Value; }
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CheckCanExecute(parameter);
        }

        protected abstract bool CheckCanExecute(object parameter);

        public void Execute(object parameter)
        {
            if (CheckCanExecute(parameter))
            {
                if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug("'Execute' command '{command}'", Name);
                }
                DoExecute(parameter);
            }
            else
            {
                if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug("Cannot 'Execute' command '{command}', CanExecute return false", Name);
                }
            }
        }

        protected abstract void DoExecute(object parameter);
    }

    public class Command : CommandBase
    {
        public Command()
        {
        }

        public Command(Action action)
        {
            Action = _ => action();
        }

        public Command(Action action, Func<bool> canExecute) : this(action)
        {
            CanExecute = _ => canExecute();
        }

        public Action<object> Action { get; set; }

        public Func<object, bool> CanExecute { get; set; }

        protected override bool CheckCanExecute(object parameter)
        {
            return Action != null && (CanExecute == null || CanExecute(parameter));
        }

        protected override void DoExecute(object parameter)
        {
            Action(parameter);
        }
    }

    public interface ICancelCommand : ICommand
    {
        /// <summary>
        /// Indicates if command is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Error produced by last execution
        /// </summary>
        /// <value>The error.</value>
        Exception Error { get; }

        /// <summary>
        /// Execute command and return executing Task.
        /// </summary>
        /// <exception cref="InvalidOperationException">If command action run recursively</exception>
        Task ExecuteAsync(object parameter);
    }

    public interface IAsyncCommand : ICommand, INotifyPropertyChanged
    {
        /// <summary>
        /// Indicates if command is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Error produced by last execution
        /// </summary>
        /// <value>The error.</value>
        Exception Error { get; }

        /// <summary>
        /// Execute command and return executing Task.
        /// </summary>
        /// <exception cref="InvalidOperationException">If command action run recursively</exception>
        Task ExecuteAsync(object parameter);

        /// <summary>
        /// Command to cancel execution of a running command.
        /// </summary>
        ICancelCommand CancelCommand { get; }

        /// <summary>
        /// Cancellation token to be used with async operations to enable <see cref="CancelCommand"/>.
        /// </summary>
        CancellationToken Token { get; }
    }

    public static class AsyncCommandExtensions
    {
        public static Task ExecuteAsync(this IAsyncCommand command)
        {
            return command.ExecuteAsync(null);
        }

        public static Task ExecuteAsync(this ICancelCommand command)
        {
            return command.ExecuteAsync(null);
        }
    }

    public static class AsyncCommandProperties
    {
        public static IProperty<bool> IsRunningProperty<T>(this T command)
            where T : IAsyncCommand
        {
            return command.Property<T, bool>(nameof(command.IsRunning));
        }

        public static IProperty<bool> ErrorProperty<T>(this T command)
            where T : IAsyncCommand
        {
            return command.Property<T, bool>(nameof(command.Error));
        }
    }

    public abstract class AsyncCommandBase : CommandBase, IAsyncCommand
    {
        protected sealed override void DoExecute(object parameter)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ExecuteCommandAction(parameter);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public async Task ExecuteAsync(object parameter)
        {
            if (!CheckCanExecute(parameter))
            {
                if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug("Cannot 'ExecuteAsync' command '{command}', CanExecute return false", Name);
                }
                return;
            }
            if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug("'ExecuteAsync' command '{command}'", Name);
            }
            await ExecuteCommandAction(parameter);
            if (Error != null)
            {
                throw Error;
            }
        }

        private bool _recursiveExecution;

        private async Task ExecuteCommandAction(object parameter)
        {
            Assert.State(_recursiveExecution, Name).IsFalse("Cannot run command '{Key}' recursively");
            Error = null;
            IsRunningCount++;
            try
            {
                Task commandAction;
                try
                {
                    _recursiveExecution = true;
                    commandAction = CommandAction(parameter);
                }
                finally
                {
                    _recursiveExecution = false;
                }
                await commandAction;
            }
            catch (OperationCanceledException)
            {
                if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                    Logger.LogDebug("Command '{command}' execution canceled", Name);
            }
            catch (Exception e)
            {
                if (Logger != null && Logger.IsEnabled(LogLevel.Warning))
                    Logger.LogWarning("Command '{command}' execution finished with error: {error}", Name, e);
                Error = e;
            }
            finally
            {
                IsRunningCount--;
                if (_cancelCommand != null && _cancelCommand.IsRunning)
                {
                    if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                    {
                        Logger.LogDebug("Command '{command}' cancel command finished", Name);
                    }
                    _cancelCommand.IsRunning = false;
                }
                _cts = null;
            }
        }

        protected abstract Task CommandAction(object parameter);

        CancelAsyncCommand _cancelCommand;

        public ICancelCommand CancelCommand
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

        private CancellationTokenSource _cts;

        public bool HasCancellationTokenSource => _cts != null;

        public CancellationTokenSource CancellationTokenSource
        {
            get
            {
                if (_cts == null)
                    _cts = new CancellationTokenSource();
                return _cts;
            }
            set => _cts = value;
        }

        public CancellationToken Token => CancellationTokenSource.Token;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(PropertyChangedEventArgs key)
        {
            PropertyChanged?.Invoke(this, key);
        }

        protected void RaisePropertyChanged([CallerMemberName] string key = null)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(key));
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> key)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(PropertySupport.ExtractPropertyName(key)));
        }

        private int _isRunningCount;

        protected int IsRunningCount
        {
            get => _isRunningCount;
            set
            {
                _isRunningCount = value;
                IsRunning = _isRunningCount > 0;
            }
        }

        private bool _isRunning;

        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                if (_isRunning == value) return;
                _isRunning = value;
                RaisePropertyChanged();
                RaiseCanExecuteChanged();
            }
        }

        private void Cancel()
        {
            if (_cts == null || _cts.IsCancellationRequested || !IsRunning) return;
            try
            {
                _cts.Cancel();
            }
            finally
            {
                _cts = null;
            }            
        }

        public bool IsCancelRunning => _cancelCommand != null && _cancelCommand.IsRunning;

        private Exception _error;

        public Exception Error
        {
            get => _error;
            set
            {
                if (_error != value)
                {
                    _error = value;
                    RaisePropertyChanged();
                }
            }
        }

        private class CancelAsyncCommand : CommandBase, ICancelCommand
        {
            readonly AsyncCommandBase _owner;

            public CancelAsyncCommand(AsyncCommandBase owner)
            {
                _owner = owner;
                owner.PropertyChanged += Owner_IsRunningChanged;
            }

            protected override bool CheckCanExecute(object parameter)
            {
                return !IsRunning && _owner.IsRunning;
            }

            protected override void DoExecute(object parameter)
            {
                ExecuteCommandAction();
            }

            public async Task ExecuteAsync(object parameter)
            {
                if (!CheckCanExecute(parameter))
                {
                    return;
                }
                await ExecuteCommandAction();
            }

            TaskCompletionSource<object> _completionSource;

            private Task ExecuteCommandAction()
            {
                try
                {
                    if (_owner.Logger != null && _owner.Logger.IsEnabled(LogLevel.Debug))
                    {
                        _owner.Logger.LogDebug("Command '{command}' cancelation requested", _owner.Name);
                    }
                    IsRunning = true;
                    _owner.Cancel();
                }
                catch (ObjectDisposedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    if (_owner.Logger != null && _owner.Logger.IsEnabled(LogLevel.Debug))
                    {
                        if (e is OperationCanceledException)
                        {
                            _owner.Logger.LogDebug("Cancelation of '{command}' canceled", _owner.Name);
                        }
                        else
                        {
                            _owner.Logger.LogDebug("Cancellation of '{command}' finished with error: {error}",
                                _owner.Name,
                                e);
                        }
                    }
                    Error = e;
                    IsRunning = false;
                }
                return IsRunning ? _completionSource.Task : Task.FromResult<object>(null);
            }

            private void Owner_IsRunningChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(IsRunning))
                {
                    if (!_owner.IsRunning && IsRunning)
                    {
                        IsRunning = false;
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public bool IsRunning
            {
                get => _completionSource != null;
                set
                {
                    if (value != IsRunning)
                    {
                        if (value)
                        {
                            _completionSource = new TaskCompletionSource<object>();
                        }
                        else
                        {
                            _completionSource.TrySetResult(null);
                            _completionSource = null;
                        }
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
                        RaiseCanExecuteChanged();
                    }
                }
            }

            private Exception _error;

            public Exception Error
            {
                get => _error;
                private set
                {
                    if (value != _error)
                    {
                        _error = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
                    }
                }
            }
        }
    }

    public sealed class AsyncCommand : AsyncCommandBase
    {
        CancellationTokenSource _delayToken = new CancellationTokenSource();

        public AsyncCommand()
        {
        }

        public AsyncCommand(Func<object, Task> action) : this(action, null)
        {
        }

        public AsyncCommand(Func<object, Task> action, Func<object, bool> canExecute)
        {
            Action = action;
            if (canExecute != null)
                CanExecute = canExecute;
            else
                CanExecute = DefaultCanExecute;
            Delay = TimeSpan.Zero;
        }

        protected override async Task CommandAction(object parameter)
        {
            if (Action != null)
            {
                if (_delayToken != null && !_delayToken.IsCancellationRequested)
                    _delayToken.Cancel();
                if (Delay > TimeSpan.Zero)
                {
                    if (Logger != null && Logger.IsEnabled(LogLevel.Trace))
                    {
                        Logger.LogTrace("Command '{command}' delayed by {delay}", Name, Delay);
                    }
                    _delayToken = new CancellationTokenSource();
                    try
                    {
                        await Task.Delay(Delay, _delayToken.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        if (Logger != null && Logger.IsEnabled(LogLevel.Trace))
                        {
                            Logger.LogTrace(
                                "Command '{command}' execution canceled since another command execution started", Name);
                        }
                        return;
                    }
                    //check if action still can be executed after delay
                    if (!CheckCanExecute(parameter))
                    {
                        if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                        {
                            Logger.LogDebug("Cannot execute command '{command}', CanExecute return false after delay",
                                Name);
                        }
                        return;
                    }
                }

                if (CancelPrevious && _lastTask != null && !_lastTask.IsCompleted && HasCancellationTokenSource)
                {
                    if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                    {
                        Logger.LogDebug("Command '{command}' cancel previous command execution", Name);
                    }
                    try
                    {
                        CancellationTokenSource.Cancel();
                        await _lastTask;
                    }
                    catch (Exception e)
                    {
                        if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                        {
                            Logger.LogDebug(
                                "Command '{command}' previous execution cancelation finished with {error}", Name,
                                e);
                        }
                        //ignore
                    }
                    //Reset cancelation token source
                    CancellationTokenSource = null;

                    //check if action still can be executed after cancelation
                    if (!CheckCanExecute(parameter))
                    {
                        if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                        {
                            Logger.LogDebug(
                                "Cannot execute command '{command}', CanExecute return false after previous command execution cancelled",
                                Name);
                        }
                        return;
                    }
                }

                _lastTask = Action(parameter);
                Assert.State(_lastTask, Name).NotNull("Command '{Key}' Action returned null");
                await _lastTask;
            }
        }

        protected override bool CheckCanExecute(object parameter)
        {
            return Action != null && (CanExecute == null || CanExecute(parameter));
        }

        Func<object, Task> _action;

        public Func<object, Task> Action
        {
            get => _action;
            set => _action = value ?? throw new ArgumentNullException(nameof(value));
        }

        Func<object, bool> _canExecute;
        private Task _lastTask;

        public Func<object, bool> CanExecute
        {
            get => _canExecute;
            set => _canExecute = value ?? throw new ArgumentNullException(nameof(value));
        }

        public TimeSpan Delay { get; set; }

        public async Task ExecuteWithoutDelay(object param)
        {
            Task commandAction;
            var oldDelay = Delay;
            try
            {
                Delay = TimeSpan.Zero;
                commandAction = ExecuteAsync(param);
            }
            finally
            {
                Delay = oldDelay;
            }

            await commandAction;
        }

        private bool DefaultCanExecute(object arg)
        {
            return true;
        }

        public bool CancelPrevious { get; set; }
    }
}