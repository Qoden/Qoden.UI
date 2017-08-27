using System;
using Microsoft.Extensions.Logging;

namespace Qoden.Binding
{
    public class Command : CommandBase
    {
        public Action<object> Action { get; set; }

        public Func<object, bool> CanExecute { get; set; }

        protected override bool CanExecuteCommand(object parameter)
        {
            return Action != null && (CanExecute == null || CanExecute(parameter));
        }

        protected override void ExecuteCommand(object parameter)
        {
            Action(parameter);
        }
    }

    /// <summary>
    /// Base class for simple command implementations.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        /// <summary>
        /// Override this method to provide custome <see cref="ICommand.CanExecute"/> logic.
        /// </summary>
        /// <param name="parameter">parameter passed to <see cref="ICommand.CanExecute"/></param>
        protected virtual bool CanExecuteCommand(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Command execution logic.
        /// </summary>
        /// <param name="parameter">parameter passed to <see cref="ICommand.Execute"/></param>
        protected abstract void ExecuteCommand(object parameter);

        /// <summary>
        /// Logger to be trace command execution
        /// </summary>
        public ILogger Logger { get; set; }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecuteCommand(parameter);
        }

        public void Execute(object parameter)
        {
            if (CanExecuteCommand(parameter))
            {
                if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug("Execute command");
                }
                ExecuteCommand(parameter);
            }
            else
            {
                if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug("Cannot Execute command, CanExecute return false");
                }
            }
        }
    }
}