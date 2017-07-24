using System;
namespace Qoden.UI.Test
{
    public class FakeViewHierarchyBuilder : IViewHierarchyBuilder
    {
        public void AddSubview(object root, object child)
        {
            var rootView = root as FakeView;
            var childView = root as FakeView;
            rootView.Subviews.Add(childView);
            childView.Parent = rootView;
        }

        public object MakeView(Type t)
        {
            return Activator.CreateInstance(t);
        }

        public void RemoveFromSuperview(object child)
        {
            var childView = child as FakeView;
            childView.Parent?.Subviews.Remove(childView);
        }
    }
}
