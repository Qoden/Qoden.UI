using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Qoden.Binding
{
    /// <summary>
    /// Async operation on a View Model (<see cref="DataContext"/>)
    /// </summary>
    public interface IAsyncCommand : ICommand, INotifyPropertyChanged
    {
        /// <summary>
        /// Indicates if command is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Running task. Not null is command is running (<see cref="IsRunning"/> == true)
        /// </summary>
        Task Task { get; }

        /// <summary>
        /// Error produced by last execution.
        /// </summary>
        Exception Error { get; }

        /// <summary>
        /// Command to cancel execution of a running command.
        /// </summary>
        ICommand CancelCommand { get; }

        /// <summary>
        /// Indicates that cancel command executed and <see cref="Task"/> awaiting cancellation.
        /// </summary>
        bool IsCancellationRequested { get; }
    }
    
    public static class AsyncCommandExtensions
    {
        public static Task ExecuteAsync(this IAsyncCommand command, object parameter)
        {
            command.Execute(parameter);
            return command.Task;
        }

        public static Task ExecuteAsync(this IAsyncCommand command)
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
}