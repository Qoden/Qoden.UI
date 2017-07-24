using System;
using System.Collections.Generic;
using Qoden.Reflection;

namespace Qoden.UI
{
	public static class Cleanup
	{
		//Use generic method to avoid boxing/unboxing when
		//array of structs are passed to this method
		public static void All<T> (params T[] disposables) where T : IDisposable
		{
			List<T> (disposables);
		}

		public static void All (params IDisposable[] disposables)
		{
			List (disposables);
		}

		public static void List<T> (IEnumerable<T> disposables) where T : IDisposable
		{
			if (disposables == null)
				return;
			foreach (var d in disposables) {
				if (!ReferenceEquals(null, d)) 
					d.Dispose ();
			}	
		}

		public static void List (IEnumerable<object> disposables)
		{
			if (disposables == null)
				return;
			foreach (var d in disposables) {
				var disposable = d as IDisposable;
				if (disposable != null) {
					disposable.Dispose ();
				}
			}
		}
			
		#if __IOS__
		public static void Outlets (Foundation.NSObject obj)
		{
			foreach (var outlet in Inspection.AllOutlets(obj)) {
				var disposable = outlet as IDisposable;
				if (disposable != null) {
					disposable.Dispose ();
				}
			}
		}

		#endif
	}
}

