using System;
using Qoden.Validation;
using UIKit;

namespace Qoden.UI
{
    public class ViewHierarchyBuilder : IViewHierarchyBuilder
    {
        const string TypeMismatchOrNull = "'{Key}' is not an instance of UIView or null";

        public object MakeView(Type t)
        {
            if (t == typeof(UIButton))
            {
                return new UIButton(UIButtonType.RoundedRect);
            }
            return Activator.CreateInstance(t);
        }

        public void AddSubview(object root, object child)
        {
            var rootView = root as UIView;
            Assert.Argument(rootView, nameof(root)).NotNull(TypeMismatchOrNull);
            var childView = child as UIView;
            Assert.Argument(childView, nameof(child)).NotNull(TypeMismatchOrNull);
            rootView.AddSubview(childView);
        }

        public void RemoveFromSuperview(object child)
        {
            var childView = child as UIView;
            Assert.Argument(childView, nameof(child)).NotNull(TypeMismatchOrNull);
            childView.RemoveFromSuperview();
        }

        public static readonly ViewHierarchyBuilder Instance = new ViewHierarchyBuilder();
    }
}
