using System;
namespace Qoden.UI
{
    /// <summary>
    /// Abstraction used to build views hierarchies in platform independednt manner
    /// </summary>
    public interface IViewHierarchyBuilder
    {
        /// <summary>
        /// Create new view instance
        /// </summary>
        object MakeView(Type t);
        /// <summary>
        /// Add subview to parent view
        /// </summary>
        /// <exception cref="ArgumentException">If root or child is not a view instance</exception>
        void AddSubview(object root, object child);
        /// <summary>
        /// Remove view from hierarchy
        /// </summary>
        /// <exception cref="ArgumentException">If child is not a view instance</exception>
        void RemoveFromSuperview(object child);
    }
}
