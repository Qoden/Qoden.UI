using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Qoden.Util;

namespace Qoden.Binding
{
    public static class NotifyPropertyChangedProperties
    {
        public static IProperty Property(this INotifyPropertyChanged owner, string key)
        {
            return Property<INotifyPropertyChanged, object>(owner, key);
        }

        public static IProperty<T> Property<TOwner, T>(this TOwner owner, Expression<Func<TOwner, T>> key)
            where TOwner : INotifyPropertyChanged
        {
            return Property<TOwner, T>(owner, PropertySupport.ExtractPropertyName(key));
        }

        public static IProperty<T> Property<TOwner, T>(this TOwner owner, string key)
            where TOwner : INotifyPropertyChanged
        {
            return new Property<T>(owner, key, BindingStrategy);
        }

        public static readonly IPropertyBindingStrategy BindingStrategy = new NotifyPropertyChangedBindingStrategy();

        public static readonly RuntimeEvent PropertyChangedEvent = new RuntimeEvent(
            typeof(INotifyPropertyChanged),
            nameof(INotifyPropertyChanged.PropertyChanged));

        class NotifyPropertyChangedBindingStrategy : EventPropertyBindingStrategyBase
        {
            protected override RuntimeEvent GetEvent()
            {
                return PropertyChangedEvent;
            }

            protected override Delegate AdaptAction(IProperty property, Action<IProperty> action)
            {
                return new PropertyChangedEventHandler((s, e) =>
                {
                    if (e.PropertyName == property.Key)
                        action(property);
                });
            }
        }
    }
}