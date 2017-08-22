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
        /// Sets view model object for a given key.
        /// </summary>
        /// <param name="key">View model key</param>
        /// <param name="model">View model</param>
        void Set(string key, object model);

        /// <summary>
        /// Clear store.
        /// </summary>
        void Clear();
    }

    public static class ViewModelStoreExtensions
    {
        /// <summary>
        /// Get view model with key equal to type name
        /// </summary>
        /// <param name="store">View model store</param>
        public static T Get<T>(this IViewModelStore store) where T : new()
        {
            return store.Get(() => new T());
        }

        /// <summary>
        /// Get view model with key equal to type name
        /// </summary>
        /// <param name="store">View model store</param>
        /// <param name="factory">Factory function to create new view model instance</param>
        public static T Get<T>(this IViewModelStore store, Func<T> factory)
        {
            var key = typeof(T).FullName;
            return store.Get(key, factory);
        }

        /// <summary>
        /// Sets view model for a key equal to view model type name
        /// </summary>
        /// <param name="store">View model store (cannot be null)</param>
        /// <param name="viewModel">View model instance (cannot be null)</param>
        public static void Set<T>(this IViewModelStore store, T viewModel)
        {
            store.Set(viewModel.GetType().FullName, viewModel);
        }
    }

    public class ViewModelStore : IViewModelStore
    {
        readonly Dictionary<string, object> _viewModels = new Dictionary<string, object>();

        public void Set(string key, object model)
        {
            _viewModels[key] = model;
        }

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

            return (T) model;
        }
    }
}