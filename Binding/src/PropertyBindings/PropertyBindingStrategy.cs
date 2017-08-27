using System;
using Qoden.Util;
using Qoden.Validation;

namespace Qoden.Binding
{
	/// <summary>
	/// Provides a way to subscribe to property change events.
	/// </summary>
	public interface IPropertyBindingStrategy
	{		
		/// <summary>
		/// Subscribes <see cref="action"/> to property change event and returns subscription object.
		/// Caller can dispose returned object to unsubscribe action from event.
		/// </summary>
		IDisposable SubscribeToPropertyChange (IProperty property, Action<IProperty> action);
	}

	/// <summary>
	/// Base class for <see cref="IPropertyBindingStrategy"/> classes which listens to .NET events to detect changes 
	/// in properties.
	/// </summary>
	public abstract class EventPropertyBindingStrategyBase : IPropertyBindingStrategy
	{
		public IDisposable SubscribeToPropertyChange (IProperty property, Action<IProperty> action)
		{
			Assert.Argument (property, "property").NotNull ();
			Assert.Argument (action, "action").NotNull ();
			return new EventSubscription (GetEvent(), AdaptAction(property, action), property.Owner);
		}

		protected abstract RuntimeEvent GetEvent();
		protected abstract Delegate AdaptAction(IProperty property, Action<IProperty> action);
	}

	public abstract class EventHandlerBindingStrategyBase : EventPropertyBindingStrategyBase
	{
		readonly RuntimeEvent _event;

		protected EventHandlerBindingStrategyBase(RuntimeEvent @event)
		{
			Assert.Argument(@event, nameof(@event)).NotNull();
			_event = @event;
		}

		protected sealed override RuntimeEvent GetEvent()
		{
			return _event;
		}
	}

	/// <summary>
	/// Subscribes to property changes by adding event listener to provided property. 
	/// </summary>
	public class EventHandlerBindingStrategy : EventHandlerBindingStrategyBase
	{		
		/// <summary>
		/// Creates <see cref="IPropertyBindingStrategy"/> which subscribes to <see cref="@event"/> to detect 
		/// property changes
		/// </summary>
		public EventHandlerBindingStrategy (RuntimeEvent @event) : base(@event)
		{
		}

		protected override Delegate AdaptAction(IProperty property, Action<IProperty> action)
		{
			return new EventHandler ((s, e) => action(property));
		}
	}

	/// <summary>
	/// Subscribes to property changes by adding event listener to provided property.
	/// </summary>
	public class EventHandlerBindingStrategy<T> : EventHandlerBindingStrategyBase where T : EventArgs
	{
		/// <summary>
		/// Creates <see cref="IPropertyBindingStrategy"/> which subscribes to <see cref="@event"/> to detect 
		/// property changes
		/// </summary>
		public EventHandlerBindingStrategy(RuntimeEvent @event) : base(@event)
		{
		}

		protected override Delegate AdaptAction(IProperty property, Action<IProperty> action)
		{
			return new EventHandler<T>((s, e) => action(property));
		}
	}
}