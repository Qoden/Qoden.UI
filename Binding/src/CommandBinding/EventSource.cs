using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Qoden.Util;
using Qoden.Validation;

namespace Qoden.Binding
{

    public delegate Task CommandHandler(object parameter); 
    
    /// <summary>
    /// Represent event in an object. This could be .NET event or something more complicated like NSNotificationCenter
    /// subscription in iOS.
    /// </summary>
    public interface ICommandTrigger
    {
        /// <summary>
        /// Proxy event handler to subscribe/unsubscribe underlying event.
        /// </summary>
        event CommandHandler Trigger;

        /// <summary>
        /// Enable/Disable <see cref="ICommandTrigger"/>. It only raise <see cref="Trigger"/> if enabled.
        /// </summary>
        void SetEnabled(bool enabled);
    }
    
    /// <summary>
    /// ICommandTrigger for .NET events
    /// </summary>
    public class EventCommandTrigger : ICommandTrigger
    {
        private readonly RuntimeEvent _event;
        private readonly Delegate _eventHandler;

        private static readonly MethodInfo HandleOwnerEventMethodInfo = typeof(EventCommandTrigger)
            .GetMethod(nameof(HandleOwnerEvent), BindingFlags.NonPublic | BindingFlags.Instance);

        private CommandHandler _handler;
        private int _subscriptions;

        public EventCommandTrigger(RuntimeEvent @event, object owner)
        {
            _event = @event;
            Owner = owner;

            _eventHandler = @event.CreateDelegate(this, HandleOwnerEventMethodInfo);
        }

        [Preserve]
        private void HandleOwnerEvent(object sender, EventArgs args)
        {
            var parameter = ParameterExtractor?.Invoke(sender, args);
            _handler?.Invoke(parameter);
        }

        public void SetEnabled(bool enabled)
        {
            SetEnabledAction?.Invoke(Owner, enabled);
        }
        /// <summary>
        /// Func to extract <see cref="ICommand"/> <see cref="ICommand.Execute"/> parameter from Target EventArgs.
        /// EventArgs could be EventArgs.Empty or corresponding event arguments.
        /// </summary>
        public Func<object, EventArgs, object> ParameterExtractor { get; set; }

        public object Owner { get; }

        public Action<object, bool> SetEnabledAction { get; set; }

        public event CommandHandler Trigger
        {
            add
            {
                _handler += value;
                if (_subscriptions == 0)
                    _event.AddEventHandler(Owner, _eventHandler);
                _subscriptions++;
            }
            remove
            {
                // ReSharper disable once DelegateSubtraction
                _handler -= value;
                _subscriptions--;
                if (_subscriptions == 0)
                    _event.RemoveEventHandler(Owner, _eventHandler);
            }
        }
    }

    /// <summary>
    /// EventListSource allows to subscribe to an event of multiple objects.
    /// T is the type of object with the event
    /// </summary>
    public class EventListCommandTrigger<T> : ICommandTrigger
    {
        private readonly RuntimeEvent _event;
        private readonly Delegate _eventHandler;

        private static readonly MethodInfo HandleOwnerEventMethodInfo = typeof(EventListCommandTrigger<T>)
            .GetMethod(nameof(HandleOwnerEvent), BindingFlags.NonPublic | BindingFlags.Instance);

        private CommandHandler _handler;
        private int _subscriptions;

        public EventListCommandTrigger(RuntimeEvent @event)
        {
            _event = @event;
            _eventHandler = @event.CreateDelegate(this, HandleOwnerEventMethodInfo);
        }

        /// <summary>
        /// Listen to the event on this object
        /// </summary>
        /// <param name="owner">Object to subscribe to</param>
        public void Listen(T owner)
        {
            Assert.Argument(owner, nameof(owner)).NotNull();
            Owners.Add(owner);
            if (_subscriptions > 0)
            {
                _event.AddEventHandler(owner, _eventHandler);
            }
        }
        
        [Preserve]
        private void HandleOwnerEvent(object sender, EventArgs args)
        {
            var parameter = ParameterExtractor?.Invoke(sender, args);
            _handler?.Invoke(parameter);
        }

        public void SetEnabled(bool enabled)
        {
            if (SetEnabledAction != null)
            {
                foreach (var owner in Owners)
                {
                    SetEnabledAction(owner, enabled);
                }
            }
        }
        
        /// <summary>
        /// Func to extract <see cref="ICommand"/> <see cref="ICommand.Execute"/> parameter from Target EventArgs.
        /// EventArgs could be EventArgs.Empty or corresponding event arguments.
        /// </summary>
        public Func<object, EventArgs, object> ParameterExtractor { get; set; }

        public List<T> Owners { get; } = new List<T>();

        public Action<object, bool> SetEnabledAction { get; set; }

        public event CommandHandler Trigger
        {
            add
            {
                _handler += value;
                if (_subscriptions == 0)
                {
                    foreach (var owner in Owners)
                    {
                        _event.AddEventHandler(owner, _eventHandler);
                    }
                }
                _subscriptions++;
            }
            remove
            {
                // ReSharper disable once DelegateSubtraction
                _handler -= value;
                _subscriptions--;
                if (_subscriptions == 0)
                {
                    foreach (var owner in Owners)
                    {
                        _event.RemoveEventHandler(owner, _eventHandler);
                    }
                }
            }
        }
    }
}