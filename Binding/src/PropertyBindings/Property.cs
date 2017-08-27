using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Linq.Expressions;
using Qoden.Util;

namespace Qoden.Binding
{
    /// <summary>
    /// A property which can be managed by binding engine
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// Object containing this property
        /// </summary>
        object Owner { get; }

        /// <summary>
        /// Property name
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the binding strategy to be user to detect changes in this property.
        /// </summary>
        /// <value>The binding strategy.</value>
        IPropertyBindingStrategy BindingStrategy { get; }

        /// <summary>
        /// Gets or sets underlying property value.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Property type
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// Gets a value indicating whether property is read only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets a value indicating whether there are any errors related to underlying property.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Gets the errors (if any) related to underlying property.
        /// </summary>
        IEnumerable Errors { get; }

        /// <summary>
        /// Subscribes action to a property change event and return subscription object.
        /// Caller can dispose returned object to unsubscribe action from event.
        /// </summary>
        IDisposable OnPropertyChange(Action<IProperty> action);
    }

    public interface IProperty<T> : IProperty
    {
        new T Value { get; set; }
    }

    public static class PropertyExtensions
    {
        public static Property<TPropertyType> GetProperty<T, TPropertyType>(
            this T owner,
            string propertyName,
            IPropertyBindingStrategy bindingStrategy = null,
            Func<T, TPropertyType> getter = null,
            Action<T, TPropertyType> setter = null)
        {
            Action<object, TPropertyType> mySetter = null;
            if (setter != null)
            {
                mySetter = (o, v) => setter((T) o, v);
            }
            Func<object, TPropertyType> myGetter = null;
            if (getter != null)
            {
                myGetter = o => getter((T) o);
            }

            return new Property<TPropertyType>(owner, propertyName, bindingStrategy, myGetter, mySetter);
        }

        public static Property<TPropertyType> GetProperty<T, TPropertyType>(
            this T owner,
            Expression<Func<T, TPropertyType>> key,
            IPropertyBindingStrategy bindingStrategy = null,
            Func<T, TPropertyType> getter = null,
            Action<T, TPropertyType> setter = null)
        {
            var propertyName = PropertySupport.ExtractPropertyName(key);
            return owner.GetProperty(propertyName, bindingStrategy, getter, setter);
        }
    }

    public class Property<TPropertyType> : IProperty<TPropertyType>
    {
        private readonly Func<object, TPropertyType> _getter;
        private readonly Action<object, TPropertyType> _setter;

        public Property(
            object owner,
            string key,
            IPropertyBindingStrategy bindingStrategy = null,
            Func<object, TPropertyType> getter = null,
            Action<object, TPropertyType> setter = null)
        {
            Owner = owner;
            Key = key;
            _getter = getter;
            _setter = setter;
            BindingStrategy = bindingStrategy;
        }

        public IPropertyBindingStrategy BindingStrategy { get; }

        public object Owner { get; }

        public string Key { get; }

        public TPropertyType Value
        {
            get => _getter != null ? _getter(Owner) : GetValue();
            set
            {
                if (_setter != null)
                {
                    _setter(Owner, value);
                }
                else
                {
                    SetValue(value);
                }
            }
        }

        /// <summary>
        /// Override this method to customize how property value is get from Owner
        /// </summary>
        protected virtual void SetValue(TPropertyType value)
        {
            KeyValueCoding.Impl(Owner).Set(Owner, Key, value);
        }

        /// <summary>
        /// Override this method to customize how property value is set.
        /// </summary>
        protected virtual TPropertyType GetValue()
        {
            return (TPropertyType) KeyValueCoding.Impl(Owner).Get(Owner, Key);
        }


        public bool HasErrors
        {
            get
            {
                var e = (IEnumerable<object>) Errors;
                return e.Any();
            }
        }

        public virtual IEnumerable Errors
        {
            get
            {
                var errorInfo = Owner as INotifyDataErrorInfo;
                if (errorInfo != null)
                {
                    return errorInfo.GetErrors(Key);
                }
                else
                {
                    return Enumerable.Empty<object>();
                }
            }
        }

        /// <summary>
        /// Determines whether this instance represent same property as given Expression.
        /// </summary>
        public bool Is<TOwnerType>(Expression<Func<TOwnerType, TPropertyType>> property)
        {
            return PropertySupport.ExtractPropertyName(property) == Key;
        }

        object IProperty.Owner => Owner;

        object IProperty.Value
        {
            get => Value;
            set => Value = (TPropertyType) value;
        }

        public virtual bool IsReadOnly => KeyValueCoding.Impl(Owner).IsReadonly(Owner, Key);

        Type IProperty.PropertyType => typeof(TPropertyType);

        IDisposable IProperty.OnPropertyChange(Action<IProperty> action)
        {
            if (BindingStrategy == null)
                throw new InvalidOperationException("Property '" + this.Key +
                                                    "' does not support change tracking (BindingStrategy is null).");
            return BindingStrategy.SubscribeToPropertyChange(this, action);
        }
    }
}