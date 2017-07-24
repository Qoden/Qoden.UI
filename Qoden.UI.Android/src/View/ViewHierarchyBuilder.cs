using System;
using Android.Content;
using Android.Views;
using Qoden.Validation;

namespace Qoden.UI
{
    public class ViewHierarchyBuilder : IViewHierarchyBuilder
    {
        const string TypeMismatchOrNull = "'{Key}' is not an instance of View or null";
        Context _context;
        object[] ConstructorArgs;
        int nextId = 1;

        public ViewHierarchyBuilder(Context context)
        {
            Assert.Argument(context, nameof(context)).NotNull();
            _context = context;
            ConstructorArgs = new object[] { _context };
        }

        public object MakeView(Type t)
        {
            var view = Activator.CreateInstance(t, ConstructorArgs);
            ((View)view).Id = nextId++;
            return view;
        }

        public void AddSubview(object root, object child)
        {
            var rootView = root as ViewGroup;
            Assert.Argument(rootView, nameof(root)).NotNull("'{Key}' is not an instance of ViewGroup or null");
            var childView = child as View;
            Assert.Argument(childView, nameof(child)).NotNull(TypeMismatchOrNull);
            rootView.AddView(childView);
        }

        public void RemoveFromSuperview(object child)
        {
            var childView = child as View;
            Assert.Argument(childView, nameof(child)).NotNull(TypeMismatchOrNull);
            var parent = childView.Parent as ViewGroup;
            if (parent != null)
            {
                parent.RemoveView(childView);
            }
        }
    }
}
