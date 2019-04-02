using System;
using System.Threading.Tasks;
using Qoden.Validation;

namespace Qoden.Binding
{
    public delegate void CommandBindingAction(ICommandBinding source);

    public interface ICommandBinding : IBinding
    {
        /// <summary>
        /// Event source which executes Source command
        /// </summary>
        ICommandTrigger Trigger { get; set; }

        /// <summary>
        /// Command to execute
        /// </summary>
        ICommand Command { get; set; }

        /// <summary>
        /// Action to update Target enabled/disabled status when command CanExecute changes
        /// </summary>
        CommandBindingAction UpdateTargetAction { get; set; }

        /// <summary>
        /// Action which is called right before Command Execute method is called
        /// </summary>
        CommandBindingAction BeforeExecuteAction { get; set; }

        /// <summary>
        /// Action which is called right after Command Execute method is called. 
        /// It does not wait until async command execution finish. For this see IAsyncCommandBinding interface.
        /// </summary>
        CommandBindingAction AfterExecuteAction { get; set; }

        /// <summary>
        /// Gets or sets the parameter to be passed to command Execute method. If not null then it overrides 
        /// any parameter passed to <see cref="Trigger"/> <see cref="ICommandTrigger.Trigger"/>.
        /// </summary>
        object Parameter { get; set; }
    }

    public interface IAsyncCommandBinding : ICommandBinding
    {
        /// <summary>
        /// Action to execute when command is started as defined by IsRunning property
        /// </summary>
        CommandBindingAction CommandStarted { get; set; }
        /// <summary>
        /// Action to execute when command is finished as defined by IsRunning property
        /// </summary>
        CommandBindingAction CommandFinished { get; set; }
    }

    public class CommandBinding : IAsyncCommandBinding
    {
        public CommandBinding()
        {
            UpdateTargetAction = DefaultUpdateTargetAction;
            Enabled = true;
        }

        public CommandBindingAction UpdateTargetAction { get; set; }

        public CommandBindingAction BeforeExecuteAction { get; set; }

        public CommandBindingAction AfterExecuteAction { get; set; }

        public CommandBindingAction CommandStarted { get; set; }

        public CommandBindingAction CommandFinished { get; set; }

        public object Parameter { get; set; }

        private static void DefaultUpdateTargetAction(ICommandBinding binding)
        {
            if (binding.Trigger == null)
                return;

            var parameter = binding.Parameter;
            binding.Trigger.SetEnabled(binding.Command.CanExecute(parameter));
        }

        public void Bind()
        {
            if (Bound)
                return;

            OnBind();
            Bound = true;
        }

        protected virtual void OnBind()
        {
            Assert.State(Command).NotNull("Source is not set");
            Command.CanExecuteChanged += Source_CanExecuteChanged;

            if (Trigger != null)
                Trigger.Trigger += Target_ExecuteCommand;
        }

        public void Unbind()
        {
            if (!Bound)
                return;

            OnUnbind();
            Bound = false;
        }

        protected virtual void OnUnbind()
        {
            Command.CanExecuteChanged -= Source_CanExecuteChanged;
            if (Trigger != null)
                Trigger.Trigger -= Target_ExecuteCommand;
        }

        private void Source_CanExecuteChanged(object sender, EventArgs e)
        {
            UpdateTarget();
        }

        private async Task Target_ExecuteCommand(object parameter)
        {
            if (!Enabled)
                return;

            parameter = Parameter ?? parameter;
            var asyncCommand = Command as IAsyncCommand;
            var isAsyncCommand = asyncCommand != null;
            Task execution = null;
            
            BeforeExecuteAction?.Invoke(this);
            CommandStarted?.Invoke(this);
            try
            {
                if (isAsyncCommand)
                {
                    execution = asyncCommand.ExecuteAsync();
                }
                else
                {
                    Command.Execute(parameter);
                }
            }
            finally
            {
                AfterExecuteAction?.Invoke(this);
                if (isAsyncCommand && execution != null)
                    await execution;
                CommandFinished?.Invoke(this);
            }
        }

        public void UpdateTarget()
        {
            if (Enabled)
            {
                UpdateTargetAction?.Invoke(this);
            }
        }

        public void UpdateSource() => throw new NotSupportedException();

        private ICommandTrigger _target;

        public ICommandTrigger Trigger
        {
            get => _target;
            set
            {
                Assert.State(Bound, "Bound").IsFalse();
                _target = value;
            }
        }

        private ICommand _source;

        public ICommand Command
        {
            get => _source;
            set
            {
                Assert.State(Bound, "Bound").IsFalse();
                _source = value;
            }
        }

        public bool Enabled { get; set; }

        public bool Bound { get; private set; }
    }

}