using System;
using System.Collections.Generic;

namespace Qoden.UI
{
    /// <summary>
    /// Holds view model instances
    /// </summary>
    public interface IViewModelStore
    {
        /// <summary>
        /// Get pr create view model with given key. 
        /// </summary>
        /// <returns>View model instance</returns>
        /// <param name="key">View model key</param>
        /// <param name="factory">Factory function to create view model</param>
        T Get<T>(string key, Func<T> factory);
        /// <summary>
        /// Clear store.
        /// </summary>
        void Clear();
    }

    public static class IViewModelStoreExtensions
    {
        public static T Get<T>(this IViewModelStore store) where T : new()
        {
            return store.Get(() => new T());
        }

        public static T Get<T>(this IViewModelStore store, Func<T> factory)
        {
            var key = typeof(T).FullName;
            return store.Get(key, factory);
        }
    }

    public class ViewModelStore : IViewModelStore
    {
        Dictionary<string, object> _viewModels = new Dictionary<string, object>();

        public void Clear()
        {
            _viewModels.Clear();
        }

        public T Get<T>(string key, Func<T> factory)
        {
            object model;
            if (!_viewModels.TryGetValue(key, out model))
            {
                model = factory();
                _viewModels.Add(key, model);
            }

            return (T)model;
        }
    }
}
