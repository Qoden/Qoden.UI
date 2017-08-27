using System;
using System.ComponentModel;
using Qoden.Util;
using Qoden.Validation;

namespace Qoden.Binding
{
    public interface IObjectBindingStrategy
    {
        IDisposable SubscribeToChanges(object @object, PropertyChangedEventHandler action);
    }

    public class NotifyPropertyChangedStrategy : IObjectBindingStrategy
    {
        public static readonly RuntimeEvent PropertyChanged = new RuntimeEvent(typeof(INotifyPropertyChanged), "PropertyChanged");
        public static readonly NotifyPropertyChangedStrategy Instance = new NotifyPropertyChangedStrategy();

        public IDisposable SubscribeToChanges(object @object, PropertyChangedEventHandler action)
        {
            Assert.Argument(@object, nameof(@object)).NotNull();
            var o = Assert.Argument(@object as INotifyPropertyChanged, nameof(@object))
                          .NotNull("Object does not implement INotifyPropertyChanged")
                          .Value;
            Assert.Argument(action, nameof(action)).NotNull();
            return new EventSubscription(PropertyChanged, action, @object);
        }
    }

    /// <summary>
    /// Bind handlers to all property changes inside given object.
    /// </summary>
    public class ObjectBinding : IBinding
    {
        private IDisposable _sourceSubscription;

        INotifyPropertyChanged _source;

        public INotifyPropertyChanged Source
        {
            get => _source;
            set
            {
                Assert.Property(value).NotNull();
                Assert.State(Bound).IsFalse("Cannot change Source of already Bound binding");
                _source = value;
            }
        }

        public bool Enabled { get; set; }

        public bool Bound => _sourceSubscription != null;

        IObjectBindingStrategy _bindingStrategy = NotifyPropertyChangedStrategy.Instance;
        public IObjectBindingStrategy BindingStrategy 
        { 
            get => _bindingStrategy; 
            set 
            {
                Assert.Property(value).NotNull();
                _bindingStrategy = value;   
            }
        }

        public void Bind()
        {
            if (Bound)
                return;
            Assert.State(Source != null).IsTrue("Source is not set");
            _sourceSubscription = BindingStrategy.SubscribeToChanges(Source, Source_Change);
        }

        void Source_Change(object sender, PropertyChangedEventArgs e)
        {
            if (Enabled && PropertyChanged != null)
            {
                var handler = PropertyChanged;
                handler.Invoke(Source, e);
            }
        }

        public void Unbind()
        {
            if (!Bound) return;
            _sourceSubscription.Dispose();
            _sourceSubscription = null;
        }

        public void UpdateSource()
        {
        }

        public void UpdateTarget()
        {
            Source_Change(Source, new PropertyChangedEventArgs(""));
        }

        PropertyChangedEventHandler _propertyChanged;
        public PropertyChangedEventHandler PropertyChanged 
        { 
            get => _propertyChanged; 
            set
            {
                Assert.Property(value).NotNull();
                _propertyChanged = value;
            }
        }
    }

    public static class ObjectBindingBindingListExtensions 
    {
        public static void Object(this BindingList list, INotifyPropertyChanged @object, PropertyChangedEventHandler handler)
        {
            list.Add(new ObjectBinding
            {
                Source = @object,
                PropertyChanged = handler
            });
        }
    }
}
