using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Qoden.Validation;

namespace Qoden.Binding
{
    /// <summary>
    /// Methods to configure <see cref="IPropertyBinding"/> <see cref="IPropertyBinding.Source"/> related properties.
    /// </summary>
    public struct PropertyBindingSourceBuilder<TSource>
    {
        public PropertyBindingSourceBuilder(IPropertyBinding binding)
        {
            Binding = binding ?? throw new ArgumentNullException(nameof(binding));
        }

        public IPropertyBinding Binding { get; }

        /// <summary>
        /// Set <see cref="IPropertyBinding"/> <see cref="IBinding.UpdateTarget"/> action. This is useful when no real 
        /// <see cref="IPropertyBinding.Target"/> is available.
        /// </summary>
        /// <param name="action">update target action</param>
        public void UpdateTarget(PropertyBindingAction action)
        {
            Binding.UpdateTargetAction = action;
        }

        /// <summary>
        /// Set action to call right before <see cref="IBinding.UpdateTarget"/>.
        /// </summary>
        /// <param name="action">before update target action</param>
        public PropertyBindingSourceBuilder<TSource> BeforeTargetUpdate(PropertyBindingAction action)
        {
            var oldAction = Binding.UpdateTargetAction;
            Binding.UpdateTargetAction = (b, source) =>
            {
                action(b, source);
                oldAction(b, source);
            };
            return this;
        }

        /// <summary>
        /// Set action to call right after <see cref="IBinding.UpdateTarget"/>.
        /// </summary>
        /// <param name="action">after update target action</param>
        public PropertyBindingSourceBuilder<TSource> AfterTargetUpdate(PropertyBindingAction action)
        {
            var oldAction = Binding.UpdateTargetAction;
            Binding.UpdateTargetAction = (b, change) =>
            {
                oldAction(b, change);
                action(b, change);
            };
            return this;
        }

        /// <summary>
        /// Convert source property into property with different type. 
        /// If <see cref="from"/> is not provided then resulting property will be readonly (see <see cref="IProperty.IsReadOnly"/>) 
        /// and binding will be one way to target (see <see cref="PropertyBindingTargetBuilder{T}.OneWay"/>).
        /// </summary>
        /// <param name="to">Conversion from <see cref="IPropertyBinding.Source"/> to <see cref="IPropertyBinding.Target"/></param>
        /// <param name="from">Conversion from <see cref="IPropertyBinding.Target"/> to <see cref="IPropertyBinding.Source"/></param>
        public PropertyBindingSourceBuilder<TNew> Convert<TNew>(Func<TSource, TNew> to, Func<TNew, TSource> from = null)
        {
            var sp = (IProperty<TSource>) Binding.Source;
            Binding.Source = sp.Convert(to, from);
            if (from == null)
            {
                Binding.DontUpdateSource();
            }
            return new PropertyBindingSourceBuilder<TNew>(Binding);
        }

        /// <summary>
        /// Returns builder object to configure target binding properties (see <see cref="IPropertyBinding.Target"/>). 
        /// </summary>
        /// <param name="targetProperty">nding Bi<see cref="IPropertyBinding.Target"/> value</param>
        public PropertyBindingTargetBuilder<TSource> To(IProperty<TSource> targetProperty)
        {
            Binding.Target = targetProperty;
            return new PropertyBindingTargetBuilder<TSource>(Binding);
        }
    }

    /// <summary>
    /// Methods to configure <see cref="IPropertyBinding"/> <see cref="IPropertyBinding.Target"/> related properties.
    /// </summary>
    public struct PropertyBindingTargetBuilder<T>
    {
        readonly IPropertyBinding _binding;

        public PropertyBindingTargetBuilder(IPropertyBinding binding)
        {
            _binding = binding;
        }

        /// <summary>
        /// Set <see cref="IBinding"/> <see cref="IBinding.UpdateTarget"/> action
        /// </summary>
        /// <param name="action">update target action</param>
        public PropertyBindingTargetBuilder<T> UpdateTarget(PropertyBindingAction action)
        {
            _binding.UpdateTargetAction = action;
            return this;
        }

        /// <summary>
        /// Set <see cref="IBinding"/> <see cref="IBinding.UpdateSource"/> action
        /// </summary>
        /// <param name="action">update source action</param>
        public PropertyBindingTargetBuilder<T> UpdateSource(PropertyBindingAction action)
        {
            _binding.UpdateSourceAction = action;
            return this;
        }

        PropertyBindingAction Before(PropertyBindingAction d, PropertyBindingAction action)
        {
            return (b, c) =>
            {
                action(b, c);
                d(b, c);
            };
        }
        
        /// <summary>
        /// Set action to call right before <see cref="IBinding.UpdateTarget"/>.
        /// </summary>
        /// <param name="action">before update target action</param>
        public PropertyBindingTargetBuilder<T> BeforeTargetUpdate(PropertyBindingAction action)
        {
            _binding.UpdateTargetAction = Before(_binding.UpdateTargetAction, action);
            return this;
        }

        /// <summary>
        /// Set action to call right before <see cref="IBinding.UpdateSource"/>.
        /// </summary>
        /// <param name="action">before update source action</param>
        public PropertyBindingTargetBuilder<T> BeforeSourceUpdate(PropertyBindingAction action)
        {
            _binding.UpdateSourceAction = Before(_binding.UpdateSourceAction, action);
            return this;
        }

        PropertyBindingAction After(PropertyBindingAction d, PropertyBindingAction action)
        {
            return (t, s) =>
            {
                d(t, s);
                action(t, s);
            };
        }

        /// <summary>
        /// Set action to call right after <see cref="IBinding.UpdateTarget"/>.
        /// </summary>
        /// <param name="action">after update target action</param>
        public PropertyBindingTargetBuilder<T> AfterTargetUpdate(PropertyBindingAction action)
        {
            _binding.UpdateTargetAction = After(_binding.UpdateTargetAction, action);
            return this;
        }

        /// <summary>
        /// Set action to call right after <see cref="IBinding.UpdateSource"/>.
        /// </summary>
        /// <param name="action">after update source action</param>
        public PropertyBindingTargetBuilder<T> AfterSourceUpdate(PropertyBindingAction action)
        {
            _binding.UpdateSourceAction = After(_binding.UpdateSourceAction, action);
            return this;
        }

        /// <summary>
        /// Set action to call right after <see cref="IBinding.UpdateSource"/> or <see cref="IBinding.UpdateTarget"/>.
        /// </summary>
        /// <param name="action">after update source or update target action</param>
        public PropertyBindingTargetBuilder<T> AfterUpdate(PropertyBindingAction action)
        {
            AfterSourceUpdate(action);
            AfterTargetUpdate(action);
            return this;
        }
        /// <summary>
        /// Set action to call right before <see cref="IBinding.UpdateSource"/> or <see cref="IBinding.UpdateTarget"/>.
        /// </summary>
        /// <param name="action">before update source or update target action</param>
        public PropertyBindingTargetBuilder<T> BeforeUpdate(PropertyBindingAction action)
        {
            BeforeSourceUpdate(action);
            BeforeTargetUpdate(action);
            return this;
        }

        /// <summary>
        /// Configures binding to only move data from <see cref="IPropertyBinding.Source"/> to 
        /// <see cref="IPropertyBinding.Target"/>.
        /// </summary>
        public PropertyBindingTargetBuilder<T> OneWay()
        {
            _binding.DontUpdateSource();
            return this;
        }

        /// <summary>
        /// Configures binding to only move data from <see cref="IPropertyBinding.Target"/> to 
        /// <see cref="IPropertyBinding.Source"/>.
        /// </summary>
        public PropertyBindingTargetBuilder<T> OneWayToSource()
        {
            var old = _binding.UpdateSourceAction;
            var b = _binding;

            void InitTargetAndStopUpdating(IPropertyBinding t, ChangeSource s)
            {
                old(t, s);
                b.DontUpdateSource();
            }

            _binding.UpdateSourceAction = InitTargetAndStopUpdating;
            return this;
        }
    }


    /// <summary>
    /// Property binding builder class.
    /// </summary>
    public static class PropertyBindingBuilder
    {
        public static PropertyBindingSourceBuilder<TTarget> Create<TSource, TTarget>(
            TSource source,
            Expression<Func<TSource, TTarget>> property)
            where TSource : INotifyPropertyChanged
        {
            Assert.Argument(source, nameof(source)).NotNull();
            Assert.Argument(property, nameof(property)).NotNull();

            var sourceProperty = source.Property(property);
            return Create(sourceProperty);
        }

        public static PropertyBindingSourceBuilder<T> Create<T>(IProperty<T> source)
        {
            Assert.Argument(source, nameof(source)).NotNull();
            return new PropertyBindingSourceBuilder<T>(new PropertyBinding
            {
                Source = source
            });
        }

        public static PropertyBindingSourceBuilder<T> Create<T>(IPropertyBinding binding)
        {
            Assert.Argument(binding, nameof(binding)).NotNull();
            return new PropertyBindingSourceBuilder<T>(binding);
        }
        
        public static PropertyBindingSourceBuilder<T> Property<TOwner, T>(this BindingList list, TOwner source,
            Expression<Func<TOwner, T>> property)
            where TOwner : INotifyPropertyChanged
        {
            var builder = Create(source, property);
            list.Add(builder.Binding);
            return builder;
        }

        public static PropertyBindingSourceBuilder<T> Property<T>(this BindingList list, IProperty<T> source)
        {
            var builder = Create(source);
            list.Add(builder.Binding);
            return builder;
        }

        public static PropertyBindingSourceBuilder<T> Property<T>(this BindingList list,
            IPropertyBinding binding)
        {
            var builder = Create<T>(binding);
            list.Add(builder.Binding);
            return builder;
        }
    }
}