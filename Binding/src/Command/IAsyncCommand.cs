using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Qoden.Binding
{
    /// <summary>
    /// Async operation on a View Model (<see cref="DataContext"/>).
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <see cref="IAsyncCommand"/> instances going thru certain state transitions during execution.
    ///     <list type="number">
    ///     <item>
    ///         <description><see cref="ICommand.Execute"/> moves command to <see cref="IsRunning"/> state.</description>
    ///     </item>
    ///     <item>
    ///         <description>Command starts it <see cref="Task"/> and start command action execution (see <see cref="CommandExecution"/>).</description>
    ///     </item>
    ///     <item>
    ///         <description>Subsequent calls to <see cref="ICommand.Execute"/> await, terminate or join <see cref="CommandExecution"/>.</description>
    ///     </item>
    ///     <item>
    ///         <description>When last <see cref="CommandExecution"/> finishes command transition to not running state as indicated by <see cref="IsRunning"/>.</description>
    ///     </item>
    ///     </list>
    /// </para>
    /// <para>
    ///     
    /// </para>
    /// <para>
    /// Async command can be canceled with <see cref="CancelCommand"/>. Right after a call to <see cref="CancelCommand"/>
    ///  <see cref="ICommand.Execute"/> method command could be either still running with <see cref="IsCancellationRequested"/> flag  
    /// </para>
    /// </remarks>
    public interface IAsyncCommand : ICommand, INotifyPropertyChanged
    {
        /// <summary>
        /// Indicates if command is running (see <see cref="IAsyncCommand"/> remarks section for details).
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Running command task. Not null when command is running (see <see cref="IsRunning"/>, also see <see cref="IAsyncCommand"/> remarks section for details). 
        /// </summary>
        Task Task { get; }
        
        /// <summary>
        /// Running command execution. It is different from <see cref="Task"/> since command might have several 
        /// <see cref="CommandExecution"/> as part of a single <see cref="Task"/> if <see cref="ICommand.Execute"/>
        /// called multiple times while command is running (see <see cref="IAsyncCommand"/> remarks section for details).
        /// </summary>
        Task CommandExecution { get; }

        /// <summary>
        /// Error produced by last execution (see <see cref="CommandExecution"/>).
        /// </summary>
        Exception Error { get; }

        /// <summary>
        /// Command to cancel execution of a running command. 
        /// </summary>
        /// <remarks>
        /// Command could be still running (see <see cref="IsRunning"/>) after a call to <see cref="CancelCommand"/> 
        /// <see cref="ICommand.Execute"/>. if you want to make sure it is finshed then wait for <see cref="Task"/> 
        /// and make sure you handle <see cref="OperationCanceledException"/>.       
        /// </remarks>
        /// <code>
        /// var command = new AsyncCommand() { ... };
        /// command.Execute();
        /// command.CancelCommand.Execute();
        /// try 
        /// {
        ///     if (command.Task != null) await command.Task;
        /// } 
        /// catch(OperationCanceledException)
        /// {
        /// }
        /// </code>
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
            var task = command.Task;
            if (task == null) //means command finished execution right away
            {
                if (command.Error != null)
                {
                    task = Task.FromException(command.Error);
                }
                else
                {
                    task = Task.FromResult(0);
                }
            }
            return task;
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