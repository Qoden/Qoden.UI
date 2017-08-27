using System;

namespace Qoden.Binding
{
    /// <summary>
    /// Operation on a View Model (<see cref="DataContext"/>)
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Raised when <see cref="CanExecute"/> changes
        /// </summary>
        event EventHandler CanExecuteChanged;

        /// <summary>
        /// Indicates that command can or cannot be executed.
        /// </summary>
        /// <param name="parameter">command parameter, can be null</param>
        bool CanExecute(object parameter);

        /// <summary>
        /// Executes command with given parameter.
        /// </summary>
        /// <param name="parameter">command parameter</param>
        void Execute(object parameter);
    }
    
    public static class CommandExtensions
    {
        /// <summary>
        /// Execute <see cref="ICommand"/> with null as parameter.
        /// </summary>
        public static void Execute(this ICommand command)
        {
            command.Execute(null);
        }

        /// <summary>
        /// Run <see cref="ICommand.CanExecute"/> with null as parameter.
        /// </summary>
        public static bool CanExecute(this ICommand command)
        {
            return command.CanExecute(null);
        }
    }

}