using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qoden.Reflection;
#pragma warning disable CS1701 // Assuming assembly reference matches identity

namespace Qoden.UI
{
	/// <summary>
	/// View builder.
	/// </summary>
	public class ViewHierarchy : IDisposable
	{
		private List<PlatformView> views = new List<PlatformView>();
		private PlatformView root;

		public ViewHierarchy(PlatformView root)
		{
			this.root = root;
			Build();
		}

		public void Dispose()
		{
			root.Dispose();
			foreach (var view in views)
			{
				view.Dispose();
			}
		}

		private void Build()
		{
			Build(root);
		}

		void Build(PlatformView view)
		{
			var viewType = view.Native.GetType();
			var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

			var properties = viewType.GetProperties(bindingFlags)
									  .OrderBy(x =>
									  {
										  var viewAttr = x.GetCustomAttribute<ViewAttribute>();
										  return viewAttr != null ? viewAttr.Order : int.MaxValue;
									  });

			foreach (var property in properties)
			{
				var obj = Build(view, property);
				if (obj != null)
				{
					views.Add(obj.Value);
				}
			}
		}

		private static PlatformView? Build(PlatformView view, MemberInfo member)
		{
			PlatformView? subview = null;
			var viewAttribute = member.GetCustomAttribute<ViewAttribute>();
			if (viewAttribute != null)
			{
				subview = InstantiateView(view, member);
			}
			var decorator = member.GetCustomAttribute<DecoratorAttribute>();
			if (decorator != null)
			{
				var child = subview.HasValue ? subview.Value.Native : null;
				if (child == null) 
				{
					child = Inspection.GetValue(view, member);
					if (child == null) 
					{
						throw new InvalidOperationException($"Cannot decorate {member.DeclaringType.Name}.{member.Name}");
					}
					subview = new PlatformView(child);
				}
				decorator.Decorate(child);
			}
			if (subview != null)
			{
				view.AddSubview(subview.Value);
			}
			return subview;
		}

		static PlatformView InstantiateView(PlatformView view, MemberInfo member)
		{
			object subview = Inspection.GetValue(view.Native, member);
			if (subview == null)
			{
				var outletType = Inspection.GetMemberType(member);
				var child = view.CreateChild(outletType);
				subview = child.Native;
				Inspection.SetValue(view.Native, member, subview);
			}
			return new PlatformView(subview);
		}

	}

	public class ViewAttribute : Attribute
	{
		public ViewAttribute(int order = int.MaxValue)
		{
			Order = order;
		}
		public int Order { get; private set; }
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
