using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qoden.Reflection;
using Qoden.Validation;
#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
    /// <summary>
    /// Builds and maintain view hierarchy defined with View and Decorate attributes.
    /// </summary>
    public class ViewHierarchy : IDisposable
    {
        List<object> _views = new List<object>();
        object _viewHierarchyRoot, _viewHolder;
        IViewHierarchyBuilder _builder;

        public ViewHierarchy(object viewHolder, object root, IViewHierarchyBuilder builder)
        {
            Assert.Argument(viewHolder, nameof(viewHolder)).NotNull();
            Assert.Argument(root, nameof(root)).NotNull();
            Assert.Argument(builder, nameof(builder)).NotNull();
            _viewHolder = viewHolder;
            _viewHierarchyRoot = root;
            _builder = builder;
            Build();
        }

        public ViewHierarchy(object root, IViewHierarchyBuilder builder) : this(root, root, builder)
        {
        }

        public void Dispose()
        {
            (_viewHolder as IDisposable)?.Dispose();
            foreach (var view in _views)
            {
                (view as IDisposable)?.Dispose();
            }
        }

        public IEnumerable<object> Subviews => _views;

        void Build()
        {
            var viewHolderType = _viewHolder.GetType();
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

            var properties = viewHolderType.GetProperties(bindingFlags)
                                      .OrderBy(x =>
                                      {
                                          var viewAttr = x.GetCustomAttribute<ViewAttribute>();
                                          return viewAttr != null ? viewAttr.Order : int.MaxValue;
                                      });

            foreach (var property in properties)
            {
                var obj = BuildChild(property);
                if (obj != null)
                {
                    _views.Add(obj);
                }
            }

            var decoratorAttribute = viewHolderType.GetTypeInfo().GetCustomAttribute<DecoratorAttribute>();
            decoratorAttribute?.Decorate(_viewHolder);
        }

        private object BuildChild(MemberInfo member)
        {
            object subview = null;
            //Create view
            var viewAttribute = member.GetCustomAttribute<ViewAttribute>();
            if (viewAttribute != null)
            {
                subview = InstantiateView(member);
            }

            //Decorate member
            var decoratorAttribute = member.GetCustomAttribute<DecoratorAttribute>();
            if (decoratorAttribute != null)
            {
                var decoratable = subview;
                if (decoratable == null)
                {
                    decoratable = Inspection.GetValue(_viewHolder, member);
                }
                if (decoratable == null)
                {
                    throw new InvalidOperationException($"Cannot decorate null value at {_viewHolder.GetType().FullName}.{member.Name}");
                }
                if (decoratable != null)
                {
                    decoratorAttribute.Decorate(decoratable);
                }
            }
            //Add view
            if (viewAttribute != null && subview != null && viewAttribute.AddToParent)
            {
                subview = (subview as IViewWrapper)?.PlatformView ?? subview;
                _builder.AddSubview(_viewHierarchyRoot, subview);
            }

            return subview;
        }

        object InstantiateView(MemberInfo member)
        {
            object subview = Inspection.GetValue(_viewHolder, member);
            if (subview == null)
            {
                var outletType = Inspection.GetMemberType(member);
                if (typeof(IViewWrapper).IsAssignableFrom(outletType))
                {
                    subview = Activator.CreateInstance(outletType);
                }
                else
                {
                    subview = _builder.MakeView(outletType);

                }
                Inspection.SetValue(_viewHolder, member, subview);
            }

            var wrapper = subview as IViewWrapper;
            if (wrapper != null)
            {
                if (wrapper.PlatformView == null)
                {
                    wrapper.PlatformView = wrapper.Create(_builder);
                }
            }

            return subview;
        }
    }

    public class ViewAttribute : Attribute
    {
        public ViewAttribute(int order = int.MaxValue, bool addToParent = true)
        {
            Order = order;
            AddToParent = addToParent;
        }
        public int Order { get; private set; }
        public bool AddToParent { get; private set; }
    }

    public class DecoratorAttribute : Attribute
    {
        MethodInfo[] methods;
        readonly string[] methodNames;
        readonly Type type;

        public DecoratorAttribute(Type type, params string[] methods)
        {
            methodNames = methods;
            this.type = type;
        }

        public void Decorate(object view)
        {
            if (methods == null)
            {
                var argTypes = new[] { view.GetType() };
                methods = methodNames.Select(methodName =>
                {
                    var method = type.GetMethod(methodName, argTypes);
                    if (method == null)
                    {
                        //TODO implement cross platform logging and log this condition properly
                        Console.WriteLine("Decorator method not found {0} {1}", type, methodName);
                        //LOG.Error("Decorator method not found {0} {1}", type, methodName);
                    }
                    return method;
                }).ToArray();
            }

            var args = new[] { view };
            foreach (var method in methods)
            {
                if (method != null)
                {
                    method.Invoke(null, args);
                }

            }
        }
    }
}
