using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Qoden.Util;
using Qoden.Validation;

namespace Qoden.Binding
{
    public interface IDataContext : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        IValidator Validator { get; }

        bool Validate();

        bool IsValid { get; }
    }

    public interface IDisposeBag : IDisposable
    {
        void AddDisposable(IDisposable disposable);
    }
    
    /// <summary>
    /// Holds data and objects to be displayed or edited.
    /// </summary>
    /// <remarks>
    /// <see cref="DataContext"/> provides minimal infrastructure for interactive object editing. 
    /// </remarks>
    public class DataContext : IDataContext, IEditableObject, IDisposeBag
    {
        private IValidator _validator;        
        private Dictionary<string, object> _originals;
        private readonly IKeyValueCoding _kvc;
        private readonly List<IDisposable> _disposeBag;

        public DataContext()
        {            
            _kvc = KeyValueCoding.Impl(GetType());
            _disposeBag = new List<IDisposable>();
        }

        protected void RememberAndBeginEdit(string key)
        {
            if (!Editing)
            {
                BeginEdit();
            }
            Remember(key);
        }

        protected void Remember(string key)
        {
            if (Editing)
            {
                if (!_originals.ContainsKey(key))
                {
                    _originals[key] = _kvc.Get(this, key);
                    if (_originals.Count == 1)
                    {
                        RaisePropertyChanged(nameof(HasChanges));
                    }
                }
            }
        }

        public IReadOnlyDictionary<string, object> Changes => _originals;

        public bool Editing => _originals != null;

        public bool HasChanges => _originals != null && _originals.Count > 0;

        public void BeginEdit()
        {
            if (!Editing)
            {
                _originals = new Dictionary<string, object>();
                OnBeginEdit();
                RaisePropertyChanged(nameof(Editing));
            }
        }

        protected virtual void OnBeginEdit()
        {
        }

        public void CancelEdit()
        {
            if (!Editing) return;
            OnCancelEdit();
            foreach (var kv in _originals)
            {
                _kvc.Set(this, kv.Key, kv.Value);
            }
            _originals = null;
            RaisePropertyChanged(nameof(HasChanges));
            RaisePropertyChanged(nameof(Editing));
        }

        protected virtual void OnCancelEdit()
        {
        }

        public void EndEdit()
        {
            if (!Editing) return;
            OnEndEdit();
            _originals = null;
            RaisePropertyChanged(nameof(HasChanges));
            RaisePropertyChanged(nameof(Editing));
        }

        protected virtual void OnEndEdit()
        {
        }

        protected bool SetProperty<T>(ref T storage, T value, string propertyName)
        {
            if (Validating || EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Indicate if data context performs validation when property changes.
        /// </summary>
        public bool PerformsValidation
        {
            get => _validator != DevNullValidator.Instance;
            set
            {
                if (value)
                {
                    //If validator is DevNull not then nullify it. It will be created lazily when needed.
                    //Otherwise there is already validator available and we should not do anything
                    if (_validator == DevNullValidator.Instance)
                    {
                        _validator = null;
                    }
                }
                else
                {
                    if (_validator != null)
                        _validator.ErrorsChanged -= _validator_ErrorsChanged;
                    _validator = DevNullValidator.Instance;
                }
            }
        }

        /// <summary>
        /// Get <see cref="DataContext"/> Validator
        /// </summary>
        public IValidator Validator
        {
            get
            {
                if (_validator == null)
                {
                    _validator = new Validator();
                    _validator.ErrorsChanged += _validator_ErrorsChanged;
                    if (!Validating)
                    {
                        Validate();
                    }
                }
                return _validator;
            }
        }

        private void _validator_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
            RaisePropertyChanged(nameof(IsValid));
        }

        /// <summary>
        /// Indicate if data context is valid.
        /// </summary>
        public bool IsValid
        {
            get => Validator.IsValid;
        }

        /// <summary>
        /// Indicate if validation is in process. See <see cref="Validate"/> for details.
        /// </summary>
	    public bool Validating { get; private set; }

        /// <summary>
        /// Validate <see cref="DataContext"/> by calling all property setters defined in it. 
        /// Make sure your setters ready to be called during validation. See remarks for details.
        /// </summary>
        /// <remarks>
        /// This method does following
        /// 1. Transition object into Validating state
        /// 2. Call all properties not marked with <see cref="DontCallDuringValidationAttribute"/> to trigger validation errors.
        /// 3. Call <see cref="OnValidate"/> after properties processed
        /// 4. Fire <see cref="PropertyChanged"/> for every property with errors.
        /// 5. Transition back to normal state.
        /// </remarks>
        /// <returns>true if validation successfull (no errors found)</returns>
		public bool Validate()
        {
            Validating = true;
            var properties = Inspection.InstanceProperties(GetType());
            try
            {
                Validator.Clear();
                //trigger all properties to generate errors
                foreach (var p in properties)
                {
                    if (!p.HasAttribute<DontCallDuringValidationAttribute>() && p.CanWrite && p.CanRead)
                    {
                        var val = _kvc.Get(this, p.Name);
                        _kvc.Set(this, p.Name, val);
                    }
                }
                OnValidate();
            }
            finally
            {
                foreach (var p in properties)
                {
                    if (Validator.HasErrorsForKey(p.Name))
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p.Name));
                    }
                }
                Validating = false;
            }
            return Validator.HasErrors;
        }

        /// <summary>
        /// Called after <see cref="DataContext"/> properties validated. See <see cref="Validate"/> for details.
        /// </summary>
		protected virtual void OnValidate()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise <see cref="PropertyChanged"/> event with provided args
        /// </summary>
        /// <param name="key">event arguments</param>
		protected void RaisePropertyChanged(PropertyChangedEventArgs key)
        {
            if (!Validating)
            {
                PropertyChanged?.Invoke(this, key);
            }
        }

        /// <summary>
        /// Convenience method to raise <see cref="PropertyChanged"/> event inside <see cref="DataContext"/> properties.
        /// </summary>
        /// <param name="key">property name, autoamtically generated by compiler</param>
        protected void RaisePropertyChanged(string key)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(key));
        }

        /// <summary>
        /// Fired when new <see cref="Validator"/> detect new error or errors removed from <see cref="Validator"/>
        /// </summary>
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Convenience method to get errors from <see cref="Validator"/>. Pass null as argument to get all errors.
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <returns>List of errors for given property</returns>
		public System.Collections.IEnumerable GetErrors(string propertyName = null)
        {
            return Validator.GetErrors(propertyName);
        }

        /// <summary>
        /// Indicate if <see cref="DataContext"/> has errors in it <see cref="Validator"/>.
        /// </summary>
		public bool HasErrors => Validator.HasErrors;

        public void AddDisposable(IDisposable disposable)
        {
            _disposeBag.Add(disposable);
        }

        public void AddDisposable(IEnumerable<IDisposable> disposables)
        {
            _disposeBag.AddRange(disposables);
        }

        public void RemoveDisposable(IDisposable disposable)
        {
            disposable?.Dispose();
            _disposeBag.Remove(disposable);
        }

        public void RemoveDisposable(IEnumerable<IDisposable> disposables)
        {
            foreach (var disposable in disposables)
            {
                disposable?.Dispose();
                _disposeBag.Remove(disposable);
            }
        }
        
        public void Dispose()
        {
            foreach (var disposable in _disposeBag)
            {
                disposable?.Dispose();
            }
            _disposeBag.Clear();
        }
    }

    public static class DataContextExtensions
    {
        public static bool HasErrorsForKey(this IDataContext obj, string key)
        {
            return obj.Validator.HasErrorsForKey(key);
        }

        public static bool HasErrorsForKey<T, TK>(this IDataContext obj, Expression<Func<T, TK>> key)
        {
            return HasErrorsForKey(obj, PropertySupport.ExtractPropertyName(key));
        }
    }
}
