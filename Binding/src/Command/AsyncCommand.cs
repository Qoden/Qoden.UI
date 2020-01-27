using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qoden.Binding
{
    [Flags]
    public enum AsyncCommandSerializationPolicy
    {
        /// <summary>
        /// Do nothing
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Cancel running command task.
        /// </summary>
        Cancel = 1,

        /// <summary>
        /// Wait until command task finish.
        /// </summary>
        Wait = 2,
    }

    public sealed class AsyncCommand : AsyncCommandBase
    {
        private CancellationTokenSource _delayToken;
        
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
        }

        protected override async Task ExecuteAsyncCommand(object parameter, CancellationToken token)
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
                    _delayToken = new CancellationTokenSource();                
                    await Task.Delay(Delay, _delayToken.Token);
                }
                
                if (Task != null)
                {
                    if ((SerializationPolicy & AsyncCommandSerializationPolicy.Cancel) != 0)
                    {
                        CancelRunningTask();
                    }
                    if ((SerializationPolicy & AsyncCommandSerializationPolicy.Wait) != 0)
                    {
                        try
                        {
                            await Task;
                        }
                        catch
                        {
                            //ignore
                        }
                    }
                }

                if (IsCanceled) return;
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

        private bool DefaultCanExecute(object arg)
        {
            return true;
        }
        
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;
        
        public AsyncCommandSerializationPolicy SerializationPolicy { get; set; } = AsyncCommandSerializationPolicy.None;
    }


    public class AsyncCommand<T> : AsyncCommandBase
    {
        private CancellationTokenSource _delayToken;
        public T Result { get; private set; }

        public AsyncCommand()
        {
        }

        public AsyncCommand(Func<object, CancellationToken, Task<T>> action) : this(action, null)
        {
        }

        public AsyncCommand(Func<object, CancellationToken, Task<T>> action, Func<object, bool> canExecute)
        {
            Action = action;
            if (canExecute != null)
                CanExecute = canExecute;
            else
                CanExecute = DefaultCanExecute;
        }

        protected override async Task ExecuteAsyncCommand(object parameter, CancellationToken token)
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
                    _delayToken = new CancellationTokenSource();
                    await Task.Delay(Delay, _delayToken.Token);
                }

                if (Task != null)
                {
                    if ((SerializationPolicy & AsyncCommandSerializationPolicy.Cancel) != 0)
                    {
                        CancelRunningTask();
                    }
                    if ((SerializationPolicy & AsyncCommandSerializationPolicy.Wait) != 0)
                    {
                        try
                        {
                            await Task;
                        }
                        catch
                        {
                            //ignore
                        }
                    }
                }

                if (IsCanceled)
                    return;

                Result = await Action(parameter, token);
            }
        }

        protected override bool CanExecuteCommand(object parameter)
        {
            return Action != null && (CanExecute == null || CanExecute(parameter));
        }

        Func<object, CancellationToken, Task<T>> _action;

        public Func<object, CancellationToken, Task<T>> Action
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

        private bool DefaultCanExecute(object arg)
        {
            return true;
        }

        public TimeSpan Delay { get; set; } = TimeSpan.Zero;

        public AsyncCommandSerializationPolicy SerializationPolicy { get; set; } = AsyncCommandSerializationPolicy.None;
    }
}