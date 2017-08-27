using System;
using System.Collections;
using Qoden.Util;
using Qoden.Validation;

namespace Qoden.Binding
{
    public static class PropertyConversion
    {
        class ConvertedProperty<T, TS> : IProperty<T>
        {
            readonly IProperty<TS> source;
            readonly Func<TS, T> toTarget;
            readonly Func<T, TS> toSource;

            public ConvertedProperty(
                IProperty<TS> source,
                Func<TS, T> toTargetConversion,
                Func<T, TS> toSourceConversion)
            {
                Assert.Argument(source, nameof(source)).NotNull();
                Assert.Argument(toTargetConversion, nameof(toTargetConversion)).NotNull();
                this.source = source;
                toTarget = toTargetConversion;
                toSource = toSourceConversion;
            }

            public object Owner => source.Owner;

            public T Value
            {
                get => toTarget(source.Value);
                set
                {
                    if (IsReadOnly) throw new InvalidOperationException("Cannot update readonly property");
                    source.Value = toSource(value);
                }
            }

            object IProperty.Value
            {
                get => Value;
                set => Value = (T) value;
            }

            public bool IsReadOnly => toSource == null;

            public bool HasErrors => source.HasErrors;

            public IEnumerable Errors => source.Errors;

            public string Key => source.Key;

            public Type PropertyType => typeof(T);

            IDisposable IProperty.OnPropertyChange(Action<IProperty> action)
            {
                return source.OnPropertyChange(action);
            }

            public IPropertyBindingStrategy BindingStrategy => source.BindingStrategy;
        }

        /// <summary>
        /// Create property whose value is value of source property converted with given functions.
        /// </summary>
        /// <param name="source">source property</param>
        /// <param name="toTargetConversion">Function to convert from source to target</param>
        /// <param name="toSourceConversion">Backwards conversion. If null then converted property throw exception if value is set.</param>
        /// <typeparam name="T">Target type</typeparam>
        /// <typeparam name="TS">Source type</typeparam>
        public static IProperty<T> Convert<T, TS>(
            this IProperty<TS> source,
            Func<TS, T> toTargetConversion,
            Func<T, TS> toSourceConversion = null)
        {
            return new ConvertedProperty<T, TS>(source, toTargetConversion, toSourceConversion);
        }
    }
}