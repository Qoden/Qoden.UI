using System;

namespace Qoden.Binding
{
	public static class CommandBindingBuilder
	{
		public struct SourceBuilder<T>
			where T: ICommand
		{
			public readonly ICommandBinding Binding;

			internal SourceBuilder (T command) : this (new CommandBinding {
					Command = command
				})
			{
			}

			internal SourceBuilder (ICommandBinding binding)
			{
                Binding = binding ?? throw new ArgumentNullException(nameof(binding));
			}

			/// <summary>
			/// Set <see cref="ICommandBinding"/> <see cref="ICommandBinding.BeforeExecuteAction"/>
			/// </summary>
			public SourceBuilder<T> BeforeExecute (CommandBindingAction action)
			{
				Binding.BeforeExecuteAction = action;
				return this;
			}

			/// <summary>
			/// Set <see cref="ICommandBinding"/> <see cref="ICommandBinding.AfterExecuteAction"/>
			/// </summary>
			public SourceBuilder<T> AfterExecute (CommandBindingAction action)
			{
				Binding.AfterExecuteAction = action;
				return this;
			}

			/// <summary>
			/// Set <see cref="IAsyncCommandBinding"/> <see cref="IAsyncCommandBinding.CommandStarted"/>
			/// </summary>
			public SourceBuilder<T> WhenStarted (CommandBindingAction action)
			{
				var b = (IAsyncCommandBinding)Binding;
				b.CommandStarted = action;
				return this;
			}

			/// <summary>
			/// Set <see cref="IAsyncCommandBinding"/> <see cref="IAsyncCommandBinding.CommandFinished"/>
			/// </summary>
			public SourceBuilder<T> WhenFinished (CommandBindingAction action)
			{
				var b = (IAsyncCommandBinding)Binding;
				b.CommandFinished = action;
				return this;
			}

			/// <summary>
			/// Set <see cref="IBinding"/> <see cref="IBinding.Enabled"/> to false
			/// </summary>
			public SourceBuilder<T> Disabled ()
			{
				Binding.Enabled = false;
				return this;
			}

			/// <summary>
			/// Set <see cref="ICommandBinding"/> <see cref="ICommandBinding.Trigger"/> and command 
			/// parameter (see <see cref="ICommand.Execute"/>)
			/// </summary>
			/// <param name="source">Event source</param>
			/// <param name="parameter">Optional command parameter (see <see cref="ICommand.Execute"/>)</param>
			public void To (ICommandTrigger source, object parameter = null)
			{
				Binding.Trigger = source;
				Binding.Parameter = parameter;
			}		
        }

		/// <summary>
		/// Configure <see cref="ICommandBinding"/> object.
		/// </summary>
		/// <param name="list">Binding list</param>
		/// <param name="command">Source command</param>
		public static SourceBuilder<T> Command<T> (this BindingList list, T command)
			where T : ICommand
		{
			var builder = new SourceBuilder<T> (command);
			list.Add (builder.Binding);
			return builder;
		}
	}
}
