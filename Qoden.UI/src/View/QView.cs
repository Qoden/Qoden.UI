using System;
namespace Qoden.UI
{
    public interface IViewWrapper
    {
        object PlatformView { get; set; }
        Type ViewType { get; }
        object Create(IViewHierarchyBuilder builder);
    }

    public interface IQView<out T> : IViewWrapper
    {
        new T PlatformView { get; }
        new T Create(IViewHierarchyBuilder builder);
    }

    public class QView<T> : IQView<T> where T : class
    {
        public QView()
        { }

        public QView(T target)
        {
            PlatformView = target;
        }

        public T PlatformView { get; set; }

        object IViewWrapper.PlatformView
        {
            get => PlatformView;
            set => PlatformView = (T)value;
        }

        public virtual T Create(IViewHierarchyBuilder builder)
        {
            return (T)builder.MakeView(typeof(T));
        }

        Type IViewWrapper.ViewType => typeof(T);

        object IViewWrapper.Create(IViewHierarchyBuilder builder)
        {
            return Create(builder);
        }

        public override string ToString()
        {
            return PlatformView != null ? "QView: " + PlatformView.ToString() : "Empty QView";
        }
    }
}
