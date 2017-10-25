using System;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace Qoden.Binding
{
    /// <summary>
    /// Async operation on a View Model (<see cref="DataContext"/>).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Command can control concurrent executions by checking <see cref="Task"/> property when 
    /// run. If <see cref="Task"/> is not null then another execution is already running. Command then can do many things 
    /// including
    /// <list type="number">
    ///     <item>
    ///     <term>Wait</term>
    ///     <description> - wait until <see cref="Task"/> finish and continue</description>
    ///     </item>
    ///     <item>
    ///     <term>Join</term>
    ///     <description> - do nothing and return <see cref="Task"/></description>
    ///     </item>
    ///     <item>
    ///     <term>Cancel</term>
    ///     <description> - cancel <see cref="Task"/> using implementation specific API</description>
    ///     </item>
    /// </list> 
    /// </para>
    /// <para>
    /// Async command can be canceled with <see cref="CancelCommand"/>. Right after a call to <see cref="CancelCommand"/>
    ///  <see cref="ICommand.Execute"/> method command could be either still running with <see cref="IsCancellationRequested"/> 
    /// flag or already finished with <see cref="IsRunning"/> equal to false and <see cref="Task"/> equal to null.
    /// <see cref="CancelCommand"/> cancels current and all pending executions (if command is waiting for them internaly).
    /// </para>
    /// </remarks>
    public interface IAsyncCommand : ICommand, INotifyPropertyChanged
    {
        /// <summary>
        /// Indicates if command is running (see <see cref="IAsyncCommand"/> remarks section for details).
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Running command task. Not null when command is running (see <see cref="IsRunning"/>, also 
        /// see <see cref="IAsyncCommand"/> remarks section for details). 
        /// </summary>
        Task Task { get; }
        
        /// <summary>
        /// Error produced by last execution.
        /// </summary>
        Exception Error { get; }

        /// <summary>
        /// Command to cancel execution of a running command. 
        /// </summary>
        /// <remarks>
        /// Command could be still running (see <see cref="IsRunning"/>) after a call to <see cref="CancelCommand"/> 
        /// <see cref="ICommand.Execute"/>. if you want to make sure it is finshed then use snippet below.
        /// </remarks>
        /// <code>
        /// var command = new AsyncCommand() { ... };
        /// command.Execute();
        /// command.CancelCommand.Execute();
        /// await command.WhenFinished();
        /// </code>
        ICommand CancelCommand { get; } 

        /// <summary>
        /// Indicates that cancel command executed and <see cref="Task"/> awaiting cancellation.
        /// </summary>
        bool IsCancellationRequested { get; }
    }
    
    public static class AsyncCommandExtensions
    {
        public static async Task ExecuteAsync(this IAsyncCommand command, object parameter, bool throwOnError = false)
        {
            command.Execute(parameter);
            var task = command.Task;
            if (task != null)
            {
                try
                {
                    await task;
                }
                catch (Exception)
                {
                    if (throwOnError)
                        throw;
                }
            }
        }

        public static Task ExecuteAsync(this IAsyncCommand command, bool throwOnError = false)
        {
            return command.ExecuteAsync(null, throwOnError);
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

        public static Task WhenFinished(this IAsyncCommand command)
        {
            if (command.IsRunning)
            {
                var tcs = new TaskCompletionSource<object>();
                void Done(object sender, PropertyChangedEventArgs p)
                {
                    if (!command.IsRunning)
                    {
                        tcs.SetResult(null);
                        command.PropertyChanged -= Done;
                    }
                }

                command.PropertyChanged += Done;
                return tcs.Task;
            }
            return Task.FromResult(0);
        }        
    }
}